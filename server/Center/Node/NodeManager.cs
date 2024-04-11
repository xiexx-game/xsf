//////////////////////////////////////////////////////////////////////////
// 
// 文件：Center/Node/NodeManager.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：节点管理
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8603, CS8600, CS8602, CS8604, CS8618
using XSF;

public class NodeManager : DicNPManager
{
    enum RunStep
    {
        None = 0,
        StartServer,
        WaitHandshake,
        HandshakeDone,
        OK,
    }

    private List<ServerInfo>[] m_LostList;
    private Dictionary<uint, ServerInfo> m_Nodes;
    private byte[] m_NodeIndex;
    private uint m_nInnerPort;
    private uint m_nOutPort;

    private int m_nStartIndex;
    private RunStep m_nStep;

    private ServerNode m_CurStartNode;

    public NodeManager()
    {   
        m_Nodes = new Dictionary<uint, ServerInfo>();
        m_LostList = new List<ServerInfo>[(int)EP.Max];
        m_NodeIndex = new byte[(int)EP.Max];

        for(int i = 0; i < (int)EP.Max; i ++)
        {
            m_LostList[i] = new List<ServerInfo>();
            m_NodeIndex[i] = 1;
        }
    }

    public override bool Init(ModuleInit init)
    {
        m_nInnerPort = XSFCore.Config.InnerPortStart;
        m_nOutPort = XSFCore.Config.OutPortStart;

        return base.Init(init);
    }

    public override bool Start()
    {
        m_nStartIndex = 0;
        m_nStep = RunStep.StartServer;
        
        return base.Start();
    }

    public override void OnStartClose()
    {
        var message = XSFCore.GetMessage((ushort)XsfPbid.SMSGID.CCcStop);
        Broadcast(message, 0);
    }

    public override void DoRegist()
    {
        XSFCore.SetMessageExecutor((ushort)XsfPbid.SMSGID.CcCHandshake, new Executor_Cc_C_Handshake());
        XSFCore.SetMessageExecutor((ushort)XsfPbid.SMSGID.CcCHeartbeat, new Executor_Cc_C_Heartbeat());
        XSFCore.SetMessageExecutor((ushort)XsfPbid.SMSGID.CcCServerOk, new Executor_Cc_C_ServerOk());
    }

    public override ModuleRunCode OnStartCheck() 
    {
        if(!XSFCore.Config.AutoStart)
        {
            return ModuleRunCode.OK;
        }
        else
        {
            switch(m_nStep)
            {
            case RunStep.StartServer:
                {
                    m_CurStartNode = XSFCore.Config.NodeList[m_nStartIndex];

                    Serilog.Log.Information("Start server type={0}",  XSFCore.EP2CNName((byte)m_CurStartNode.ep));

                    string args = $"./single_start.sh {XSFCore.Server.InitData.ServerTag} {XSFCore.EP2Name((byte)m_CurStartNode.ep)} {XSFCore.Server.SID.ID}";
                    XSFCore.StartProcess("sh", args, XSFCore.Server.InitData.WorkDir);

                    Serilog.Log.Information("RunStep.StartServer done ....");

                    m_nStep = RunStep.WaitHandshake;
                }
                break;

            case RunStep.HandshakeDone:
                {
                    m_nStartIndex ++;
                    if(m_nStartIndex >= XSFCore.Config.NodeList.Length)
                    {
                        m_nStep = RunStep.OK;
                    }
                    else
                    {
                        m_nStep = RunStep.StartServer;
                    }
                }
                break;


            case RunStep.OK:
                return ModuleRunCode.OK; 
            } 

            return ModuleRunCode.Wait;
        }
    }

    public ServerInfo AddNode(uint nID, string ip, uint[] ports)
    {
        ServerID sid = ServerID.GetSID(nID);
        ServerInfo newNode = null;
        if(sid.Index == 0 )
        {
            var nodeLost = GetLostNode(sid.Type, nID, false);
            if(nodeLost == null)
            {
                newNode = new ServerInfo();
                GetPort(sid.Type, ref newNode.Ports);

                sid.Index = GetIndex(sid.Type);
                newNode.ID = ServerID.GetID(sid);
                newNode.IP = ip;
                newNode.Status = NodeStatus.New;

                Serilog.Log.Information($"NodeManager AddNode new, id={newNode.ID}, ip={ip}");
            }
            else
            {
                newNode = nodeLost;
                newNode.Status = NodeStatus.New;
                newNode.IP = ip;

                Serilog.Log.Information($"NodeManager AddNode new, find lost server node id={newNode.ID}, ip1={newNode.IP}, ip2={ip}");
            }

            m_Nodes[newNode.ID] = newNode;
        }
        else
        {
            if(m_Nodes.TryGetValue(nID, out newNode))
            {
                Serilog.Log.Information($"NodeManager AddNode find exist 1, id={nID}, ip={ip}");
            }
            else
            {
                var nodeLost = GetLostNode(sid.Type, nID, true);
                if(nodeLost == null)
                {
                    Serilog.Log.Error($"NodeManager AddNode nodeLost not found, id={nID}, ip={ip}");
                    return null;
                }
                else
                {
                    if(!nodeLost.IP.Equals(ip))
                    {
                        Serilog.Log.Error($"NodeManager AddNode find exist, ip error, id={nID}, ip1={nodeLost.IP}, ip2={ip}");
                        return null;
                    }

                    for(int i = 0; i < (int)EP.Max; i ++)
                    {
                        if(nodeLost.Ports[i] != ports[i])
                        {
                            Serilog.Log.Error($"NodeManager AddNode find exist, port error, id={nID}, i={i} port1={nodeLost.Ports[i]}, port2={ports[i]}");
                            return null;
                        }
                    }
                }

                newNode = nodeLost;
                m_Nodes.Add(nID, newNode);
                Serilog.Log.Information($"NodeManager AddNode find exist 2, id={nID}, ip={ip}");
            }
        }

        return newNode;
    }

