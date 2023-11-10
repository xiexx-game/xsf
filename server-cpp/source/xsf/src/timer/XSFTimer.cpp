//////////////////////////////////////////////////////////////////////////
//
// 文件：source/xsf/src/timer/XSFTimer.cpp
// 作者：Xoen Xie
// 时间：2023/08/14
// 描述：定时器
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#include "XSFTimer.h"
#include "XSF.h"
#include "XSFServer.h"

namespace xsf
{
    XSFTimer::XSFTimer(void)
    {


    }

    XSFTimer::~XSFTimer(void)
    {


    }

    bool XSFTimer::Create(void)
    {
        m_nCurrentPoint = CurrentTime();

        for ( int32 i = 0; i < TIME_NEAR; i++ ) 
        {
            m_Near[i].Clear();
        }

        for ( int32 i = 0; i < 4; i++ ) 
        {
            for (int32 j = 0; j < TIME_LEVEL; j++ ) 
            {
                m_T[i][j].Clear();
            }
        }

        m_TempList.Clear();

        uint32 time = XSFCore::Time();
        XSFCore::GetLocalTM( time, m_LastTM );

        m_bWorking = true;
        StartThread();

        return true;
    }

    void XSFTimer::Release(void)
    {
        XSF_INFO("Timer Release start");

        m_bWorking = false;

        ThreadJoin();

        for( auto it = m_Timers.begin(); it != m_Timers.end(); ++ it )
        {
            //XSF_ERROR("XSFTimer::Release delete timer=%lu", it->second);
            delete it->second;
        }

        TimerEvent TEvent;
        while( m_EventQueue.Pop(TEvent) )
        {
            XSF_WARN("m_EventQueue.Pop");
        }

        XSF_INFO("Timer Release end");

        delete this;
    }

    uint64 XSFTimer::Add( uint8 nTimerID, ITimerHandler * pHandler, uint32 nInterval, int32 nTimes, const char * sDebugStr )
    {
        if( !m_bWorking || nInterval <= 0 )
            return 0;

        Timer * pNewTimer = new Timer();
        pNewTimer->nTimerID = nTimerID;
        pNewTimer->pHandler = pHandler;
        pNewTimer->nInterval = nInterval;
        pNewTimer->nTimes = nTimes;
        pNewTimer->bWorking = true;
        strcpy( pNewTimer->sDebugStr, sDebugStr );

        if( pNewTimer->nTimes == 0 )
            pNewTimer->nTimes = 1;

        uint32 nCurTime = XSFCore::ClockMonotonic();
        m_nIndex ++;
        pNewTimer->nID = UINT64_ID(nCurTime, m_nIndex);
        
        m_Timers.insert( TimerMap::value_type(pNewTimer->nID, pNewTimer));

        XSF_SPINLOCK_LOCK;

        pNewTimer->nExpire = m_nTime + nInterval;
        InnerAdd(pNewTimer);

        XSF_SPINLOCK_UNLOCK;

        return pNewTimer->nID;
    }

    // 线程执行内容
    void XSFTimer::OnThreadRun(void)
    {
        while( m_bWorking )
        {
            uint64 nCurrentPoint = CurrentTime();

            if( nCurrentPoint > m_nCurrentPoint )
            {
                uint32 nDiff = (uint32)(nCurrentPoint - m_nCurrentPoint);
                m_nCurrentPoint = nCurrentPoint;

                for( uint32 i = 0; i < nDiff; ++ i )
                {
                    TimerUpdate();
                }
            }
            
            usleep(2500);
        }
    }

    bool XSFTimer::Dispatch(void)
    {
        uint32 nTickStart = XSFCore::TickCount();

        TimerEvent TEvent;
        while( m_bWorking && m_EventQueue.Pop(TEvent) )
        {
            TimerMap::iterator it = m_Timers.find(TEvent.nID);
            if( it != m_Timers.end() )
            {
                Timer * pTimer = it->second;

                if( pTimer->bWorking )
                {
                    TEvent.pHandler->OnTimer(TEvent.nTimerID, TEvent.bRemove);
                }

                if( TEvent.bRemove )
                {
                    m_Timers.erase(it);
                    delete pTimer;
                }

                if( XSFCore::TickCount()-nTickStart > 200 )
                {
                    return true;
                }
            }
            
        }

        return false;
    }

    void XSFTimer::InnerAdd( Timer * pTimer )
    {
        uint32 nTime = pTimer->nExpire;
        uint32 nCurTime = m_nTime;

        if ((nTime|TIME_NEAR_MASK)==(nCurTime|TIME_NEAR_MASK)) 
        {
            m_Near[nTime&TIME_NEAR_MASK].Link(pTimer);
        } 
        else 
        {
            int32 i = 0;
            uint32 mask=TIME_NEAR << TIME_LEVEL_SHIFT;
            for (i = 0; i < 3; i ++) 
            {
                if ((nTime|(mask-1))==(nCurTime|(mask-1))) 
                {
                    break;
                }

                mask <<= TIME_LEVEL_SHIFT;
            }

            m_T[i][((nTime>>(TIME_NEAR_SHIFT + i*TIME_LEVEL_SHIFT)) & TIME_LEVEL_MASK)].Link(pTimer);	
        }
    }

