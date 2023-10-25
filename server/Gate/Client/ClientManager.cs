//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/Gate/Client/ClientManager.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：客户端连接管理
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8625

using XSF;

namespace GateClient
{
    public class ClientManager : IModule, INetHandler
    {
        private IConnection? m_Connection;

        private Client[] m_Clients;

        private int m_nTotal;

        private byte m_GlobalKey = 0;

        private INetPacker m_ClientPakcer;

        public ClientManager()
        {
            m_Clients = new Client[XSFUtil.Config.GateMaxCount];
            m_ClientPakcer = new ClientPakcer();
        }

        public override void DoRegist()
        {
            XSFUtil.SetMessageExecutor((ushort)XsfPb.CMSGID.CltGtHandshake, new Executor_Clt_Gt_Handshake());
            XSFUtil.SetMessageExecutor((ushort)XsfPb.CMSGID.CltGtHeartbeat, new Executor_Clt_Gt_Heartbeat());
        }

        public override bool Start()
        {
            m_Connection = XSFUtil.Listen(m_ClientPakcer, this, (int)XSFUtil.Server.Ports[(int)EP.Client]);
            if(m_Connection == null)
                return false;

            return true;
        }

        public override void OnClose()
        {
            for(int i = 0; i < m_Clients.Length; i ++)
            {
                if(m_Clients[i] != null)
                    m_Clients[i].Disconnect();
            }
        }

        public override ModuleRunCode OnCloseCheck()
        {
            if(m_nTotal > 0)
                return ModuleRunCode.Wait;

            return ModuleRunCode.OK;
        }

        internal void Delete(Client c)
        {
            int nIndex = c.m_CID.ID;
            if(nIndex >= m_Clients.Length)
            {
                Serilog.Log.Error($"clientManager.Delete error, nIndex:{nIndex} >= m_Clients.Length:{m_Clients.Length}, client id={c.ID} [{c.CID.Gate}-{c.CID.ID}-{c.CID.Key}]");
                return;
            }

            if(c == m_Clients[nIndex])
            {
                m_Clients[nIndex] = null;
                m_nTotal --;
            }
            else
            {
                Serilog.Log.Error($"clientManager.Delete error, c {c.ID} == m_Clients[nIndex] {m_Clients[nIndex].ID}");
            }
        }

        public void OnAccept(IConnection connection) 
        {
            Serilog.Log.Information("clientManager OnAccept ...");
            if(m_nTotal >= XSFUtil.Config.GateMaxCount)
            {
                connection.Close();
                return;
            }

            for(int i = 0; i < m_Clients.Length; i ++)
            {
                if(m_Clients[i] == null)
                {
                    Client client = new Client();
                    client.Create(this, connection);
                    client.m_CID.Gate = XSFUtil.Server.SID.Index;
                    client.m_CID.ID = (ushort)i;
                    client.m_CID.Key = m_GlobalKey ++;
                    client.m_nID = ClientID.GetID(client.m_CID);

                    Serilog.Log.Information("clientManager OnAccept ... id=" + client.m_nID);


                    connection.DoStart(client);

                    m_Clients[i] = client;

                    m_nTotal ++;

                    return;
                }
            }

            connection.Close();
        }

        public void OnConnected(IConnection connection) 
        {
            Serilog.Log.Error("ClientManager.OnConnected error call");
        }
        
        public void OnError(IConnection connection, NetError nErrorCode) 
        {
            Serilog.Log.Error("ClientManager.OnError error call");
        }
        
        public void OnRecv(IConnection connection, IMessage message, ushort nMessageID, uint nRawID, byte[]? data) 
        {
            Serilog.Log.Error("ClientManager.OnRecv error call");
        }

        public static void CreateModule()
        {
            var module = new ClientManager();

            ModuleInit init = new ModuleInit();
            init.ID = (int)ModuleID.Client;
            init.Name = "ClientManager";

            XSFUtil.Server.AddModule(module, init);
        }
    }
}
