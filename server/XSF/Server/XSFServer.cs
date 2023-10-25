//////////////////////////////////////////////////////////////////////////
// 
// 文件：XSF/Server/XSFServer.cs
// 作者：Xoen Xie
// 时间：2023/07/01
// 描述：Server
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8600, CS8602, CS8604, CS8618, CS8625
using System.Xml;

namespace XSF
{
    internal class XSFServer : Singleton<XSFServer>, IServer
    {
        enum RunStatus
        {
            None = 0,
            Init,
            WaitStart,
            Start,
            RunningCheck,
            Running,
            StopCheck0,
            StopCheck,
            Stop,
        }

        struct ModuleInfo
        {
            public IModule module;
            public ModuleInit initData;
        }

        private List<ModuleInfo> m_InitList;
        private int MaxModuleID;
        private IModule [] m_Modules;
        private IModule [] m_StepModule;

        private ServerID m_SID;
        public ServerID SID { get { return m_SID; } }

        public uint ID {get; private set;}

        public uint[] Ports { get; private set; }

        public ServerInit InitData { get; private set; }

        public XSFConfig Config { get; private set; }

        private Thread m_MainThread;
        private ManualResetEvent m_MainEvent;
        private ManualResetEvent m_StopEvent;
        private bool m_bIsRunning;

        private RunStatus m_nStatus;

        private int m_nExitFlag;

        public bool IsRunning 
        { 
            get
            {
                return m_nStatus == RunStatus.Running;
            }
        }

        private ulong m_LastUpdateTime;

        public XSFServer()
        {
            m_InitList = new List<ModuleInfo>();
            Config = new XSFConfig();
            Ports = new uint[(int)EP.Max];
        }

        public void Init(EP ep, string[] args)
        {
            m_SID.Type = (byte)ep;
            
            ReadArgs(args);
            XSFLogger.Init();
            ReadConfig();

            XSFTimer.Instance.Create();
        }

        public void SetID(uint nID)
        {
            ID = nID;
            m_SID = ServerID.GetSID(ID);
        }

        public void SetPort(byte ep, uint port)
        {
            Ports[ep] = port;
        }

        public void DoStart()
        {
            if(m_nStatus == RunStatus.WaitStart)
            {
                Serilog.Log.Information("XSFServer DoStart call ....");
                m_nStatus = RunStatus.Start;
            }
        }

        public void Run()
        {
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;

            m_MainThread = new Thread(MainThreadMethod);
            m_MainThread.Start();
            m_MainThread.Join();

            m_nExitFlag ++;

            if(m_StopEvent != null)
            {
                m_StopEvent.Set();
            }

            Serilog.Log.Information("Server Run 退出");
        }

        private void Release()
        {
            Serilog.Log.Information("服务器所有处理完毕，退出");

            XSFLogger.End();
        }

        internal void SpeedUp()
        {
            m_MainEvent.Set();
        }

        void OnProcessExit(object? sender, EventArgs e)
        {
            Serilog.Log.Information("服务器结束中");

            if(m_nExitFlag == 0)
            {
                m_StopEvent = new ManualResetEvent(false);
                Stop();
                Serilog.Log.Information("服务器结束中，等待主线程结束");
                m_StopEvent.WaitOne();
            }
            
            Release();
        }

        public void Stop()
        {
            if(m_nStatus == RunStatus.Running)
                m_nStatus = RunStatus.StopCheck0;
            else
                m_nStatus = RunStatus.Stop;

            m_MainEvent.Set();
        }

        void MainThreadMethod()
        {
            Serilog.Log.Debug("主逻辑线程启动");
            m_MainEvent = new ManualResetEvent(false);
            m_bIsRunning = true;
            m_nStatus = RunStatus.Init;

            while(m_bIsRunning)
            {
                bool res = m_MainEvent.WaitOne(60);
                if(res)
                {
                    m_MainEvent.Reset();
                }

                try
                {
                    StatusUpdate();
                
                    XSFNet.Instance.Dispath();
                    XSFTimer.Instance.Dispatch();
                }
                catch(Exception e)
                {
                    Serilog.Log.Error("MainThreadMethod Catch exception, message=" + e.Message);
                }
            }

            Serilog.Log.Debug("主逻辑线程已退出");

            XSFTimer.Instance.Release();
        }

