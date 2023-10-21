//////////////////////////////////////////////////////////////////////////
//
// 文件：server/XSF/Timer/XSFTimer.cs
// 作者：Xoen Xie
// 时间：2023/08/14
// 描述：定时器，skynet原版定时器代码修改而来
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8625, CS8618, CS8600, CS8602

namespace XSF
{
    public interface ITimerHandler 
    {
        void OnTimer(byte nTimerID, bool bLastCall);
    }

    public class TimersManager
    {
        private ulong [] m_TimerIDs;

        public TimersManager(int nMaxCount)
        {
            m_TimerIDs = new ulong[nMaxCount];
        }

        public void StartTimer(byte nTimerID, ITimerHandler handler, uint nInterval, int nTimes, string sDebugStr)
        {
            if(m_TimerIDs[nTimerID] != 0)
            {
                XSFTimer.Instance.Del(m_TimerIDs[nTimerID]);
            }

            m_TimerIDs[nTimerID] = XSFTimer.Instance.Add(nTimerID, handler, nInterval, nTimes, sDebugStr);
        }

        public void DelTimer(byte nTimerID)
        {
            XSFTimer.Instance.Del(m_TimerIDs[nTimerID]);
            m_TimerIDs[nTimerID] = 0;
        }

        public void CloseAllTimer()
        {
            for(int i = 0; i < m_TimerIDs.Length; i ++)
            {
                if(m_TimerIDs[i] > 0)
                {
                    XSFTimer.Instance.Del(m_TimerIDs[i]);
                    m_TimerIDs[i] = 0;
                }
            }
        }
    }

    internal class Timer 
    {
        public ulong nID = 0;
        public bool IsWorking = false;
        public byte nTimerID = 0;
        public ITimerHandler handler = null;
        public uint Interval = 0xFFFFFFFF;
        public int Times = 0;
        public uint Expire = 0;
        public string DebugStr = "";

        public Timer Next = null;
    }

    internal class TimerList
    {
        public Timer Head = null;
        public Timer Tail = null;

        public TimerList()
        {
            Head = new Timer();
        }

        public Timer Clear()
        {
            Timer ret = Head.Next;
            Head.Next = null;
            Tail = Head;

            return ret;
        }

        public void Link(Timer t)
        {
            Tail.Next = t;
            Tail = t;
            t.Next = null;
        }
    }

    internal class TimerEvent
    {
        public ulong nID;
        public ITimerHandler handler;

        public byte nTimerID;
        public bool bRemove;
    }

    internal class XSFTimer :Singleton<XSFTimer>
    {
        private Dictionary<ulong, Timer> m_Timers;

        private const int TIME_NEAR_SHIFT = 8;
        private const int TIME_NEAR = 1 << TIME_NEAR_SHIFT;
        private const int TIME_LEVEL_SHIFT = 6;
        private const int TIME_LEVEL = 1 << TIME_LEVEL_SHIFT;
        private const int TIME_NEAR_MASK = TIME_NEAR - 1;
        private const int TIME_LEVEL_MASK = TIME_LEVEL - 1;

        private TimerList[] m_Near;
        private TimerList[,] m_T;

        private TimerList m_TempList;

        private SpinLock m_SpinLock;

        private uint m_nTime;
        private ulong m_CurrentTime;

        private Thread m_Thread;

        private bool m_bIsRunning;

        private XSFQueue<TimerEvent> m_EventQueue;

        private uint m_IDIndex;

        public XSFTimer()
        {
            m_Timers = new Dictionary<ulong, Timer>();
            m_Near = new TimerList[TIME_NEAR];
            m_T = new TimerList[4,TIME_LEVEL];
            m_TempList = new TimerList();
            m_SpinLock = new SpinLock();
            m_Thread = new Thread(TimeThread);
            m_EventQueue = new XSFQueue<TimerEvent>();

            m_DeleteList = new List<ulong>();
        }

        public void Create()
        {
            m_CurrentTime = CurrentTime();
            for(int i = 0; i < TIME_NEAR; i ++)
            {
                m_Near[i] = new TimerList();
                m_Near[i].Clear();
            }

            for(int i = 0; i < 4; i ++)
            {
                for(int j = 0; j < TIME_LEVEL; j ++)
                {
                    m_T[i,j] = new TimerList();
                    m_T[i,j].Clear();
                }
            }

            m_TempList.Clear();

            
            m_Thread.Start();
        }

