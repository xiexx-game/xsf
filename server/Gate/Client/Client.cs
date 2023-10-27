//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/Gate/Client/Client.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：客户端连接管理
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8602, CS8618, CS8604, CS8603
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
            m_nLastHTTime = XSFCore.CurrentS;

            m_Timers = new TimersManager((int)TimerID.Max);

            m_Timers.StartTimer((byte)TimerID.HTCheck, this, XSFCore.Config.ClientHeartbeatCheck, -1, "Client.Create");

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

        public void Disconnect(int nReason)
        {
            var message = XSFCore.GetMessage((ushort)XsfPb.CMSGID.GtCltDisconnect) as XsfMsg.MSG_Gt_Clt_Disconnect;
            message.mPB.Reason = nReason;
            SendMessage(message);

            m_Timers.StartTimer((byte)TimerID.Disconnect, this, 300, 1, "Client.Disconnect");
        }

        public void OnHandshake()
        {
            m_IsHandshake = true;
        }

        public ServerConnector GetConnector(byte nEP)
        {
            if(m_ConnectorIDs[(int)nEP] > 0)
            {
                return ConnectorManager.Instance.GetConnector((byte)nEP, m_ConnectorIDs[(int)nEP]);
            }
            else
            {
                var connector = ConnectorManager.Instance.GetConnector((byte)nEP, 0);
                if(connector != null)
                {
                    m_ConnectorIDs[(int)nEP] = connector.ConnectorID;
                }

                return connector;
            }
        }

        public void SendMessage(IMessage message)
        {
            if(m_Connection != null)
                m_Connection.SendMessage(message);
        }

        public void SendData(byte [] data)
        {
            if(m_Connection != null)
                m_Connection.SendData(data);
        }

        public void SetServerID(byte nEP, uint nServerID)
        {
            m_ConnectorIDs[nEP] = nServerID;
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
            Serilog.Log.Error("Client.OnError, id=[{0}]", ID);
            m_Owner.Delete(this);
            Close();

            var message = XSFCore.GetMessage((ushort)XsfPb.SMSGID.GtGtAClientClose) as XsfMsg.MSG_Gt_GtA_ClientClose;
            message.mPB.ClientId = ID;
            for(int i = 0; i < m_ConnectorIDs.Length; i ++)
            {
                if(m_ConnectorIDs[i] > 0)
                {
                    var connector = ConnectorManager.Instance.GetConnector((byte)i, m_ConnectorIDs[i]);
                    if(connector != null)
                        connector.SendMessage(message);
                }
            }
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
                    var innerMsg = XSFCore.GetMessage(nMessageID);
                    var connector = GetConnector(innerMsg.DestEP);
                    if(connector == null)
                    {
                        Disconnect((int)XsfPb.DisconnectReason.MsgInvalid);
                        return;
                    }

                    var clientIDData = BitConverter.GetBytes(ID);
                    Array.Copy(clientIDData, 0, data, 6, clientIDData.Length);
                    
                    connector.SendData(nMessageID, data);
                }
                else
                {
                    Disconnect((int)XsfPb.DisconnectReason.MsgInvalid);
                }
            }
        }

        public void OnTimer(byte nTimerID, bool bLastCall)
        {
            switch(nTimerID)
            {
            case (byte)TimerID.HTCheck:
                {
                    var current = XSFCore.CurrentS;
                    if(current > m_nLastHTTime + XSFCore.Config.ClientHeartbeatTimeout)
                    {
                        Serilog.Log.Error($"Client OnTimer heartbeat timeout ... {current} > {m_nLastHTTime} + {XSFCore.Config.ClientHeartbeatTimeout}");
                        Close();
                    }
                } 
                break;

            case (byte)TimerID.Disconnect:
                {
                    Close();
                }
                break;
            }
            
        }
    }
}