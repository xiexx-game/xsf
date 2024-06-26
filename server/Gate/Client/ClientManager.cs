//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/Gate/Client/ClientManager.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：客户端连接管理
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8625, CS8618, CS8603

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
            m_Clients = new Client[XSFCore.Config.GateMaxCount];
            m_ClientPakcer = new LocalPacker();
        }

        public override void DoRegist()
        {
            XSFCore.SetMessageExecutor((ushort)XsfPbid.CMSGID.CltGtHandshake, new Executor_Clt_Gt_Handshake());
            XSFCore.SetMessageExecutor((ushort)XsfPbid.CMSGID.CltGtHeartbeat, new Executor_Clt_Gt_Heartbeat());
        }

        public override bool Start()
        {
            m_Connection = XSFCore.Listen(m_ClientPakcer, this, (int)XSFCore.Server.Ports[(int)EP.Client]);
            if(m_Connection == null)
                return false;

            return true;
        }

        public override void OnStartClose()
        {
            for(int i = 0; i < m_Clients.Length; i ++)
            {
                if(m_Clients[i] != null && m_Clients[i].ID > 0)
                    m_Clients[i].Disconnect((int)XsfPb.DisconnectReason.ServerDown);
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
                m_Clients[nIndex].Release();
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
            if(m_nTotal >= XSFCore.Config.GateMaxCount)
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
                    client.m_CID.Gate = XSFCore.Server.SID.Index;
                    client.m_CID.ID = (ushort)i;
                    client.m_CID.Key = m_GlobalKey ++;
                    client.m_nID = ClientID.GetID(client.m_CID);

                    Serilog.Log.Information("clientManager OnAccept new ... id=" + client.ID);

                    connection.DoStart(client);

                    m_Clients[i] = client;

                    m_nTotal ++;

                    return;
                }
                else
                {
                    if(m_Clients[i].ID <= 0)
                    {
                        Client client = m_Clients[i];
                        client.Create(this, connection);
                        client.m_CID.Gate = XSFCore.Server.SID.Index;
                        client.m_CID.ID = (ushort)i;
                        client.m_CID.Key = m_GlobalKey ++;
                        client.m_nID = ClientID.GetID(client.m_CID);
                        connection.DoStart(client);
                        m_nTotal ++;

                        Serilog.Log.Information("clientManager OnAccept get old ... id=" + client.ID);

                        return;
                    }
                }
            }

            connection.Close();
        }

        public Client GetClient(uint nClientID)
        {
            var cid = ClientID.GetCID(nClientID);
            if(cid.ID >= m_Clients.Length)
            {
                Serilog.Log.Error($"ClientManager.GetClient cid.ID:{cid.ID} >= m_Clients.Length:{m_Clients.Length}");
                return null;
            }

            if(m_Clients[cid.ID] != null && m_Clients[cid.ID].ID == nClientID && m_Clients[cid.ID].IsHandshake)
            {
                return m_Clients[cid.ID];
            }

            return null;
        }

        public void Broadcast(byte[] data)
        {
            for(int i = 0; i < m_Clients.Length; i ++)
            {
                if(m_Clients[i] != null && m_Clients[i].IsHandshake)
                {
                    m_Clients[i].SendData(data);
                }
            }
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

        public static ClientManager Instance
        {
            get; private set;
        }

        public static void CreateModule()
        {
            Instance = new ClientManager();

            ModuleInit init = new ModuleInit();
            init.ID = (int)ModuleID.Client;
            init.Name = "ClientManager";

            XSFCore.Server.AddModule(Instance, init);
        }
    }
}