        public void Release()
        {
            m_bIsRunning = false;
            m_Thread.Join();

            Serilog.Log.Information("定时器线程退出");
        }

        public ulong Add(byte nTimerID, ITimerHandler handler, uint nInterval, int nTimes, string sDebugStr)
        {
            if(!m_bIsRunning || nInterval == 0)
                return 0;

            Timer nt = new Timer();
            nt.nTimerID = nTimerID;
            nt.handler = handler;
            nt.Interval = nInterval;
            nt.Times = nTimes;
            nt.IsWorking = true;
            nt.DebugStr = sDebugStr;

            if(nt.Times == 0)
                nt.Times = 1;

            var current = XSFUtil.CurrentS;
            m_IDIndex ++;
            nt.nID = XSFUtil.UINT64_ID(m_IDIndex, current);

            m_Timers.Add(nt.nID, nt);

            bool lockTaken = false;
            m_SpinLock.Enter(ref lockTaken);
            nt.Expire = m_nTime + nInterval;
            InnerAdd(nt);
            m_SpinLock.Exit();

            //Serilog.Log.Information("定时器创建完毕 id={0}, message={1}, nt.Expire={2}", nt.nID, sDebugStr, nt.Expire);

            return nt.nID;
        }

        public void Del(ulong nTimerID)
        {
            Timer t = null;
            if(m_Timers.TryGetValue(nTimerID, out t))
            {
                t.IsWorking = false;
            }
        }


        private void TimeThread()
        {
            Serilog.Log.Information("Timer Thread Create done");
            m_bIsRunning = true;

            while(m_bIsRunning)
            {
                var current = CurrentTime();

                if(current > m_CurrentTime)
                {
                    uint nDiff = (uint)(current - m_CurrentTime);
                    m_CurrentTime = current;

                    for(uint i = 0; i < nDiff; i ++)
                    {
                        TimerUpdate();
                    }
                }

                Thread.Sleep(2);
            }
        }

        private ulong CurrentTime()
        {
            return XSFUtil.CurrentMS/ 10;
        }

        private void InnerAdd(Timer nt)
        {
            uint nTime = nt.Expire;
            uint nCurTime = m_nTime;
            //Serilog.Log.Information("InnerAdd timer={0}, Expire={1}", nt.DebugStr, nt.Expire);
            if ((nTime|TIME_NEAR_MASK)==(nCurTime|TIME_NEAR_MASK)) 
            {
                m_Near[nTime&TIME_NEAR_MASK].Link(nt);
                //Serilog.Log.Information("InnerAdd Add Near, index={0}", nTime&TIME_NEAR_MASK);
            } 
            else 
            {
                int i = 0;
                uint mask = TIME_NEAR << TIME_LEVEL_SHIFT;
                for (i = 0; i < 3; i ++) 
                {
                    if ((nTime|(mask-1))==(nCurTime|(mask-1))) 
                    {
                        break;
                    }

                    mask <<= TIME_LEVEL_SHIFT;
                }

                m_T[i,((nTime>>(TIME_NEAR_SHIFT + i*TIME_LEVEL_SHIFT)) & TIME_LEVEL_MASK)].Link(nt);	

                //Serilog.Log.Information("InnerAdd Add m_T, index={0},{1}", i, ((nTime>>(TIME_NEAR_SHIFT + i*TIME_LEVEL_SHIFT)) & TIME_LEVEL_MASK));
            }
        }

        void MoveList(int nLevel, int nIndex)
        {
            Timer pCurrent = m_T[nLevel, nIndex].Clear();
            while (pCurrent != null) 
            {
                Timer pCurWork = pCurrent;
                pCurrent = pCurrent.Next;

                if( pCurWork.IsWorking )
                {
                    InnerAdd(pCurWork);
                }
                else
                {
                    TimerEvent TEvent = new TimerEvent();
                    TEvent.nID = pCurWork.nID;
                    TEvent.nTimerID = pCurWork.nTimerID;
                    TEvent.handler = pCurWork.handler;
                    TEvent.bRemove = true;
                    m_EventQueue.Push(TEvent);
                    XSFServer.Instance.SpeedUp();
                }
            }
        }

