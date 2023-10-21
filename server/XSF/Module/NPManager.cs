//////////////////////////////////////////////////////////////////////////
//
// 文件：server/XSF/Module/NPManager.cs
// 作者：Xoen Xie
// 时间：2023/08/18
// 描述：网点管理器
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8602, CS8603, CS8600, CS8618, CS8625

namespace XSF
{
    public class NPManagerInit : ModuleInit
    {
        public int Port;
    }

    public abstract class NPManager : IModule, INetHandler
    {
        protected int m_nPort;

        public virtual int Port { get { return m_nPort; } }

        private List<NetPoint> m_WaitList;

        private IConnection? m_Connection;

        public NPManager()
        {
            m_WaitList = new List<NetPoint>();
        }
        
        public override bool Init(ModuleInit init)
        {
            base.Init(init);

            var localInit = init as NPManagerInit;
            m_nPort = localInit.Port;

            return true;
        }

        public override void Release()
        {
            if(m_Connection != null)
                m_Connection.Close();
        }

        public override bool Start()
        {
            m_Connection = XSFNet.Instance.Listen(XSFUtil.ServerPakcer, this, Port);
            if(m_Connection == null)
                return false;

            return true;
        }

        public override ModuleRunCode OnCloseCheck()
        {
            if(Total > 0)
            {
                return ModuleRunCode.Wait;
            }

            return ModuleRunCode.OK;
        }

        protected bool DeleteFromWaitList(NetPoint np)
        {
            return m_WaitList.Remove(np);
        }

        protected virtual NetPoint NewNP()
        {
            return new NetPoint();
        }

        public abstract bool Add(NetPoint np);
        public abstract void Delete(NetPoint np);
        public abstract uint Total { get; }
        public abstract NetPoint Get(uint nID);

        public abstract void Broadcast(IMessage message, uint nSkipID);

        public virtual void OnNPLost(NetPoint np) {}
        public virtual void OnNPConnected(NetPoint np) {}
        public virtual bool CanConnected { get { return true;} }


        public void OnConnected(IConnection connection) 
        {
            Serilog.Log.Error("NPManager OnConnected error call, name={0}", Name);
        }

        public void OnRecv(IConnection connection, IMessage message, ushort nMessageID, uint nRawID, byte[]? data) 
        {
            Serilog.Log.Error("NPManager OnRecv error call, name={0}", Name);
        }

        public void OnError(IConnection connection, NetError nErrorCode) 
        {
            Serilog.Log.Error("NPManager OnError error call, name={0}", Name);
        }

        public void OnAccept(IConnection connection) 
        {
            if(!CanConnected)
            {
                connection.Close();
                return;
            }

            NetPoint np = NewNP();
            np.Create(this, connection);
            connection.DoStart(np);

            m_WaitList.Add(np);
        }
    }

    public class DicNPManager : NPManager
    {
        Dictionary<uint, NetPoint> m_NetPoints;

        public DicNPManager()
        {
            m_NetPoints = new Dictionary<uint, NetPoint>();
        }

        public override bool Add(NetPoint np)
        {
            if(!DeleteFromWaitList(np))
            {
                Serilog.Log.Error("DicNPManager.Add name={0}, np not int wait list, id=[{1},{2}-{3}-{4}", Name, np.ID, np.SID.ID, np.SID.Type, np.SID.Index);
                np.Release();
                return false;
            }
            
            if(m_NetPoints.ContainsKey(np.ID))
            {
                Serilog.Log.Error("DicNPManager.Add name={0}, np exist, id=[{1},{2}-{3}-{4}", Name, np.ID, np.SID.ID, np.SID.Type, np.SID.Index);
                np.Release();
                return false;
            }

            m_NetPoints.Add(np.ID, np);

            OnNPConnected(np);

            Serilog.Log.Information("DicNPManager.Add name={0}, NetPoint login, id=[{1},{2}-{3}-{4}", Name, np.ID, np.SID.ID, XSFUtil.EP2CNName((byte)np.SID.Type), np.SID.Index);

            return true;
        }
        public override void Delete(NetPoint np)
        {
            if(np.ID == 0)
            {
                DeleteFromWaitList(np);
            }
            else
            {
                m_NetPoints.Remove(np.ID);

                OnNPLost(np);
            }

            Serilog.Log.Information("DicNPManager.Delete name={0}, NetPoint logout, id=[{1},{2}-{3}-{4}", Name, np.ID, np.SID.ID, XSFUtil.EP2CNName((byte)np.SID.Type), np.SID.Index);
        }
        public override uint Total { get { return (uint)m_NetPoints.Count; } }