    public byte GetIndex(byte nEP)
    {
        var index = m_NodeIndex[nEP];
        m_NodeIndex[nEP] ++;

        return index;
    }

    public void GetPort(byte nEP, ref uint[] ports)
    {
        switch((EP)nEP)
        {
        case EP.Gate:
            ports[(int)EP.Client] = GetNextPort(false);
            break;

        case EP.Game:
            ports[(int)EP.Gate] = GetNextPort(true);
            break;

        case EP.Login:
            ports[(int)EP.Gate] = GetNextPort(true);
            break;

        case EP.DB:
            ports[(int)EP.DB] = GetNextPort(true);
            break;

        case EP.Hub:
            ports[(int)EP.Hub] = GetNextPort(true);
            break;

        default:
            break;
        }
    }

    private uint GetNextPort(bool IsInner)
    {
        if(IsInner)
        {
            return m_nInnerPort ++;
        } 
        else 
        {
            return m_nOutPort ++;
        }
    }

    public ServerInfo GetLostNode(byte nEP, uint nID, bool CheckEqual)
    {
        var list = m_LostList[nEP];
        for(int i = 0; i < list.Count; i ++)
        {
            var node = list[i];
            if(CheckEqual)
            {
                if(node.ID == nID)
                {
                    list.RemoveAt(i);
                    return node;
                }
            }
            else
            {
                list.RemoveAt(i);
                return node;
            }
        }

        return null;
    }

    public override void OnNPConnected(NetPoint np)
    {
        var message = XSFCore.GetMessage((ushort)XsfPbid.SMSGID.CCcServerInfo) as XsfMsg.MSG_C_Cc_ServerInfo;
        message.mPB.Infos.Clear();

        // 把当前已经收到的服务器信息下发给新加入的节点
        uint nID = np.ID;
        //Serilog.Log.Information("OnNPConnected nID=" + nID);
        foreach (KeyValuePair<uint, ServerInfo> kvp in m_Nodes)
        {
            if(kvp.Key != nID)
            {
                var node = kvp.Value;
                XsfPb.MSG_ServerInfo info = new XsfPb.MSG_ServerInfo();
                info.ServerId = node.ID;
                info.Ip = node.IP;
                for(int i = 0; i < node.Ports.Length; i ++)
                    info.Ports.Add(node.Ports[i]);
                info.Status = (uint)node.Status;
                message.mPB.Infos.Add(info);
            }
        }

        if(message.mPB.Infos.Count > 0)
            np.SendMessage(message);

        // 把新加入的节点信息，广播给其他所有服务器节点
        {
            message.mPB.Infos.Clear();

            ServerInfo nodeAdd = null;
            m_Nodes.TryGetValue(np.ID, out nodeAdd);
            XsfPb.MSG_ServerInfo info = new XsfPb.MSG_ServerInfo();
            info.ServerId = nodeAdd.ID;
            info.Ip = nodeAdd.IP;
            for(int i = 0; i < nodeAdd.Ports.Length; i ++)
                info.Ports.Add(nodeAdd.Ports[i]);
            info.Status = (uint)nodeAdd.Status;
            message.mPB.Infos.Add(info);
                    
            Broadcast(message, np.ID);
        }

        if(m_CurStartNode != null && (byte)m_CurStartNode.ep == np.SID.Type)
        {
            m_nStep = RunStep.HandshakeDone;
        }
    }

    public override void OnNPLost(NetPoint np)
    {
        ServerInfo node = null;
        if(m_Nodes.TryGetValue(np.ID, out node))
        {
            m_Nodes.Remove(np.ID);
            m_LostList[np.SID.Type].Add(node);
        }
        else
        {
            Serilog.Log.Information("NodeManager OnNPLost, node not exist, id={0}", np.ID);
            return;
        }

        var message = XSFCore.GetMessage((ushort)XsfPbid.SMSGID.CCcServerLost) as XsfMsg.MSG_C_Cc_ServerLost;
        message.mPB.ServerId = np.ID;
        Serilog.Log.Information("【中心服】有服务器节点离线, id={0}", np.ID);

        Broadcast(message, 0);
    }

    public void OnNodeOk(uint nID)
    {
        ServerInfo node = null;
        if(m_Nodes.TryGetValue(nID, out node))
        {
            node.Status = NodeStatus.Ok;
        }
        else
        {
            Serilog.Log.Information("NodeManager OnNodeOk, node not exist, id={0}", nID);
            return;
        }

        Serilog.Log.Information("【中心服】收到服务器已准备好 id={0}", nID);

        var message = XSFCore.GetMessage((ushort)XsfPbid.SMSGID.CCcServerOk) as XsfMsg.MSG_C_Cc_ServerOk;
        message.mPB.ServerId = nID;

        Broadcast(message, nID);
    }




    public static void CreateModule()
    {
        NodeManager module = new NodeManager();

        NPManagerInit init = new NPManagerInit();
        init.ID = (int)ModuleID.Node;
        init.Name = "NodeManager";
        init.NoWaitStart = true;
        init.Port = (int)XSFCore.Config.CenterPort;

        XSFCore.Server.AddModule(module, init);
    }

    public static NodeManager Instance
    {
        get
        {
            return XSFCore.Server.GetModule((int)ModuleID.Node) as NodeManager;
        }
    }
}