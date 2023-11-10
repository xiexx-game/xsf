//////////////////////////////////////////////////////////////////////////
//
// 文件：source/xsf/inc/IXSFTimer.h
// 作者：Xoen Xie
// 时间：2023/08/14
// 描述：定时器相关定义
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _I_XSF_TIMER_H_
#define _I_XSF_TIMER_H_

#include "XSFDef.h"

namespace xsf
{
    #define INFINITE_LOOP -1


    #define TIMER_ID_START                              \
    enum EMTimerID                                      \
    {                                                   \
        TimerID_Start = 0,                              \
                                                        \


    #define TIMER_ID_END                                \
        TimerID_Max,                                    \
    };                                                  \
                                                        \


    #define TIMER_FUNCTION_DEFINE void OnTimer( uint8 nTimerID, bool bLastCall ) override;


    #define TIMER_FUNCTION_START(_CLASS_NAME)				        \
    typedef void * (_CLASS_NAME::* TIMER_CALL)( bool bLastCall );   \
    void _CLASS_NAME::OnTimer( uint8 nTimerID, bool bLastCall )	    \
    {																\
        static TIMER_CALL timerCall[TimerID_Max] =	                \
        {                                                           \
            NULL,			                                        \
                                                                    \



    #define TIMER_FUNCTION_END	                                    \
        };                                                          \
                                                                    \
        (this->*timerCall[nTimerID])(bLastCall);                    \
    }                                                               \
                                                                    \



    struct ITimerHandler
    {
        virtual ~ITimerHandler(void) {}

        // bLastCall 如果是最后一次调用，则为true，否则为false
        virtual void OnTimer( uint8 nTimerID, bool bLastCall ) = 0;
    };


    class TimerManager
    {
    public:
        TimerManager(int32 nMaxID);
        ~TimerManager(void);

        void StartTimer( uint8 nTimerID, ITimerHandler * pHandler, uint32 nInterval, int32 nTimes, const char * sDebugStr );

        void DelTimer(uint64 nTimerID);

        void CloseAll(void);

    private:
        int32 m_nMaxID = 0;
        uint64 *m_Timers = nullptr;
    };
}


#endif      // end of _I_XSF_TIMER_H_