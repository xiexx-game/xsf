//////////////////////////////////////////////////////////////////////////
//
// 文件：source/xsf/src/XSFThread.cpp
// 作者：Xoen Xie
// 时间：2023/08/14
// 描述：线程封装类
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#include "XSFThread.h"

namespace xsf
{
    void ThreadProc( XSFSimpleThread * pThreadObj )
    {
        pThreadObj->OnThreadStart();

        pThreadObj->OnThreadRun();

        pThreadObj->OnThreadExit();
    }

    // 开启线程
    void XSFSimpleThread::StartThread(void)
    {
        m_Thread = thread( ThreadProc, this);
    }





    void XSFLoopThread::StartThread(void)
    {
        m_bThreadWorking = true;

        XSFSimpleThread::StartThread();
    }

    void XSFLoopThread::StopThread(void)
    {
        {
            if (!m_bThreadWorking)
                return;
                
            m_bThreadWorking = false;
            m_cv.notify_one();
        }
        
        ThreadJoin();
    }

    void XSFLoopThread::OnThreadRun(void)
    {
        std::unique_lock<std::mutex> lk(m_Mutex);

        while (m_bThreadWorking)
        {
            m_cv.wait(lk);

            OnThreadLoop();
        }
    }
}