    void XSFTimer::MoveList(int32 nLevel, int32 nIndex)
    {
        Timer * pCurrent = m_T[nLevel][nIndex].Clear();
        while (pCurrent != nullptr) 
        {
            Timer * pCurWork = pCurrent;
            pCurrent = pCurrent->pNext;

            if( pCurWork->bWorking )
            {
                InnerAdd(pCurWork);
            }
            else
            {
                TimerEvent TEvent;
                TEvent.nID = pCurWork->nID;
                TEvent.nTimerID = pCurWork->nTimerID;
                TEvent.pHandler = pCurWork->pHandler;
                TEvent.bRemove = true;
                m_EventQueue.Push(TEvent);
            }
        }
    }

    void XSFTimer::TimerShift(void)
    {
        int mask = TIME_NEAR;
        uint32 ct = ++ m_nTime;
        if (ct == 0) 
        {
            MoveList(3, 0);
        } 
        else 
        {
            uint32 time = ct >> TIME_NEAR_SHIFT;
            int i=0;

            while ((ct & (mask-1))==0) 
            {
                int idx=time & TIME_LEVEL_MASK;
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

    void XSFTimer::DispatchList(Timer * pCurrent)
    {
        bool bSpeedup = false;
        while( pCurrent != nullptr )
        {
            TimerEvent TEvent;
            TEvent.nID = pCurrent->nID;
            TEvent.nTimerID = pCurrent->nTimerID;
            TEvent.pHandler = pCurrent->pHandler;
            TEvent.bRemove = false;

            bSpeedup = true;

            Timer * pCurWork = pCurrent;
            pCurrent = pCurWork->pNext;

            if( pCurWork->bWorking )
            {
                if( pCurWork->nTimes > 0 )
                {
                    pCurWork->nTimes --;
                    if( pCurWork->nTimes <= 0 )
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
            XSFServer::Instance()->Speedup();
    }
    
    void XSFTimer::TimerExecute(void)
    {
        int32 idx = m_nTime & TIME_NEAR_MASK;

        while (m_Near[idx].head.pNext != nullptr) 
        {
            Timer * pCurrent = m_Near[idx].Clear();

            XSF_SPINLOCK_UNLOCK;
            DispatchList(pCurrent);
            XSF_SPINLOCK_LOCK;
        }
    }

    void XSFTimer::TimerUpdate(void)
    {
        XSF_SPINLOCK_LOCK;

        TimerExecute();

        TimerShift();

        TimerExecute();

        Timer * pCurrent = m_TempList.Clear();
        while( pCurrent != nullptr )
        {
            Timer * pCurWork = pCurrent;
            pCurrent = pCurrent->pNext;

            pCurWork->nExpire = m_nTime + pCurWork->nInterval;
            pCurWork->pNext = nullptr;

            InnerAdd(pCurWork);
        }

        XSF_SPINLOCK_UNLOCK;
    }

    uint64 XSFTimer::CurrentTime(void)
    {
        struct timeval tv;
        gettimeofday(&tv, NULL);
        uint64 t = (uint64)tv.tv_sec * 100;
        t += tv.tv_usec / 10000;
        return t;
    }



    TimerManager::TimerManager(int32 nMaxID)
        : m_nMaxID(nMaxID)
    {
        m_Timers = (uint64*)xsf_malloc(sizeof(uint64)*nMaxID);
    }

    TimerManager::~TimerManager(void)
    {
        xsf_free(m_Timers);
    }

    void TimerManager::StartTimer( uint8 nTimerID, ITimerHandler * pHandler, uint32 nInterval, int32 nTimes, const char * sDebugStr )
    {
        auto timer = XSFServer::Instance()->GetTimer();
        if(m_Timers[nTimerID] > 0)
        {
            timer->Del(m_Timers[nTimerID]);
            m_Timers[nTimerID] = 0;
        }
            
        m_Timers[nTimerID] = timer->Add(nTimerID, pHandler, nInterval, nTimes, sDebugStr);
    }

    void TimerManager::DelTimer(uint64 nTimerID)
    {
        auto timer = XSFServer::Instance()->GetTimer();
        if(m_Timers[nTimerID] > 0)
        {
            timer->Del(m_Timers[nTimerID]);
            m_Timers[nTimerID] = 0;
        }
    }

    void TimerManager::CloseAll(void)
    {
        auto timer = XSFServer::Instance()->GetTimer();
        for(int32 i = 0; i < m_nMaxID; i ++)
        {
            if(m_Timers[i] > 0)
            {
                timer->Del(m_Timers[i]);
                m_Timers[i] = 0;
            }
        }
    }
}