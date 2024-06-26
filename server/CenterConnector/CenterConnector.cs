//////////////////////////////////////////////////////////////////////////
//
// 文件：server/CenterConnector/CenterConnector.cs
// 作者：Xoen Xie
// 时间：2023/06/15
// 描述：中心服连接器
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8602, CS8600, CS8618

using XSF;
using Google.Protobuf.Collections;

namespace CC
{
    internal class CenterConnector : ICenterConnector
    {
        private Dictionary<uint, ServerInfo> m_ServerInfos;

        internal IServerInfoHandler? m_Handler;

        public CenterConnector()
        {
            m_ServerInfos = new Dictionary<uint, ServerInfo>();
        }

        public override bool Start()
        {
            var config = XSFCore.Config;

            return Connect(config.MainCenterIP, (int)config.CenterPort);
        }

        public override void DoRegist()
        {
            XSFCore.SetMessageExecutor((ushort)XsfPbid.SMSGID.CCcHandshake, new Executor_C_Cc_Handshake());
            XSFCore.SetMessageExecutor((ushort)XsfPbid.SMSGID.CCcServerInfo, new Executor_C_Cc_ServerInfo());
            XSFCore.SetMessageExecutor((ushort)XsfPbid.SMSGID.CCcServerLost, new Executor_C_Cc_ServerLost());
            XSFCore.SetMessageExecutor((ushort)XsfPbid.SMSGID.CCcServerOk, new Executor_C_Cc_ServerOk());
            XSFCore.SetMessageExecutor((ushort)XsfPbid.SMSGID.CCcStop, new Executor_C_Cc_Stop());
        }

        public override ModuleRunCode OnStartCheck()
        {
            if(IsHandshake)
            {
                return ModuleRunCode.OK;
            }

            return ModuleRunCode.Wait;
        }

        public override void OnOK()
        {
            var message = XSFCore.GetMessage((ushort)XsfPbid.SMSGID.CcCServerOk) as XsfMsg.MSG_Cc_C_ServerOk;
            message.mPB.ServerId = XSFCore.Server.ID;
            SendMessage(message);
        }

        public override void SendHandshake()
        {
            Serilog.Log.Information("CenterConnector SendHandshake");
            var message = XSFCore.GetMessage((ushort)XsfPbid.SMSGID.CcCHandshake) as XsfMsg.MSG_Cc_C_Handshake;
            message.mPB.ServerId = XSFCore.Server.ID;

            message.mPB.Ports.Clear();
            for(int i = 0; i < XSFCore.Server.Ports.Length; i ++)
            {
                message.mPB.Ports.Add(XSFCore.Server.Ports[i]);
            }

            SendMessage(message);
        }

        public override void SendHeartbeat()
        {
            //Serilog.Log.Information("CenterConnector SendHeartbeat");
            var message = XSFCore.GetMessage((ushort)XsfPbid.SMSGID.CcCHeartbeat);
            SendMessage(message);
        }

        public void AddInfo(XsfMsg.MSG_C_Cc_ServerInfo message)
        {
            for(int i = 0; i < message.mPB.Infos.Count; i ++)
            {
                bool IsNewAdd = false;
                var pbInfo = message.mPB.Infos[i];
                ServerInfo info = null;
                if(m_ServerInfos.TryGetValue(pbInfo.ServerId, out info))
                {
                    info.IP = pbInfo.Ip;
                    for(int J = 0; J < pbInfo.Ports.Count; J ++)
                    {
                        info.Ports[J] = pbInfo.Ports[J];
                    }
                    info.Status = (NodeStatus)pbInfo.Status;
                } 
                else
                {
                    IsNewAdd = true;
                    info = new ServerInfo();
                    info.ID = pbInfo.ServerId;
                    info.IP = pbInfo.Ip;
                    for(int J = 0; J < pbInfo.Ports.Count; J ++)
                    {
                        info.Ports[J] = pbInfo.Ports[J];
                    }
                    info.Status = (NodeStatus)pbInfo.Status;

                    m_ServerInfos.Add(info.ID, info);
                }

                var sid = ServerID.GetSID(info.ID);
                Serilog.Log.Information("【中心服连接器】收到服务器信息, id={0} [{1}-{2}-{3}] status={4}", info.ID, sid.ID, sid.Index, XSFCore.EP2CNName(sid.Type), info.Status);

                if(IsNewAdd)
                {
                    Serilog.Log.Information("【中心服连接器】新增服务器节点, id={0} [{1}-{2}-{3}] status={4}", info.ID, sid.ID, sid.Index, XSFCore.EP2CNName(sid.Type), info.Status);
                    if(m_Handler != null)
                        m_Handler.OnServerNew(info);

                    if(info.Status == NodeStatus.Ok)
                    {
                        Serilog.Log.Information("【中心服连接器】收到服务器节点已准备好, id={0} [{1}-{2}-{3}]", info.ID, sid.ID, sid.Index, XSFCore.EP2CNName(sid.Type) );
                        if(m_Handler != null)
                            m_Handler.OnServerOk(info);
                    }
                }
            }
        }

        public void OnNodeLost(uint nID)
        {
            m_ServerInfos.Remove(nID);
            var sid = ServerID.GetSID(nID);

            Serilog.Log.Information("【中心服连接器】有服务器节点离线, id={0} [{1}-{2}-{3}]", nID, sid.ID, sid.Index, XSFCore.EP2CNName(sid.Type));
            if(m_Handler != null)
                m_Handler.OnServerLost(nID);
        }

        public void OnNodeOk(uint nID)
        {
            ServerInfo info = null;
            if(m_ServerInfos.TryGetValue(nID, out info))
            {
                info.Status = NodeStatus.Ok;
                var sid = ServerID.GetSID(nID);

                Serilog.Log.Information("【中心服连接器】收到服务器节点已准备好, id={0} [{1}-{2}-{3}]", nID, sid.ID, sid.Index, XSFCore.EP2CNName(sid.Type) );

                if(m_Handler != null)
                    m_Handler.OnServerOk(info);
            }
            else
            {
                Serilog.Log.Error("服务器已准备好，但是本地未找到服务器信息, id={0}", nID);
            }
        }
    }
}