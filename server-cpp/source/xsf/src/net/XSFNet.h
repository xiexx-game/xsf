//////////////////////////////////////////////////////////////////////////
//
// 文件：source/xsf/src/net/XSFNet.h
// 作者：Xoen Xie
// 时间：2023/08/14
// 描述：网络系统
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _XSF_NET_H_
#define _XSF_NET_H_

#include "XSFDef.h"
#include "XSFThread.h"
#include "IXSFNet.h"
#include "XSFNetDef.h"
#include "XSFQueue.h"
#include "XSFEpoll.h"
#include "XSFServer.h"

namespace xsf
{
    class ConnectionTCP;

    class XSFNet : public XSFSimpleThread
    {
    public:
        XSFNet(void);
        ~XSFNet(void);

        bool Create(void);

        void Release(void);

        int32 GetEpollFD(void) { return m_nEpollFD; }

    public:
        ConnectionTCP * CreateConnection(INetHandler * pHandler, INetPacker * packer);


        void PushOperation(INetOperation * pOperation);

        bool Dispatch(void);

        void SubConnCount(void)
        {
            if( m_nConnCount > 0 )
                m_nConnCount --;
        }

        uint32 GetConnCount(void)
        {
            return m_nConnCount;
        }

    public:
        bool EpollSocketAdd(ConnectionTCP * pConnection);
        void EpollSocketModify(ConnectionTCP * pConnection, bool bWriteEnable);
        void EpollSocketDel(ConnectionTCP * pConnection);

        void PushEvent(INetEvent * pEvent)
        {
            m_EventOut.Push(pEvent);

            // 触发主线程加速处理
            XSFServer::Instance()->Speedup();
        }

    public:
        // 线程执行内容
        void OnThreadRun(void) override;

    private:
        void OnOperationEvent(void);
        void OnNetEvent(EpollEvent * pEvent);

    private:
        int32 m_nEpollFD = INVALID_FD;			// epoll中心描述符
	    int32 m_nEventFD = INVALID_FD;			// 事件通知描述符

        bool m_bWorking = false;

        XSFQueue<INetOperation*> m_Operations;


        EpollEvent m_SPWaitEvent[MAX_EPOLL_EVENT];
        int m_nEventCount = 0;
        bool m_bIsThreadRunning = false;

        uint32 m_nConnCount = 0;

        XSFQueue<INetEvent*> m_EventOut;
    };

} // namespace xsf


#endif      // end of _XSF_NET_H_