        void TimerShift()
        {
            int mask = TIME_NEAR;
            uint ct = ++ m_nTime;
            if (ct == 0) 
            {
                MoveList(3, 0);
            } 
            else 
            {
                uint time = ct >> TIME_NEAR_SHIFT;
                int i=0;

                while ((ct & (mask-1))==0) 
                {
                    int idx= (int)(time & TIME_LEVEL_MASK);
                    if (idx!=0) 
                    {
                        MoveList(i, idx);
                        break;				
                    }
                    mask <<= TIME_LEVEL_SHIFT;
                    time >>= TIME_LEVEL_SHIFT;
                    ++i;
                }
            }
        }

        void DispatchList(Timer pCurrent)
        {
            bool bSpeedup = false;
            while( pCurrent != null )
            {
                TimerEvent TEvent = new TimerEvent();
                TEvent.nID = pCurrent.nID;
                TEvent.nTimerID = pCurrent.nTimerID;
                TEvent.handler = pCurrent.handler;
                TEvent.bRemove = false;

                //Serilog.Log.Information("Timer dispatch, message={0}", pCurrent.DebugStr);

                bSpeedup = true;

                Timer pCurWork = pCurrent;
                pCurrent = pCurWork.Next;

                if( pCurWork.IsWorking )
                {
                    if( pCurWork.Times > 0 )
                    {
                        pCurWork.Times --;
                        if( pCurWork.Times <= 0 )
                        {
                            TEvent.bRemove = true;
                        }
                        else
                        {
                            m_TempList.Link(pCurWork);
                        }
                    }
                    else
                    {
                        m_TempList.Link(pCurWork);
                    }
                }
                else
                {
                    TEvent.bRemove = true;
                }

                m_EventQueue.Push(TEvent);
            }

            if( bSpeedup )
                XSFServer.Instance.SpeedUp();
        }

        void TimerExecute()
        {
            int idx = (int)(m_nTime & TIME_NEAR_MASK);

            bool lockEnter = false;
            while (m_Near[idx].Head.Next != null) 
            {
                Timer pCurrent = m_Near[idx].Clear();

                m_SpinLock.Exit();
                DispatchList(pCurrent);
                m_SpinLock.Enter(ref lockEnter);
            }
        }

        void TimerUpdate()
        {
            bool lockEnter = false;
            m_SpinLock.Enter(ref lockEnter);

            TimerExecute();

            TimerShift();

            TimerExecute();

            Timer pCurrent = m_TempList.Clear();
            while( pCurrent != null )
            {
                Timer pCurWork = pCurrent;
                pCurrent = pCurrent.Next;

                pCurWork.Expire = m_nTime + pCurWork.Interval;
                pCurWork.Next = null;

                InnerAdd(pCurWork);
            }

            m_SpinLock.Exit();
        }

        private List<ulong> m_DeleteList;

        internal bool Dispatch()
        {
            var current = XSFUtil.CurrentMS;

            TimerEvent TEvent;
            while( m_bIsRunning && m_EventQueue.Pop(out TEvent) )
            {
                //Serilog.Log.Information("定时器触发了, id={0}", TEvent.nID);

                Timer t = null;
                if(m_Timers.TryGetValue(TEvent.nID, out t))
                {
                    //Serilog.Log.Information("定时器触发了，message={0}", t.DebugStr);
                    if(t.IsWorking)
                    {
                        TEvent.handler.OnTimer(TEvent.nTimerID, TEvent.bRemove);
                    }

                    if(TEvent.bRemove)
                    {
                        m_DeleteList.Add(TEvent.nID);
                    }
                    
                    if(XSFUtil.CurrentMS > current + 200)
                    {
                        DoDelete();
                        return true;
                    }
                }
            }

            DoDelete();
            return false;
        }

        void DoDelete()
        {
            for(int i = 0; i < m_DeleteList.Count; i ++)
            {
                m_Timers.Remove(m_DeleteList[i]);
            }

            m_DeleteList.Clear();
        }
    }
}