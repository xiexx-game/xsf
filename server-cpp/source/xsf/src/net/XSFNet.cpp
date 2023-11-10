//////////////////////////////////////////////////////////////////////////
//
// 文件：source/xsf/src/net/XSFNet.cpp
// 作者：Xoen Xie
// 时间：2023/08/14
// 描述：网络系统
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#include <sys/eventfd.h>

#include "XSFNet.h"
#include "ConnectionTCP.h"
#include "XSFLog.h"
#include "XSFNetMisc.h"
#include "XSF.h"

namespace xsf
{
    XSFNet::XSFNet(void)
    {

    }

    XSFNet::~XSFNet(void)
    {

    }

    bool XSFNet::Create(void)
    {
        NET_FUNCTION_ENTER(0, 0);

        
        m_nEventFD = eventfd(0, 0);

        m_nEpollFD = EpollCreate();
        if (INVALID_FD == m_nEpollFD)
        {
            XSF_ERROR("XSFNet::Create faild, SU_INVALID_FD == m_nEpollFD");
            return false;
        }

        if (!EpollAdd(m_nEpollFD, m_nEventFD, this))
        {
            XSF_ERROR("XSFNet::Create epoll add error");
            return false;
        }

        m_bWorking = true;

        StartThread();

        NET_FUNCTION_EXIT(0, 0);

        return true;
    }

    void XSFNet::Release(void)
    {
        NET_FUNCTION_ENTER(0, 0);

        XSF_INFO("XSFNet::Release start");

        m_bWorking = false;

        PushOperation(new NetOperationShutdown());

        ThreadJoin();

        INetEvent * pEvent = nullptr;
        while( m_EventOut.Pop(pEvent) )
        {
            pEvent->Release();
            pEvent = nullptr;
        }

        if( INVALID_FD != m_nEventFD )
            EpollRelease(m_nEventFD);

        if( INVALID_FD != m_nEpollFD )
            EpollRelease(m_nEpollFD);


        XSF_INFO("XSFNet::Release end");

        NET_FUNCTION_EXIT(0, 0);

        delete this;
    }


    ConnectionTCP * XSFNet::CreateConnection( INetHandler * pHandler, INetPacker *packer )
    {
        if( pHandler == nullptr )
            return nullptr;

        ConnectionTCP * pConnection = new ConnectionTCP();
        //XSF_ERROR("XSFNet::CreateConnection pNewConnection=%lu", pConnection);
        if( !pConnection->Create(packer) )
        {
            pConnection->Close();
            return nullptr;
        }

        m_nConnCount ++;

        pConnection->SetNetHandler(pHandler);

        return pConnection;
    }


    void XSFNet::PushOperation(INetOperation * pOperation)
    {
        NET_FUNCTION_ENTER(0, 0);

        m_Operations.Push(pOperation);

        uint64 nData = 1;
        int nRet = write(m_nEventFD, &nData, sizeof(nData));
        if (nRet != sizeof(nData))
        {
            XSF_ERROR("XSFNet::PushOperation error, nRet != sizeof(nData), nRet=%d", nRet);
        }

        NET_FUNCTION_EXIT(0, 0);
    }

    bool XSFNet::Dispatch(void)
    {
        uint32 nTickStart = XSFCore::TickCount();

        INetEvent * pEvent = nullptr;
        while( m_EventOut.Pop(pEvent) )
        {
            pEvent->OnEvent();
            pEvent->Release();
            pEvent = nullptr;

            if( XSFCore::TickCount() - nTickStart >= 128 )
            {
                return true;
            }
        }

        return false;
    }
    

    bool XSFNet::EpollSocketAdd(ConnectionTCP * pConnection)
    {
        NET_FUNCTION_ENTER(0, pConnection->GetSocketID());

        if (!EpollAdd(m_nEpollFD, pConnection->m_nSocketFD, pConnection))
        {
            NET_FUNCTION_EXIT(1, pConnection->GetSocketID());
            return false;
        }

        pConnection->m_bInEpoll = true;

        NET_FUNCTION_EXIT(0, pConnection->GetSocketID());

        return true;
    }

    void XSFNet::EpollSocketModify(ConnectionTCP * pConnection, bool bWriteEnable)
    {
        NET_FUNCTION_ENTER(0, pConnection->GetSocketID());

        EpollModify(m_nEpollFD, pConnection->m_nSocketFD, pConnection, bWriteEnable);

        pConnection->m_bWriteEnable = bWriteEnable;

        NET_FUNCTION_EXIT(0, pConnection->GetSocketID());
    }

