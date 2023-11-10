//////////////////////////////////////////////////////////////////////////
//
// 文件：source/xsf/src/net/XSFNetMisc.h
// 作者：Xoen Xie
// 时间：2023/08/14
// 描述：网络相关扩展
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _XSF_NET_MISC_H_
#define _XSF_NET_MISC_H_

#include "XSFNetDef.h"

#include "ConnectionTCP.h"
#include "XSFLog.h"
#include "XSFNet.h"

namespace xsf
{
    // 连接关闭
    class NetOperationClose : public INetOperation
    {
    public:
        NetOperationClose(ConnectionTCP * pConnection )
            : m_pConnection(pConnection)
        {

        }

        void Execute( XSFNet * pNet ) override
        {
            NET_FUNCTION_ENTER(0, m_pConnection->GetSocketID());
            
            pNet->EpollSocketDel(m_pConnection);
            
            m_pConnection->EpollRelease();

            pNet->SubConnCount();
            
            NET_FUNCTION_EXIT(0, 0);
        }

    private:
        ConnectionTCP * m_pConnection = nullptr;
    };

    //////////////////////////////////////////////////////////////////////////
    //
    //  事件相关
    //
    //////////////////////////////////////////////////////////////////////////


    // 新连接进入
    class NetEventAccept : public INetEvent
    {
    public:
        NetEventAccept(ConnectionTCP * pAcceptor, ConnectionTCP * pNewConnection)
            : m_pAcceptor(pAcceptor)
            , m_pNewConnection(pNewConnection)
        {

        }

        void OnEvent(void) override
        {
            NET_FUNCTION_ENTER(0, m_pNewConnection->GetSocketID());

            INetHandler * pHandler = m_pAcceptor->GetNetHandler();
            if( pHandler != nullptr )
            {
                NET_FUNCTION_ENTER(1, 0);

                INetHandler * pNewHandler = pHandler->OnAccept(m_pNewConnection);
                if( pNewHandler != nullptr )
                {
                    NET_FUNCTION_ENTER(2, m_pNewConnection->GetSocketID());
                    m_pNewConnection->SetNetHandler(pNewHandler);
                }
                else
                {
                    m_pNewConnection->Close();
                }
            }
            

            NET_FUNCTION_EXIT(0, m_pNewConnection->GetSocketID());
        }

    private:
        ConnectionTCP * m_pAcceptor = nullptr;
        ConnectionTCP * m_pNewConnection = nullptr;
    };

    // 连接对端成功
    class NetEventConnected : public INetEvent
    {
    public:
        NetEventConnected(ConnectionTCP * pConnection )
            : m_pConnection(pConnection)
        {

        }

        void OnEvent(void) override
        {
            NET_FUNCTION_ENTER(0, m_pConnection->GetSocketID());

            INetHandler * pHandler = m_pConnection->GetNetHandler();
            if( pHandler != nullptr )
            {
                NET_FUNCTION_ENTER(1, m_pConnection->GetSocketID());

                pHandler->OnConnected(m_pConnection);
            }

            NET_FUNCTION_EXIT(0, m_pConnection->GetSocketID());
        }

    private:
        ConnectionTCP * m_pConnection = nullptr;
    };

    // 接收到数据
    class NetEventRecv : public INetEvent
    {
    public:
        NetEventRecv(ConnectionTCP * pConnection, NetBuffer & buffer, DataResult & result )
            : m_pConnection(pConnection)
            , m_Buffer(buffer)
            , m_Result(result)
        {

        }

        void OnEvent(void) override
        {
            NET_FUNCTION_ENTER(0, m_pConnection->GetSocketID());

            INetHandler * pHandler = m_pConnection->GetNetHandler();
            if( pHandler != nullptr )
            {
                NET_FUNCTION_ENTER(1, m_pConnection->GetSocketID());

                pHandler->OnRecv(m_pConnection, &m_Result);
            }

            NET_FUNCTION_EXIT(0, m_pConnection->GetSocketID());
        }

        void Release(void) override 
        {
            m_Buffer.Free(); 
            delete this; 
        }

    private:
        ConnectionTCP * m_pConnection = nullptr;
        NetBuffer m_Buffer;
        DataResult m_Result;
    };

    // 连接出现错误
    class NetEventError : public INetEvent
    {
    public:
        NetEventError( ConnectionTCP * pConnection, uint32 nErrorCode)
            : m_pConnection(pConnection)
            , m_nErrorCode(nErrorCode)
        {

        }