        public override NetPoint Get(uint nID)
        {
            NetPoint np = null;
            if(m_NetPoints.TryGetValue(nID, out np))
            {
                return np;
            }

            return null;
        }

        public override void Broadcast(IMessage message, uint nSkipID)
        {
            foreach(KeyValuePair<uint, NetPoint> kv in m_NetPoints)
            {
                if(kv.Value.ID != nSkipID)
                {
                    kv.Value.SendMessage(message);
                }
            }
        }
    }

    public class FastNPManagerInit : NPManagerInit
    {
        public int MaxSize;
    }

    public class FastNPManager : NPManager
    {
        private NetPoint[] m_NetPoints;

        private uint m_nTotal;

        public override bool Init(ModuleInit init)
        {
            base.Init(init);
            var localInit = init as FastNPManagerInit;
            m_NetPoints = new NetPoint[localInit.MaxSize];

            return true;
        }

        public override bool Add(NetPoint np)
        {
            if(!DeleteFromWaitList(np))
            {
                Serilog.Log.Error("FastNPManager.Add name={0}, np not int wait list, id=[{1},{2}-{3}-{4}", Name, np.ID, np.SID.ID, np.SID.Type, np.SID.Index);
                np.Release();
                return false;
            }

            if(m_nTotal >= m_NetPoints.Length)
            {
                Serilog.Log.Error("FastNPManager.Add name={0}, is full, id=[{1},{2}-{3}-{4}", Name, np.ID, np.SID.ID, np.SID.Type, np.SID.Index);
                np.Release();
                return false;
            }
            
            if(np.SID.Index >= m_NetPoints.Length)
            {
                Serilog.Log.Error("FastNPManager.Add name={0}, index too big, id=[{1},{2}-{3}-{4}", Name, np.ID, np.SID.ID, np.SID.Type, np.SID.Index);
                np.Release();
                return false;
            }

            if(m_NetPoints[np.SID.Index] != null)
            {
                Serilog.Log.Error("FastNPManager.Add name={0}, np exist, id=[{1},{2}-{3}-{4}", Name, np.ID, np.SID.ID, np.SID.Type, np.SID.Index);
                np.Release();
                return false;
            }

            m_NetPoints[np.SID.Index] = np;
            m_nTotal ++;

            OnNPConnected(np);

            Serilog.Log.Information("FastNPManager.Add name={0}, NetPoint login, id=[{1},{2}-{3}-{4}", Name, np.ID, np.SID.ID, XSFUtil.EP2CNName((byte)np.SID.Type), np.SID.Index);

            return true;
        }
        public override void Delete(NetPoint np)
        {
            if(np.ID == 0)
            {
                DeleteFromWaitList(np);
            }
            else
            {
                int nIndex = np.SID.Index;
                if(nIndex >= m_NetPoints.Length)
                {
                    return;
                }

                if(m_NetPoints[nIndex] != np)
                {
                    return;
                }

                m_NetPoints[nIndex] = null;
                m_nTotal --;

                OnNPLost(np);
            }

            Serilog.Log.Information("FastNPManager.Delete name={0}, NetPoint logout, id=[{1},{2}-{3}-{4}", Name, np.ID, np.SID.ID, XSFUtil.EP2CNName((byte)np.SID.Type), np.SID.Index);
        }
        public override uint Total { get { return m_nTotal; } }

        public override NetPoint Get(uint nID)
        {
            if(m_nTotal <= 0)
                return null;

            ServerID SID = ServerID.GetSID(nID);
            int nIndex = SID.Index;
            if(nIndex >= m_NetPoints.Length)
            {
                return null;
            }

            return m_NetPoints[nIndex];
        }

        public override void Broadcast(IMessage message, uint nSkipID)
        {
            for(int i = 0; i < m_NetPoints.Length; i++)
            {
                if(m_NetPoints[i] != null && m_NetPoints[i].ID != nSkipID)
                {
                    m_NetPoints[i].SendMessage(message);
                }
            }
        }   
    }
}