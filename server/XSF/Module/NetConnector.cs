//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/XSF/Module/NetConnector.cs
// 作者：Xoen Xie
// 时间：2023/07/01
// 描述：网络连接器
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8602

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

        private bool m_bIsHandshake;

        private bool m_bNeedReconnect;

        public NetConnector()
        {
            m_Timers = new TimersManager();
        }

        public override bool Init(ModuleInit init)
        {
            base.Init(init);

            var localInit = init as NetConnectorInit;
            m_bNeedReconnect = localInit.NeedReconnect;

            m_Timers.Init((int)TimerID.Max);
            
            return true;
        }

        public override bool Start()
        {
            return true;
        }

        public override void OnClose()
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
            m_bIsHandshake = true;

            OnHandshakeOk();
        }

        public void OnRecv(IConnection connection, byte[]? data)
        {

        }

        public void OnError(IConnection connection, NetError nErrorCode)
        {
            Serilog.Log.Error("NetConnector OnError, name={0}, code={1}", Name, nErrorCode);
            m_bIsHandshake = false;
            m_Timers.DelTimer((byte)TimerID.HT);

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
                if(Connect(m_sIP, m_nPort))
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