        void OnEvent(void) override
        {
            NET_FUNCTION_ENTER(0, m_pConnection->GetSocketID());

            //XSF_ERROR("NetEventError m_pConnection=%lu, m_nErrorCode=%u", m_pConnection, m_nErrorCode);

            INetHandler * pHandler = m_pConnection->GetNetHandler();
            if( pHandler != nullptr )
            {
                NET_FUNCTION_ENTER(1, m_pConnection->GetSocketID());

                pHandler->OnError(m_pConnection, m_nErrorCode);
            }

            m_pConnection->Close();

            NET_FUNCTION_EXIT(0, m_pConnection->GetSocketID());
        }

    private:
        ConnectionTCP * m_pConnection = nullptr;
        uint32 m_nErrorCode = 0;
    };



    //////////////////////////////////////////////////////////////////////////
    //
    //  指令相关
    //
    //////////////////////////////////////////////////////////////////////////

    // 往epoll中添加一个连接
    class NetOperationAdd : public INetOperation
    {
    public:
        NetOperationAdd(ConnectionTCP * pConnection)
            : m_pConnection(pConnection)
        {

        }

        void Execute( XSFNet * pNet ) override
        {
            NET_FUNCTION_ENTER(0, m_pConnection->GetSocketID());
            
            if( !pNet->EpollSocketAdd(m_pConnection) )
            {
                NET_FUNCTION_ENTER(1, m_pConnection->GetSocketID());

                m_pConnection->PushEvent(new NetEventError(m_pConnection, NetCode_ErrorSocketAdd));
            }

            NET_FUNCTION_EXIT(0, m_pConnection->GetSocketID());
        }

    private:
		ConnectionTCP * m_pConnection = nullptr;
    };

    // 连接请求
    class NetOperationConnect : public INetOperation
    {
    public:
        NetOperationConnect(ConnectionTCP * pConnection)
            : m_pConnection(pConnection)
        {

        }

        void Execute( XSFNet * pNet ) override
        {
            NET_FUNCTION_ENTER(0, m_pConnection->GetSocketID());
            
            uint8 nResult = m_pConnection->EpollConnect();
            switch (nResult)
            {
            case ConnectResult_OK:
                if( pNet->EpollSocketAdd(m_pConnection) )
                {
                    NET_FUNCTION_ENTER(1, m_pConnection->GetSocketID());
                    m_pConnection->m_nNetStatus = NetStatus_Work;
                    m_pConnection->PushEvent(new NetEventConnected(m_pConnection));
                }
                else
                {
                    NET_FUNCTION_ENTER(2, m_pConnection->GetSocketID());
                    m_pConnection->PushEvent(new NetEventError(m_pConnection, NetCode_ErrorSocketAdd));
                }
                break;

            case ConnectResult_Wait:
                if (pNet->EpollSocketAdd(m_pConnection))
                {
                    NET_FUNCTION_ENTER(3, m_pConnection->GetSocketID());
                    m_pConnection->m_nNetStatus = NetStatus_Connecting;
                    pNet->EpollSocketModify(m_pConnection, true);
                }
                else
                {
                    NET_FUNCTION_ENTER(4, m_pConnection->GetSocketID());
                    m_pConnection->PushEvent(new NetEventError(m_pConnection, NetCode_ErrorSocketAdd));
                }
                break;
            
            default:
                {
                    NET_FUNCTION_ENTER(5, m_pConnection->GetSocketID());
                    m_pConnection->PushEvent(new NetEventError(m_pConnection, NetCode_ErrorConnect));
                }
                break;
            }

            NET_FUNCTION_EXIT(0, m_pConnection->GetSocketID());
        }

    private:
		ConnectionTCP * m_pConnection = nullptr;

    };

    // 发送一个数据包
    class NetOperationSend : public INetOperation
    {
    public:
        NetOperationSend(ConnectionTCP * pConnection, NetBuffer & buffer )
            : m_pConnection(pConnection)
            , m_Buffer(buffer)
        {

        }

        void Execute( XSFNet * pNet ) override
        {
            NET_FUNCTION_ENTER(0, m_pConnection->GetSocketID());
            
            if( m_pConnection->Send(m_Buffer) )
            {
                NET_FUNCTION_ENTER(1, m_pConnection->GetSocketID());
                if( !m_pConnection->m_bWriteEnable )
                {
                    NET_FUNCTION_ENTER(2, m_pConnection->GetSocketID());
                    pNet->EpollSocketModify(m_pConnection, true);
                }
            }

            NET_FUNCTION_EXIT(0, m_pConnection->GetSocketID());
        }

    private:
        ConnectionTCP * m_pConnection = nullptr;
        NetBuffer m_Buffer;
    };


    

    // 关闭网络模块
    class NetOperationShutdown : public INetOperation
    {
    public:
        void Execute( XSFNet * pNet ) override 
        {
            // 什么都不需要做，只需要把网络线程激活即可
            XSF_WARN("Net Shutdown ....");
        }
    };

}



#endif // end of _XSF_NET_MISC_H_