    void XSFNet::EpollSocketDel(ConnectionTCP * pConnection)
    {
        NET_FUNCTION_ENTER(0, pConnection->GetSocketID());

        if( pConnection->m_bInEpoll )
        {
            NET_FUNCTION_ENTER(1, pConnection->GetSocketID());

            EpollDel(m_nEpollFD, pConnection->m_nSocketFD);

            for( int i = 0; i < m_nEventCount; ++ i )
            {
                if ((ConnectionTCP*)(m_SPWaitEvent[i].ud) == pConnection)
				{
                    NET_FUNCTION_ENTER(2, pConnection->GetSocketID());
					m_SPWaitEvent[i].ud = nullptr;
					break;
				}
            }

            pConnection->m_bInEpoll = false;
        }
            
        NET_FUNCTION_EXIT(0, pConnection->GetSocketID());
    }




    // 线程执行内容
    void XSFNet::OnThreadRun(void)
    {
        m_bIsThreadRunning = true;

	    uint64 nEventData = 0;

        while(true)
        {
            m_nEventCount = EpollWait(m_nEpollFD, m_SPWaitEvent, -1);
            
#ifdef _XSF_NET_DEBUG_
            XSF_INFO("XSFNet::OnThreadRun m_nEventCount=%u", m_nEventCount);
#endif

            for (int nEventIndex = 0; nEventIndex < m_nEventCount; ++nEventIndex)
            {
                EpollEvent * pEvent = &(m_SPWaitEvent[nEventIndex]);
                if( pEvent->ud == nullptr )
                    continue;

                if ( this == pEvent->ud )
                {
                    NET_FUNCTION_ENTER(1, 0);
                    read(m_nEventFD, &nEventData, sizeof(nEventData));	// 把写入数据读取
    
                    OnOperationEvent();
                }
                else
                {
#ifdef _XSF_NET_DEBUG_
                    ConnectionTCP * pConnection = (ConnectionTCP*)(pEvent->ud);
                    NET_FUNCTION_ENTER(2, pConnection->GetSocketID() );
#endif
                    OnNetEvent(pEvent);
                }
            }
            
            if( !m_bWorking && m_Operations.GetCount() <= 0 && m_nConnCount <= 0 )
            {
                XSF_INFO("XSFNet::OnThreadRun exit ....");
                break;
            }
        }

        NET_FUNCTION_EXIT(0, 0);
    }


    void XSFNet::OnOperationEvent(void)
    {
        NET_FUNCTION_ENTER(0, 0);

        INetOperation * pOperation = NULL;

        while (m_Operations.Pop(pOperation))
        {
            pOperation->Execute(this);
            delete pOperation;
            pOperation = NULL;
        }

        NET_FUNCTION_EXIT(0, 0);
    }

    void XSFNet::OnNetEvent(EpollEvent * pEvent)
    {
        NET_FUNCTION_ENTER(0, 0);

        ConnectionTCP * pConnection = (ConnectionTCP*)(pEvent->ud);

        switch (pConnection->m_nNetStatus)
        {
        case NetStatus_Accept:
            NET_FUNCTION_ENTER(1, pConnection->GetSocketID());
            pConnection->EpollAccept();
            break;

        case NetStatus_Connecting:
            NET_FUNCTION_ENTER(2, pConnection->GetSocketID());
            if( pConnection->EpollCheckConnect() )
            {
                NET_FUNCTION_ENTER(3, pConnection->GetSocketID());
                EpollSocketModify(pConnection, false);
            }
            else
            {
                NET_FUNCTION_ENTER(4, pConnection->GetSocketID());
                EpollSocketDel(pConnection);
            }
            break;

        case NetStatus_Release:
        case NetStatus_Work:
            {
                NET_FUNCTION_ENTER(5, pConnection->GetSocketID());
                if( pEvent->read )
                {
                    NET_FUNCTION_ENTER(6, pConnection->GetSocketID());
                    if( !pConnection->EpollRecv() )
                    {
                        NET_FUNCTION_ENTER(7, pConnection->GetSocketID());
                        EpollSocketDel(pConnection);
                        return;
                    }
                }

                NET_FUNCTION_ENTER(8, pConnection->GetSocketID());
                if( pEvent->write )
                {
                    NET_FUNCTION_ENTER(9, pConnection->GetSocketID());
                    uint8 nResult = pConnection->EpollSend();
                    if( nResult == SendResult_Error )
                        EpollSocketDel(pConnection);
                    else if( nResult == SendResult_Done )
                        EpollSocketModify(pConnection, false);
                }
            }
            break;
        
        default:
            XSF_ERROR("XSFNet::OnNetEvent status error, status=%d, write=%s, read=%s", pConnection->m_nNetStatus, pEvent->write ? "true": "false", pEvent->read ? "true" : "false");
            break;
        }

        NET_FUNCTION_EXIT(0, 0);
    }

}
