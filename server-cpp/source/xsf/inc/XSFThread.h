//////////////////////////////////////////////////////////////////////////
//
// 文件：source/xsf/inc/XSFThread.h
// 作者：Xoen Xie
// 时间：2023/08/14
// 描述：线程封装类
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _XSF_THREAD_H_
#define _XSF_THREAD_H_

#include <thread>
#include <mutex>
#include <condition_variable>
#include <atomic>

#include "XSFDef.h"

namespace xsf
{
    #define XSF_SPINLOCK_DEFINE   std::atomic_flag m_Lock = ATOMIC_FLAG_INIT;

    #define XSF_SPINLOCK_LOCK                                           \
    while (m_Lock.test_and_set(std::memory_order_acquire))              \
                ;                                                      \


    #define XSF_SPINLOCK_UNLOCK     m_Lock.clear(std::memory_order_release);


    class XSFSimpleThread
    {
    public:
        XSFSimpleThread(void) {}
        virtual ~XSFSimpleThread(void) {}

        thread::id GetThreadID(void) { return m_Thread.get_id(); }

        // 开启线程
        virtual void StartThread(void);

        // 线程开启后首先被调用
        virtual void OnThreadStart(void) {}

        // 线程退出前被调用
        virtual void OnThreadExit(void) {}

        // 线程执行内容
        virtual void OnThreadRun(void) = 0;

        // 等待线程结束
        void ThreadJoin(void) { m_Thread.join(); }

    private:
        thread m_Thread;
    };


    class XSFLoopThread : public XSFSimpleThread
    {
    public:
        XSFLoopThread(void) {}
        virtual ~XSFLoopThread(void) {}

        // 开启线程
        void StartThread(void) override;

        void StopThread(void);

        virtual void OnThreadRun(void) override;

        // 唤醒线程
        void ThreadWakeup(void)
        {
            m_cv.notify_one();
        }

        virtual void OnThreadLoop(void) = 0;

        bool IsWorking(void) { return m_bThreadWorking; }

    protected:
        mutex m_Mutex;
        condition_variable m_cv;

        volatile bool m_bThreadWorking = false;
    };

}


#endif      // end of _XSF_THREAD_H_