        private void StatusUpdate()
        {
            switch(m_nStatus)
            {
            case RunStatus.Init:
                {
                    m_Modules = new IModule[MaxModuleID+1];
                    m_StepModule = new IModule[MaxModuleID+1];
                    for(int i = 0; i < m_InitList.Count; i ++)
                    {
                        var info = m_InitList[i];
                        m_Modules[info.initData.ID] = info.module;
                        Serilog.Log.Information("模块初始化, id={0}, name={1}", info.initData.ID, info.initData.Name);
                        if(!info.module.Init(info.initData))
                        {
                            m_nStatus = RunStatus.Stop;
                            break;
                        }
                    }

                    for(int i = 0; i < m_Modules.Length; i ++)
                    {
                        if(m_Modules[i] != null)
                        {
                            Serilog.Log.Information("模块注册, id={0}, name={1}", m_Modules[i].ID, m_Modules[i].Name);
                            m_Modules[i].DoRegist();
                        }
                            
                    }

                    int WaitStart = 0;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     
                    for(int i = 0; i < m_Modules.Length; i ++)
                    {
                        if(m_Modules[i] != null )
                        {
                            if(m_Modules[i].NoWaitStart)
                            {
                                Serilog.Log.Information("模块不等待开始, id={0}, name={1}", m_Modules[i].ID, m_Modules[i].Name);
                                if(!m_Modules[i].Start())
                                {
                                    m_nStatus = RunStatus.Stop;
                                    break;
                                }
                            }
                            else
                            {
                                WaitStart ++;
                            }
                        }
                    }

                    if(m_nStatus != RunStatus.Stop)
                    {
                        if(WaitStart > 0)
                        {
                            m_nStatus = RunStatus.WaitStart;
                        }
                        else
                        {
                            m_nStatus = RunStatus.Start;
                        }
                    }
                }
                break;

            case RunStatus.Start:
                {
                    for(int i = 0; i < m_Modules.Length; i ++)
                    {
                        if(m_Modules[i] != null && !m_Modules[i].NoWaitStart)
                        {
                            Serilog.Log.Information("模块等待开始, id={0}, name={1}", m_Modules[i].ID, m_Modules[i].Name);
                            if(!m_Modules[i].Start())
                            {
                                m_nStatus = RunStatus.Stop;
                                break;
                            }
                        }
                    }

                    Array.Copy(m_Modules, m_StepModule, m_Modules.Length);

                    Serilog.Log.Information("进入开服检测 ...");
                    m_nStatus = RunStatus.RunningCheck;
                }
                break;

            case RunStatus.RunningCheck:
                {
                    var isAllOK = true;
                    for(int i = 0; i < m_StepModule.Length; i ++)
                    {
                        if(m_StepModule[i] != null)
                        {
                            ModuleRunCode RunCode = m_StepModule[i].OnStartCheck();
                            if(RunCode == ModuleRunCode.OK)
                            {
                                Serilog.Log.Information("模块启动OK, id={0}, name={1}", m_StepModule[i].ID, m_StepModule[i].Name);
                                m_StepModule[i] = null;
                            }
                            else if(RunCode == ModuleRunCode.Error)
                            {
                                m_nStatus = RunStatus.Stop;
                                break;
                            }
                            else if(RunCode == ModuleRunCode.Wait)
                            {
                                isAllOK = false;
                            }
                        }
                    }

                    if(isAllOK)
                    {
                        Serilog.Log.Information("==== [{0} {1}-{2}-{3}] 所有模块都已启动完毕 ====", XSFUtil.Server.ID, XSFUtil.Server.SID.ID, XSFUtil.EP2CNName(XSFUtil.Server.SID.Type), XSFUtil.Server.SID.Index);
                        for(int i = 0; i < m_Modules.Length; i ++)
                        {
                            if(m_Modules[i] != null)
                                m_Modules[i].OnOK();
                        }

                        m_nStatus = RunStatus.Running;
                        m_LastUpdateTime = XSFUtil.CurrentMS;
                    }
                }
                break;

            case RunStatus.Running:
                {
                    var current = XSFUtil.CurrentMS;
                    if(current >= m_LastUpdateTime + 20)
                    {
                        var nDeltaTime = current - m_LastUpdateTime;

                        for(int i = 0; i < m_Modules.Length; i ++)
                        {
                            if(m_Modules[i] != null)
                                m_Modules[i].OnUpdate((uint)nDeltaTime);
                        }

                        m_LastUpdateTime = current;
                    }
                    
                }
                break;

            case RunStatus.StopCheck0:
                {
                    for(int i = 0; i < m_Modules.Length; i ++)
                    {
                        if(m_Modules[i] != null) {
                            Serilog.Log.Information("模块开始关闭, id={0}, name={1}", m_Modules[i].ID, m_Modules[i].Name);
                            m_Modules[i].OnClose();
                        }
                    }

                    Array.Copy(m_Modules, m_StepModule, m_Modules.Length);

                    m_nStatus = RunStatus.StopCheck;
                }
                break;

            case RunStatus.StopCheck:
                {
                    var isAllOK = true;
                    for(int i = m_StepModule.Length - 1; i >= 0; i --)
                    {
                        if(m_StepModule[i] != null)
                        {
                            ModuleRunCode RunCode = m_StepModule[i].OnCloseCheck();
                            if(RunCode == ModuleRunCode.OK)
                            {
                                Serilog.Log.Information("模块关闭完成, id={0}, name={1}", m_StepModule[i].ID, m_StepModule[i].Name);
                                m_StepModule[i] = null;
                            }
                            else
                            {
                                isAllOK = false;
                            }
                        }
                    }

                    if(isAllOK)
                    {
                        m_nStatus = RunStatus.Stop;
                    }
                }
                break;

            case RunStatus.Stop:
                {
                    Serilog.Log.Information("==== [{0} {1}-{2}-{3}] 所有模块都已正常关闭 ====", XSFUtil.Server.ID, XSFUtil.Server.SID.ID, XSFUtil.EP2CNName(XSFUtil.Server.SID.Type), XSFUtil.Server.SID.Index);
                    for(int i = 0; i < m_Modules.Length; i ++)
                    {
                        if(m_Modules[i] != null)
                            m_Modules[i].Release();
                    }

                    m_nStatus = RunStatus.None;
                    m_bIsRunning = false;
                }
                break;
            }
        }

