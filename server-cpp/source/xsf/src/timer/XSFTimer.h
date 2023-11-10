//////////////////////////////////////////////////////////////////////////
//
// 文件：source/xsf/src/timer/XSFTimer.h
// 作者：Xoen Xie
// 时间：2023/08/14
// 描述：定时器
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _XSF_TIMER_H_
#define _XSF_TIMER_H_

#include "XSFDef.h"
#include "IXSFTimer.h"
#include "XSFThread.h"
#include "XSFMallocHook.h"
#include "XSFQueue.h"
#include "XSFLog.h"

namespace xsf
{
    #define TIME_NEAR_SHIFT 8
    #define TIME_NEAR (1 << TIME_NEAR_SHIFT)
    #define TIME_LEVEL_SHIFT 6
    #define TIME_LEVEL (1 << TIME_LEVEL_SHIFT)
    #define TIME_NEAR_MASK (TIME_NEAR-1)
    #define TIME_LEVEL_MASK (TIME_LEVEL-1)

    struct Timer
    {
        uint64 nID = 0;
        volatile bool bWorking = false;
        uint8 nTimerID = 0;
        ITimerHandler * pHandler = nullptr;
        uint32 nInterval = 0xFFFFFFFF;
        int32 nTimes = 0;
        uint32 nExpire = 0;
        char sDebugStr[BUFFER_SIZE_128] = {0};

        Timer * pNext = nullptr;
    
        JEMALLOC_HOOK;
    };

    struct TimerList
    {
        Timer head;
        Timer * pTail = nullptr;

        Timer * Clear(void)
        {
            Timer * ret = head.pNext;
            head.pNext = nullptr;
            pTail = &(head);

            return ret;
        }

        void Link( Timer * t )
        {
            pTail->pNext = t;
            pTail = t;
            t->pNext = nullptr;
        }
    };

    struct IXSFDateHandler;

    class XSFTimer : public XSFSimpleThread
    {
        using TimerMap = unordered_map<uint64, Timer*>;
        struct TimerEvent
        {
            uint64 nID = 0;
            ITimerHandler * pHandler;
            uint8 nTimerID;
            bool bRemove = false;
        };

    public:
        XSFTimer(void);
        ~XSFTimer(void);

        bool Create(void);

        void Release(void);

        uint64 Add( uint8 nTimerID, ITimerHandler * pHandler, uint32 nInterval, int32 nTimes, const char * sDebugStr );

        void Del(uint64 nTimerID)
        {
            TimerMap::iterator it = m_Timers.find(nTimerID);
            if( it != m_Timers.end() )
            {
                it->second->bWorking = false;
            }
        }

        // 线程执行内容
        void OnThreadRun(void) override;

        bool Dispatch(void);

        void SetDateHandler( IXSFDateHandler * pHandler ) { m_pDateHandler = pHandler; }

    private:
        void InnerAdd( Timer * pTimer );
        void MoveList(int32 nLevel, int32 nIndex);
        void TimerShift(void);
        void DispatchList(Timer * pCurrent);
        void TimerExecute(void);
        void TimerUpdate(void);
        uint64 CurrentTime(void);

    private:
        bool m_bWorking = false;
        uint32 m_nTime = 0;
        uint64 m_nCurrentPoint = 0;

        TimerList m_Near[TIME_NEAR];
	    TimerList m_T[4][TIME_LEVEL];

        TimerList m_TempList;

        XSF_SPINLOCK_DEFINE;

        TimerMap m_Timers;
        uint32 m_nIndex = 0;

        XSFQueue<TimerEvent> m_EventQueue;

        XSF_TM m_LastTM;
        XSF_TM m_CurTM;
        IXSFDateHandler * m_pDateHandler = nullptr;
    };

}

#endif      // end of _XSF_TIMER_H_