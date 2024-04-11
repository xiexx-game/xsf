//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/XSF/Module/NetConnector.cs
// 作者：Xoen Xie
// 时间：2023/07/01
// 描述：网络连接器
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8602, CS8604

namespace XSF
{
    public class NetConnectorInit : ModuleInit
    {
        public bool NeedReconnect;
    }

    public abstract class NetConnector : IModule, INetHandler, ITimerHandler
    {
        enum TimerID
        {
            HT = 0,
            Reconnect,

            Max,
        }

        private string m_sIP = "";
        private int m_nPort;

        private TimersManager m_Timers;

        private IConnection? m_Connection;

        private bool m_bNeedReconnect;

        public ServerID SID { get { return m_SID; } }
        public uint ConnectorID { get { return m_nID; } }
        protected uint m_nID;
        protected ServerID m_SID;

        public bool IsHandshake { get; private set; }

        public NetConnector()
        {
            m_Timers = new TimersManager((int)TimerID.Max);
            IsHandshake = false;
        }

        public override bool Init(ModuleInit init)
        {
            base.Init(init);

            var localInit = init as NetConnectorInit;
            m_bNeedReconnect = localInit.NeedReconnect;
            
            return true;
        }

        public void SetID(uint nID)
        {
            m_nID = nID;
            m_SID = ServerID.GetSID(m_nID);
        }

        public override bool Start()
        {
            return true;
        }

        public override void DoClose()
        {
            m_Timers.CloseAllTimer();

            m_bNeedReconnect = false;
            if(m_Connection != null)
            {
                m_Connection.Close();
                m_Connection = null;
            }
        }

        public override void Release()
        {
            
        }

        public bool Connect(string ip, int port)
        {
            m_sIP = ip;
            m_nPort = port;

            return DoConnect();
        }

        public void SetConnectInfo(string ip, int port)
        {
            m_sIP = ip;
            m_nPort = port;
        }

        public bool DoConnect()
        {
            m_Connection = XSFNet.Instance.Connect(this, m_sIP, m_nPort);
            if(m_Connection == null)
            {
                return false;
            }

            return true;
        }



        public void OnConnected(IConnection connection)
        {
            m_Timers.DelTimer((byte)TimerID.Reconnect);

            SendHandshake();
        }

        public void OnHandshake()
        {
            m_Timers.StartTimer((byte)TimerID.HT, this, XSFServer.Instance.Config.HeartbeatInterval, -1, "NetConnector.OnHandshake");
            IsHandshake = true;

            OnHandshakeOk();
        }

        public void SendMessage(IMessage message)
        {
            if (m_Connection == null)
            {
                Serilog.Log.Error($"NetConnector.SendMessage connection not exist, msg id={message.ID}");
                return;
            }

            m_Connection.SendMessage(message);
        }

        public void SendData(ushort nMessageID, byte[] data)
        {
            if (m_Connection == null)
            {
                Serilog.Log.Error($"NetConnector.SendMessage connection not exist, msg id={nMessageID}");
                return;
            }

            m_Connection.SendData(data);
        }

        public void OnRecv(IConnection connection, IMessage message, ushort nMessageID, uint nRawID, byte[]? data)
        {
            message.Execute(this, nMessageID, nRawID, data);
        }

        public void OnError(IConnection connection, NetError nErrorCode)
        {
            Serilog.Log.Error("NetConnector OnError, name={0}, code={1}", Name, nErrorCode);
            m_Timers.DelTimer((byte)TimerID.HT);
            IsHandshake = false;

            if(m_bNeedReconnect)
            {
                m_Timers.StartTimer((byte)TimerID.Reconnect, this, XSFServer.Instance.Config.ReconnectInterval, -1, "NetConnector.OnError");
            }

            OnNetError();
        }

        public void OnAccept(IConnection connection)
        {
            Serilog.Log.Error("NetConnector OnAccept error call ..., name={0}", Name);
        }

        public void OnTimer(byte nTimerID, bool bLastCall)
        {
            switch((TimerID)nTimerID)
            {
            case TimerID.HT:
                SendHeartbeat();
                break;

            case TimerID.Reconnect:
                if(DoConnect())
                {
                    m_Timers.DelTimer((byte)TimerID.Reconnect);
                }
                break;
            }
        }

        public abstract void SendHandshake();
        public abstract void SendHeartbeat();

        public virtual void OnHandshakeOk() {}
        public virtual void OnNetError() {}
    }
}