        private bool ReadArgs(string[] args)
        {
            ServerInit si = new ServerInit();

            try
            {
                string argData = "";
                if(XSFUtil.GetArg(args, "-tag", out argData))
                {
                    si.ServerTag = argData;
                }
                else
                {
                    throw new ServerArgException("启动参数中未指定 -tag 值");
                }

                si.OutputConsole = XSFUtil.GetArg(args, "-c", out argData);

                if(XSFUtil.GetArg(args, "-i", out argData))
                {
                    m_SID.ID = Convert.ToUInt16(argData);
                    ID = ServerID.GetID(m_SID);
                }
                else
                {
                    throw new ServerArgException("启动参数中未指定 -i 值");
                }

                if(XSFUtil.GetArg(args, "-rd", out argData))
                {
                    si.WorkDir = argData;
                }
                else
                {
                    si.WorkDir = Environment.CurrentDirectory;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine($"游戏执行参数错误, exception={e.Message}, stack={e.StackTrace}");
                Environment.Exit(0);
                return false;
            }

            InitData = si;

            return true;
        }

        private void ReadConfig()
        {
            string filepath = $"{InitData.WorkDir}/config/{InitData.ServerTag}/config.xml";
            try
            {   
                if(!File.Exists(filepath))
                {
                    throw new XSFSchemaLoadException("服务器配置不存在, path=" + filepath);
                }

                XMLReader reader = new XMLReader();
                var data = File.ReadAllText(filepath);
                reader.Read(data);

                XmlElement eleConfig = reader.mRootNode.SelectSingleNode("config") as XmlElement;

                {
                    XmlElement ele = eleConfig.SelectSingleNode("name") as XmlElement;
                    Config.Name = XMLReader.GetString(ele, null);

                    ele = eleConfig.SelectSingleNode("desc") as XmlElement;
                    Config.Desc = XMLReader.GetString(ele, null);

                    ele = eleConfig.SelectSingleNode("auto_start") as XmlElement;
                    Config.AutoStart = XMLReader.GetBoolean(ele, null);

                    ele = eleConfig.SelectSingleNode("htcheck") as XmlElement;
                    Config.HeartbeatCheck = XMLReader.GetUInt(ele, null);

                    ele = eleConfig.SelectSingleNode("httimeout") as XmlElement;
                    Config.HeartbeatTimeout = XMLReader.GetUInt(ele, null);

                    ele = eleConfig.SelectSingleNode("htinterval") as XmlElement;
                    Config.HeartbeatInterval = XMLReader.GetUInt(ele, null);

                    ele = eleConfig.SelectSingleNode("rc_interval") as XmlElement;
                    Config.ReconnectInterval = XMLReader.GetUInt(ele, null);

                    ele = eleConfig.SelectSingleNode("client_htcheck") as XmlElement;
                    Config.ClientHeartbeatCheck = XMLReader.GetUInt(ele, null);

                    ele = eleConfig.SelectSingleNode("client_httimeout") as XmlElement;
                    Config.ClientHeartbeatTimeout = XMLReader.GetUInt(ele, null);

                    ele = eleConfig.SelectSingleNode("main_center_ip") as XmlElement;
                    Config.MainCenterIP = XMLReader.GetString(ele, null);

                    ele = eleConfig.SelectSingleNode("center_port") as XmlElement;
                    Config.CenterPort = XMLReader.GetUInt(ele, null);

                    ele = eleConfig.SelectSingleNode("inner_port_start") as XmlElement;
                    Config.InnerPortStart = XMLReader.GetUInt(ele, null);

                    ele = eleConfig.SelectSingleNode("out_port_start") as XmlElement;
                    Config.OutPortStart = XMLReader.GetUInt(ele, null);

                    ele = eleConfig.SelectSingleNode("gate_max_count") as XmlElement;
                    Config.GateMaxCount = XMLReader.GetUInt(ele, null);

                    ele = eleConfig.SelectSingleNode("nats") as XmlElement;
                    Config.NatsAddress = XMLReader.GetString(ele, null);

                    ele = eleConfig.SelectSingleNode("account_lifetime") as XmlElement;
                    Config.AccountLifetime = XMLReader.GetUInt(ele, null);

                    ele = eleConfig.SelectSingleNode("actor_save") as XmlElement;
                    Config.ActorSaveTime = XMLReader.GetUInt(ele, null);

                    ele = eleConfig.SelectSingleNode("client_max_msg_length") as XmlElement;
                    Config.ClientMsgMaxLength = XMLReader.GetUInt(ele, null);

                    ele = eleConfig.SelectSingleNode("max_gate") as XmlElement;
                    Config.MaxGate = XMLReader.GetUInt(ele, null);

                    ele = eleConfig.SelectSingleNode("actor_release") as XmlElement;
                    Config.ActorReleaseTime = XMLReader.GetUInt(ele, null);

                }

                Config.Me = new ServerNode();
                Config.Me.ep = (EP)XSFServer.Instance.SID.Type;

                if(Config.Me.ep == EP.Center) {
                    Config.Me.Name = "中心服";
                    m_SID.Index = 1;
                }
                
                XmlElement eleServer = reader.mRootNode.SelectSingleNode("server") as XmlElement;
                var nodes = eleServer.ChildNodes;
                List<ServerNode> nodeList = new List<ServerNode>();
                for(int i = 0; i < nodes.Count; i ++)
                {
                    XmlElement eleItem = nodes[i] as XmlElement;
                    var ep = XMLReader.GetString(eleItem, "ep");
                    var epValue = XSFUtil.Name2EP(ep);
                    //Serilog.Log.Information("ep={0}, epValue={1}", ep, epValue);
                    if(XSFUtil.IsEPInvalid((byte)epValue))
                    {
                        uint count = XMLReader.GetUInt(eleItem, "count");
                        if(count <= 0)
                            count = 1;

                        for(int c = 0; c < count; c ++)
                        {
                            var node = new ServerNode();
                            node.Name = XMLReader.GetString(eleItem, "name");
                            node.ep = epValue;
                            nodeList.Add(node);
                        }
                    }
                    else
                    {
                        throw new XSFSchemaLoadException("ep invalid, ep=" + ep);
                    }
                }

                Config.NodeList = nodeList.ToArray();

                var IsFind = false;
                if(Config.Me.ep == EP.Center)
                    IsFind = true;

                for(int i = 0; i < Config.NodeList.Length; i ++)
                {
                    var node = Config.NodeList[i];
                    Serilog.Log.Information("服务器节点，名称:{0}, EP:{1}", node.Name, XSFUtil.EP2CNName((byte)node.ep));

                    if(node.ep == Config.Me.ep)
                    {
                        Config.Me = node;
                        IsFind = true;
                    }
                }

                if(!IsFind) 
                {
                    throw new XSFSchemaLoadException("未找到服务器节点配置, ep=" + Config.Me.ep);
                }

                Serilog.Log.Information("启动服务器 ID={0}, {1}-{2}-{3}", ID, m_SID.ID, m_SID.Index, m_SID.Type);

            }
            catch(Exception e)
            {
                Serilog.Log.Error($"服务器配置读取出错, exception={e.Message}, stack={e.StackTrace}");
                Environment.Exit(0);
                return;
            }
        }

        public void AddModule(IModule module, ModuleInit init)
        {
            ModuleInfo info = new ModuleInfo();
            info.module = module;
            info.initData = init;
            m_InitList.Add(info);

            if(init.ID > MaxModuleID)
            {
                MaxModuleID = init.ID;
            }
        }

        public IModule GetModule(int nID)
        {
            return m_Modules[nID];
        }
    }
}