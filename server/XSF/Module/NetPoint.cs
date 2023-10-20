//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/XSF/Module/NetPoint.cs
// 作者：Xoen Xie
// 时间：2023/07/01
// 描述：网络节点
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8602, CS8618, CS8604

namespace XSF
{
    public class NetPoint : ITimerHandler, INetHandler
    {
        enum TimerID
        {
            HTCheck = 0,
            Max,
        }

        public ServerID SID { get { return m_SID; } }
        public uint ID { get { return m_nID; } }
        protected uint m_nID;
        protected ServerID m_SID;

        protected IConnection? m_Connection;
        protected uint m_nLastHTTime;

        public NPManager Owner { get; private set;}

        private TimersManager m_Timers;

        public bool Create(NPManager owner, IConnection connection)
        {
            Owner = owner;
            m_Connection = connection;
            m_nLastHTTime = XSFUtil.CurrentS;

            m_Timers = new TimersManager((int)TimerID.Max);

            m_Timers.StartTimer((byte)TimerID.HTCheck, this, XSFServer.Instance.Config.HeartbeatInterval, -1, "NetPoint.Create");

            return true;
        }

        public void SetID(uint nID)
        {
            m_nID = nID;
            m_SID = ServerID.GetSID(m_nID);
        }

        public void Release()
        {
            if(m_Connection != null)
            {
                m_Connection.Close();
            }

            m_Timers.CloseAllTimer();
        }


        public void OnTimer(byte nTimerID, bool bLastCall)
        {

        }

        public void UpdateHTTime()
        {
            m_nLastHTTime = XSFUtil.CurrentS;
        }

        public string RemoteIP
        {
            get
            {
                return m_Connection.RemoteIP;
            }
        }

        public void SendMessage(IMessage message)
        {
            if (m_Connection == null)
            {
                Serilog.Log.Error($"NetPoint.SendMessage connection not exist, msg id={message.ID}");
                return;
            }

            m_Connection.SendMessage(message);
        }

        public void SendData(ushort nMessageID, byte[] data)
        {
            if (m_Connection == null)
            {
                Serilog.Log.Error($"NetPoint.SendMessage connection not exist, msg id={nMessageID}");
                return;
            }

            m_Connection.SendData(data);
        }


        public void OnConnected(IConnection connection)
        {
            Serilog.Log.Error("NetPoint.OnConnected error call, name={0}", Owner.Name);
        }

        public void OnRecv(IConnection connection, IMessage message, ushort nMessageID, uint nRawID, byte[]? data)
        {
            message.Execute(this, nMessageID, nRawID, data);
        }

        public void OnError(IConnection connection, NetError nErrorCode)
        {
            Serilog.Log.Error("NetPoint.OnError, name={0}, id=[{1}|{2}-{3}-{4}]", Owner.Name, ID, SID.ID, XSFUtil.EP2CNName(SID.Type), SID.Index);
            Owner.OnNPError(this);
            Owner.Delete(this);
        }

        public void OnAccept(IConnection connection)
        {
            Serilog.Log.Error("NetPoint.OnAccept error call, name={0}", Owner.Name);
        }
    }
}

