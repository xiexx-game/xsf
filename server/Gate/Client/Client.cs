//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/Gate/Client/Client.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：客户端连接管理
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8602, CS8618, CS8604
using XSF;

namespace GateClient
{
    public class Client : INetHandler, ITimerHandler
    {
        enum TimerID
        {
            HTCheck = 0,
            Disconnect,
            Max,
        }

        public ClientID CID { get { return m_CID; } }
        public uint ID { get { return m_nID; } }
        internal uint m_nID;
        internal ClientID m_CID;

        internal IConnection? m_Connection;

        private uint m_nLastHTTime;

        private ClientManager m_Owner;

        private TimersManager m_Timers;

        private bool m_IsHandshake;

        private uint[] m_ConnectorIDs;

        public Client()
        {
            m_ConnectorIDs = new uint[(int)EP.Max];
        }

        public bool Create(ClientManager owner, IConnection connection)
        {
            m_Owner = owner;
            m_Connection = connection;
            m_nLastHTTime = XSFUtil.CurrentS;

            m_Timers = new TimersManager((int)TimerID.Max);

            m_Timers.StartTimer((byte)TimerID.HTCheck, this, XSFUtil.Config.ClientHeartbeatCheck, -1, "NetPoint.Create");

            return true;
        }

        public void Close()
        {
            if(m_Connection != null)
            {
                m_Connection.Close();
                m_Connection = null;
            }

            m_Timers.CloseAllTimer();
        }

        public void Disconnect()
        {

        }

        public void OnHandshake()
        {
            m_IsHandshake = true;
        }

        public ServerConnector GetConnector(EP nEP)
        {
            if(m_ConnectorIDs[(int)nEP] > 0)
            {
                return ConnectorManager.Instance.GetConnector(m_ConnectorIDs[(int)nEP]);
            }
            else
            {
                
            }
        }

        public void SendMessage(IMessage message)
        {
            if(m_Connection != null)
                m_Connection.SendMessage(message);
        }

        public void OnAccept(IConnection connection) 
        {
            Serilog.Log.Error("Client.OnAccept error call");
        }

        public void OnConnected(IConnection connection) 
        {
            Serilog.Log.Error("Client.OnConnected error call");
        }
        
        public void OnError(IConnection connection, NetError nErrorCode) 
        {
            Serilog.Log.Error("Client.OnError, id=[{1}]", ID);
            m_Owner.Delete(this);
            Close();
        }
        
        public void OnRecv(IConnection connection, IMessage message, ushort nMessageID, uint nRawID, byte[]? data) 
        {
            if(message != null)
                message.Execute(this, nMessageID, nRawID, data);
            else
            {
                if(m_IsHandshake)
                {
                    Serilog.Log.Information("client recv message id=" + nMessageID);
                }
                else
                {
                    Close();
                }
            }
        }

        public void OnTimer(byte nTimerID, bool bLastCall)
        {
            switch(nTimerID)
            {
            case (byte)TimerID.HTCheck:
                {
                    var current = XSFUtil.CurrentS;
                    if(current > m_nLastHTTime + XSFUtil.Config.ClientHeartbeatTimeout)
                    {
                        Serilog.Log.Error($"Client OnTimer heartbeat timeout ... {current} > {m_nLastHTTime} + {XSFUtil.Config.ClientHeartbeatTimeout}");
                        Close();
                    }
                } 
                break;

            case (byte)TimerID.Disconnect:
                {

                }
                break;
            }
            
        }
    }
}