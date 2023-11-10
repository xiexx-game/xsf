//////////////////////////////////////////////////////////////////////////
//
// 文件：base\src\ConnectionTCP.cpp
// 作者：Xoen Xie
// 时间：2023/08/22
// 描述：TCP连接
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#include "ConnectionTCP.h"
#include "XSFLog.h"
#include "XSFNet.h"
#include "XSF.h"
#include "XSFNetMisc.h"
#include "XSFServer.h"
#include "XSFBytesOps.h"

#include <errno.h>
#include <netinet/tcp.h>

#define BACKLOG 32

namespace xsf
{
    ConnectionTCP::ConnectionTCP(void)
    {

    }

    ConnectionTCP::~ConnectionTCP(void)
    {

    }

    bool ConnectionTCP::Create(INetPacker * packer)
    {
        NET_FUNCTION_ENTER(0, m_nSocketFD);

        m_Pakcer = packer;

        m_Recv.Malloc(MAX_NET_BUFFER);
        m_Send.Malloc(MAX_NET_BUFFER);


        NET_FUNCTION_EXIT(0, m_nSocketFD);

        return true;
    }

    // 监听一个端口
    bool ConnectionTCP::Listen( uint16 nPort )
    {
        NET_FUNCTION_ENTER(0, m_nSocketFD);

        XSF_INFO("ConnectionTCP::Listen port=%u", nPort);

        // 获取地址信息
        struct addrinfo addrIn;
        memset(&addrIn, 0, sizeof(addrIn));
        addrIn.ai_family = AF_UNSPEC;
        addrIn.ai_socktype = SOCK_STREAM;
        addrIn.ai_protocol = IPPROTO_TCP;

        char sPort[16];
        sprintf(sPort, "%d", nPort);
        struct addrinfo * pAddrOut = nullptr;
        int32 nStatus = getaddrinfo("0.0.0.0", sPort, &addrIn, &pAddrOut);
        if (nStatus != 0)
        {
            XSF_ERROR("ConnectionTCP::Listen getaddrinfo error, nStatus=%d, %s", nStatus, gai_strerror(nStatus) );
            return false;
        }

        // 创建套接字
        m_nSocketFD = socket(pAddrOut->ai_family, pAddrOut->ai_socktype, 0);
        if (m_nSocketFD < 0) 
        {
            freeaddrinfo(pAddrOut);
            XSF_ERROR("ConnectionTCP::Listen socket create error");
            return false;            
        }

        do
        {
            // 这里需要设置一下socket，因为正常关闭，socket会有一个TIME_WAIT时间，会导致无法再一次bind
            int32 reuse = 1;
            if ( setsockopt(m_nSocketFD, SOL_SOCKET, SO_REUSEADDR, (void *)&reuse, sizeof(int)) == -1 )
            {
                XSF_ERROR("ConnectionTCP::Listen SO_REUSEADDR error, errno=%d, %s", errno, strerror(errno) );
                break;
            }

            nStatus = bind(m_nSocketFD, (struct sockaddr *)pAddrOut->ai_addr, pAddrOut->ai_addrlen);
            if (nStatus != 0)
            {
                XSF_ERROR("ConnectionTCP::Listen bind error, errno=%d, %s", errno, strerror(errno) );
                break;
            }

            nStatus = listen(m_nSocketFD, BACKLOG);
            if (nStatus != 0)
            {
                XSF_ERROR("ConnectionTCP::Listen listen error, errno=%d, %s", errno, strerror(errno));
                break;
            }

            freeaddrinfo(pAddrOut);

            // 改变状态
            m_nNetStatus = NetStatus_Accept;
            
            XSFServer::Instance()->GetNet()->PushOperation(new NetOperationAdd(this));

            NET_FUNCTION_EXIT(1, m_nSocketFD);

            //XSF_ERROR("ConnectionTCP::Listen addr=%lu, handler=%lu", this, m_pNetHandler);

            return true;

        } while (false);

        close(m_nSocketFD);
        m_nSocketFD = INVALID_FD;
        freeaddrinfo(pAddrOut);

        NET_FUNCTION_EXIT(0, m_nSocketFD);

        return false;
    }

    // 连接一个地址
    bool ConnectionTCP::Connect( const char * sIP, uint16 nPort)
    {
        NET_FUNCTION_ENTER(0, m_nSocketFD);

        if( m_nNetStatus == NetStatus_Connecting || m_nNetStatus == NetStatus_Work )
        {
            NET_FUNCTION_EXIT(2, m_nSocketFD);
            return true;
        }

        m_nNetStatus = NetStatus_Connecting;

        if( nullptr == sIP || nPort == 0 )
        {
            NET_FUNCTION_EXIT(1, m_nSocketFD);
            return false;
        }

        strcpy( m_sIP, sIP);
        m_nPort = nPort;

        XSF_INFO("ConnectionTCP::Connect IP=%s, port=%u, addr=%lu, handler=%lu", m_sIP, m_nPort, this, m_pNetHandler);
        XSFServer::Instance()->GetNet()->PushOperation(new NetOperationConnect(this));

        NET_FUNCTION_EXIT(0, m_nSocketFD);

        return true;    
    }

    // 发送消息
    bool ConnectionTCP::SendMessage(IMessage * pMessage)
    {
        NET_FUNCTION_ENTER(0, m_nSocketFD);

        if (m_nNetStatus != NetStatus_Work)
        {
            XSF_ERROR("ConnectionTCP::SendMessage m_nNetStatus:%u != NetStatus_Work, m_nSocketFD=%u", m_nNetStatus, m_nSocketFD);
            return false;
        }
        
        uint32 nLength = 0;
        byte * pData = m_Pakcer->Pack(pMessage, nLength);

        //XSF_INFO("ConnectionTCP::SendMessage nLength=%u", nLength);

        NetBuffer buffer;
        buffer.Malloc(nLength);
        buffer.Push(pData, nLength);

        XSFServer::Instance()->GetNet()->PushOperation(new NetOperationSend(this, buffer));

        return true;
    }

    // 发送数据
    bool ConnectionTCP::Send( const byte * pData, uint32 nDataLen )
    {
        NET_FUNCTION_ENTER(0, m_nSocketFD);

        if (m_nNetStatus != NetStatus_Work)
        {
            XSF_ERROR("ConnectionTCP::Send m_nNetStatus:%u != NetStatus_Work, m_nSocketFD=%u", m_nNetStatus, m_nSocketFD);
            return false;
        }   

        if (pData == nullptr || nDataLen <= 0)
        {
            XSF_ERROR("ConnectionTCP::Send pData == nullptr || nDataLen <= 0, m_nSocketFD=%u", m_nSocketFD);
            return false;
        }

        NetBuffer buffer;
        buffer.Malloc(nDataLen);
        buffer.Push((byte*)pData, nDataLen);

        XSFServer::Instance()->GetNet()->PushOperation(new NetOperationSend(this, buffer));

        NET_FUNCTION_EXIT(0, m_nSocketFD);

        return true;
    }

    // 关闭连接
    void ConnectionTCP::Close(void)
    {
        NET_FUNCTION_ENTER(0, m_nSocketFD);

        m_nNetStatus = NetStatus_Release;
        m_pNetHandler = nullptr;

        //XSF_ERROR("ConnectionTCP::Close addr=%lu", this);
        XSFServer::Instance()->GetNet()->PushOperation(new NetOperationClose(this));

        NET_FUNCTION_EXIT(0, m_nSocketFD);
    }







    uint8 ConnectionTCP::EpollConnect(void)
    {
        NET_FUNCTION_ENTER(0, m_nSocketFD);

        addrinfo addrIn;
        memset(&addrIn, 0, sizeof(addrIn));
        addrIn.ai_family = AF_UNSPEC;
        addrIn.ai_socktype = SOCK_STREAM;
        addrIn.ai_protocol = IPPROTO_TCP;

        addrinfo *pAddrOut = nullptr;
        char sPort[16];
        sprintf(sPort, "%d", m_nPort);
        int32 nStatus = getaddrinfo(m_sIP, sPort, &addrIn, &pAddrOut);
        if (nStatus != 0)
        {
            XSF_ERROR("ConnectionTCP::EpollConnect getaddrinfo error nStatus=%d, %s, ip=%s", nStatus, gai_strerror(nStatus), m_sIP);
            return ConnectResult_Error;
        }

        addrinfo * pAddr = nullptr;
        for (pAddr = pAddrOut; pAddr != nullptr; pAddr = pAddr->ai_next)
        {
            m_nSocketFD = socket(pAddr->ai_family, pAddr->ai_socktype, pAddr->ai_protocol);
            if (m_nSocketFD < 0)
            {
                continue;
            }

            FDNonblocking(m_nSocketFD);

            int32 v = 1;
            setsockopt(m_nSocketFD, IPPROTO_TCP, TCP_NODELAY, &v, sizeof(v));

            nStatus = connect(m_nSocketFD, pAddr->ai_addr, pAddr->ai_addrlen);
            if (nStatus != 0 && errno != EINPROGRESS)
            {
                close(m_nSocketFD);
                m_nSocketFD = INVALID_FD;
                continue;
            }

            break;
        }

        uint8 nRes = ConnectResult_OK;
        do
        {
            if (m_nSocketFD < 0)
            {
                XSF_ERROR("ConnectionTCP::EpollConnect m_nSocketFD < 0");
                nRes = ConnectResult_Error;
                NET_FUNCTION_EXIT(1, m_nSocketFD);
                break;
            }

            if (nStatus == 0)
            {
                NET_FUNCTION_ENTER(1, m_nSocketFD);
                struct sockaddr * addr = pAddr->ai_addr;
                void * sin_addr = (pAddr->ai_family == AF_INET) ? (void*)&((struct sockaddr_in *)addr)->sin_addr : (void*)&((struct sockaddr_in6 *)addr)->sin6_addr;
                inet_ntop(pAddr->ai_family, sin_addr, m_sIP, sizeof(m_sIP));
            }
            else
            {
                NET_FUNCTION_ENTER(2, m_nSocketFD);
                nRes = ConnectResult_Wait;
            }

        } while (false);

        freeaddrinfo(pAddrOut);

        NET_FUNCTION_EXIT(0, m_nSocketFD);

        return nRes;
    }

    bool ConnectionTCP::EpollCheckConnect(void)
    {
        NET_FUNCTION_ENTER(0, m_nSocketFD);

        if (m_nNetStatus == NetStatus_Release)
        {
            NET_FUNCTION_EXIT(1, m_nSocketFD);
            return false;
        }
            
        int32 error = 0;
        socklen_t len = sizeof(error);
        int code = getsockopt(m_nSocketFD, SOL_SOCKET, SO_ERROR, &error, &len);
        if (code < 0 || error)
        {
            PushEvent(new NetEventError(this, NetCode_ErrorConnect));
            NET_FUNCTION_EXIT(2, m_nSocketFD);
            return false;
        }

        m_nNetStatus = NetStatus_Work;

        sockaddr_all u;
        socklen_t slen = sizeof(u);
        if (getpeername(m_nSocketFD, &u.s, &slen) == 0)
        {
            void * sin_addr = (u.s.sa_family == AF_INET) ? (void*)&u.v4.sin_addr : (void *)&u.v6.sin6_addr;
            inet_ntop(u.s.sa_family, sin_addr, m_sIP, sizeof(m_sIP));
        }

        // 通知主线程，连接成功
        PushEvent(new NetEventConnected(this));

        NET_FUNCTION_EXIT(0, m_nSocketFD);

        return true;        
    }

    void ConnectionTCP::EpollAccept(void)
    {
        NET_FUNCTION_ENTER(0, m_nSocketFD);

        if (m_nNetStatus != NetStatus_Accept)
        {
            NET_FUNCTION_EXIT(1, m_nSocketFD);
            return;
        }

        sockaddr_all u;
        socklen_t len = sizeof(u);
        int nNewFD = accept(m_nSocketFD, &u.s, &len);
        if (nNewFD < 0)
        {
            if (errno == EMFILE || errno == ENFILE) 
            {
                XSF_ERROR("ConnectionTCP::EpollAccept errno == EMFILE || errno == ENFILE, errno=%d, %s", errno, strerror(errno) );
            }
            else
            {
                XSF_ERROR("ConnectionTCP::EpollAccept accept error, errno=%d, %s", errno, strerror(errno) );
                PushEvent(new NetEventError(this, NetCode_ErrorAccept));    
            }

            NET_FUNCTION_EXIT(2, m_nSocketFD);
            
            return;
        }

        FDNonblocking(nNewFD);
        int32 v = 1;
        setsockopt(nNewFD, IPPROTO_TCP, TCP_NODELAY, &v, sizeof(v));

        ConnectionTCP * pNewConnection = new ConnectionTCP();
        //XSF_ERROR("ConnectionTCP::EpollAccept pNewConnection=%lu", pNewConnection);
        if (!pNewConnection->Create(m_Pakcer) || !pNewConnection->EpollSetSocket(nNewFD, &u) )
        {
            pNewConnection->EpollRelease();

            NET_FUNCTION_EXIT(3, m_nSocketFD);
            return;
        }

        PushEvent(new NetEventAccept(this, pNewConnection));

        NET_FUNCTION_EXIT(0, m_nSocketFD);
    }

    bool ConnectionTCP::EpollSetSocket(int32 socket, void * sockaddr)
    {
        NET_FUNCTION_ENTER(0, m_nSocketFD);

        m_nSocketFD = socket;
        
        sockaddr_all * pU = (sockaddr_all*)sockaddr;

        void * sin_addr = (pU->s.sa_family == AF_INET) ? (void*)&(pU->v4.sin_addr) : (void *)&(pU->v6.sin6_addr);
        m_nPort = ntohs((pU->s.sa_family == AF_INET) ? pU->v4.sin_port : pU->v6.sin6_port);

        inet_ntop(pU->s.sa_family, sin_addr, m_sIP, sizeof(m_sIP));

        if( !XSFServer::Instance()->GetNet()->EpollSocketAdd(this) )
        {
            m_nNetStatus = NetStatus_Release;
            XSF_ERROR("ConnectionTCP::EpollSetSocket epoll add error");
            return false;
        }

        m_nNetStatus = NetStatus_Work;

        NET_FUNCTION_EXIT(0, m_nSocketFD);

        return true;
    }

    void ConnectionTCP::EpollRelease(void)
    {
        NET_FUNCTION_ENTER(0, m_nSocketFD);

        m_Recv.Free();
        m_Data.Free();
        m_Send.Free();

        
        while( !m_SendQueue.empty() )
        {
            NetBuffer & buffer = m_SendQueue.front();
            buffer.Free();

            m_SendQueue.pop();
        }

        if(m_nSocketFD != INVALID_FD)
        {
            close(m_nSocketFD);
            m_nSocketFD = INVALID_FD;
        }
        
        NET_FUNCTION_EXIT(0, m_nSocketFD);

        //XSF_ERROR("ConnectionTCP::EpollRelease addr=%lu", this);

        delete this;
    }

    bool ConnectionTCP::EpollRecv(void)
    {
        NET_FUNCTION_ENTER(0, m_nSocketFD);

        if (m_nNetStatus != NetStatus_Work)
            return false;

        bool bRes = true;
        uint32 nSizeLeft = 0;
        byte * pCurPoint = m_Recv.EmptyBuffer(nSizeLeft);

        int32 nCurRecvSize = read(m_nSocketFD, pCurPoint, nSizeLeft);

        //XSF_INFO("ConnectionTCP::EpollRecv 1 nSizeLeft=%u, nCurRecvSize=%d", nSizeLeft, nCurRecvSize);

        if (nCurRecvSize < 0)
        {
            switch (errno)
            {
            case EINTR:
                bRes = true;
                break;

            case AGAIN_WOULDBLOCK:
                XSF_WARN("ConnectionTCP::EpollRecv : socket-server: EAGAIN capture");
                bRes = true;
                break;

            default:
                XSF_ERROR("ConnectionTCP::EpollRecv errno=%d, errors=%s", errno, strerror(errno));
                PushEvent(new NetEventError(this, NetCode_ErrorRecv));
                bRes = false;
                break;
            }

            NET_FUNCTION_EXIT(2, m_nSocketFD);
            return bRes;
        }
        else if (nCurRecvSize == 0)
        {
            PushEvent(new NetEventError(this, NetCode_Disconnect));
            NET_FUNCTION_EXIT(3, m_nSocketFD);
            return false;
        }

        m_Recv.m_nTail += nCurRecvSize;

        //XSF_INFO("ConnectionTCP::EpollRecv 2 nCurRecvSize=%u, m_Recv.m_nTail=%u", nCurRecvSize, m_Recv.m_nTail);
        
        bRes = OnDataRecv();

        NET_FUNCTION_EXIT(0, m_nSocketFD);

        return bRes;
    }

    int ConnectionTCP::EpollSend(void)
    {
        NET_FUNCTION_ENTER(0, m_nSocketFD);

        while( true )
        {
            // 当前发送buffer已经满了，不再补充
            uint32 nSendMax = 0;
            m_Send.EmptyBuffer(nSendMax);
            if( nSendMax <= 0 )
                break;

            if( m_SendQueue.empty() )   // 发送队列中没有数据了
                break;

            // 取出第一个包
            NetBuffer & current = m_SendQueue.front();
            uint32 nSendSize = 0;
            byte * pHeadBuffer = current.HeadBuffer(nSendSize);

            if( nSendMax >= nSendSize )    // 如果当前的空余大于本次添加的包大小，直接放入，并删除
            {
                m_Send.Push(pHeadBuffer, nSendSize);
                current.Free();
                m_SendQueue.pop();
            }
            else
            {
                m_Send.Push(pHeadBuffer, nSendMax);
                current.AddHead(nSendMax);
                break;
            }
        }

        int32 nSendSize = write(m_nSocketFD, m_Send.m_pBuffer, m_Send.m_nTail);
        if( nSendSize < 0 )
        {
            NET_FUNCTION_ENTER(1, m_nSocketFD);

            switch (errno)
            {
            case EINTR:
            case AGAIN_WOULDBLOCK:
                {
                    NET_FUNCTION_EXIT(1, m_nSocketFD);
                    return SendResult_Run;
                }
            }

            m_nNetStatus = NetStatus_Error;
            XSF_ERROR("ConnectionTCP::EpollSend Error, errno=%d, errors=%s", errno, strerror(errno));

            PushEvent(new NetEventError(this, NetCode_ErrorSend));

            NET_FUNCTION_EXIT(2, m_nSocketFD);

            return SendResult_Error;
        }

        if ((uint32)nSendSize < m_Send.m_nTail )		// 说明还有数据没有发送完毕
        {
            m_Send.m_nTail -= nSendSize;
			memmove( m_Send.m_pBuffer, m_Send.m_pBuffer + nSendSize, m_Send.m_nTail );
            
            NET_FUNCTION_EXIT(3, m_nSocketFD);

            return SendResult_Run;
        }
        else
        {
            NET_FUNCTION_ENTER(2, m_nSocketFD);
            m_Send.Reset();

            if( m_SendQueue.empty() )
            {
                NET_FUNCTION_EXIT(4, m_nSocketFD);
                return SendResult_Done;
            }
            else
            {
                NET_FUNCTION_EXIT(5, m_nSocketFD);
                return SendResult_Run;
            }
        }
    }

    void ConnectionTCP::PushEvent(INetEvent * pEvent)
    {
        if (m_nNetStatus == NetStatus_Release)
        {
            pEvent->Release();
            return;
        }

        XSFServer::Instance()->GetNet()->PushEvent(pEvent);
    }

    bool ConnectionTCP::OnDataRecv(void)
    {
        NET_FUNCTION_ENTER(0, m_nSocketFD);

        #define SIZE_UINT32     (uint32)sizeof(uint32)
        #define SIZE_UINT8      (uint32)sizeof(uint8)

        byte * pBufferStart = m_Recv.m_pBuffer;
        uint32 nSizeLeft = m_Recv.m_nTail;

        while( true )
        {
            //XSF_INFO("m_Data.m_nSize=%u", m_Data.m_nSize);
            if( m_Data.m_nSize <= 0 )       // 等待一个新的包
            {
                if( nSizeLeft <= SIZE_UINT32 )      // 数据还不够或仅够一个头数据，则需要等待下一次的数据接收内容
                {
                    NET_FUNCTION_ENTER(1, m_nSocketFD);

                    if(pBufferStart != m_Recv.m_pBuffer)
                    {
                        memmove( m_Recv.m_pBuffer, pBufferStart, nSizeLeft);
                        m_Recv.m_nTail = nSizeLeft;
                    }
                    break;
                }

                // 如果接收的数据大于4个字节，说明已经有接收到实际的网络协议包数据
                // 先读出下一个数据协议包的大小
                uint32 nDataLength = xsf_byte::ReadUint32(pBufferStart);
                if( nDataLength < m_Pakcer->GetMinLength() || nDataLength > m_Pakcer->GetMaxLength() )
                {
                    XSF_ERROR("ConnectionTCP::OnDataRecv Data length error, nDataLength[%u], min:[%u] max:[%u]", nDataLength, m_Pakcer->GetMinLength(), m_Pakcer->GetMaxLength());
                    PushEvent(new NetEventError(this, NetCode_ErrorHeadRecv));
                    return false;
                }

                m_Data.Malloc(SIZE_UINT32 + nDataLength);
                m_Data.Push(pBufferStart, SIZE_UINT32);

                //XSF_INFO("ConnectionTCP::OnDataRecv 1 nDataLength=%u", nDataLength);
            
                pBufferStart += SIZE_UINT32;
                nSizeLeft -= SIZE_UINT32;
            }
            else
            {
                uint32 nDataNeed = m_Data.m_nSize - m_Data.m_nTail;

                //XSF_INFO("ConnectionTCP::OnDataRecv 2 nDataNeed=%u, nSizeLeft=%u", nDataNeed, nSizeLeft);

                if (nSizeLeft < nDataNeed)	// 如果剩下的数据比实际包数据小，则需要等待接收剩余包数据
                {
                    NET_FUNCTION_ENTER(1000, m_nSocketFD);
                    m_Data.Push(pBufferStart, nSizeLeft);

                    m_Recv.Reset();

                    NET_FUNCTION_EXIT(1, m_nSocketFD);
                    break;
                }
                else if (nSizeLeft == nDataNeed)		// 如果刚好相等，则这个包刚好收完
                {
                    NET_FUNCTION_ENTER(1001, m_nSocketFD);
                    m_Data.Push(pBufferStart, nSizeLeft);

                    DataResult result;
                    if(m_Pakcer->Read(m_Data.m_pBuffer, m_Data.m_nTail, &result))
                    {
                        PushEvent(new NetEventRecv(this, m_Data, result));
                    }
                    else
                    {
                        XSF_ERROR("ConnectionTCP::OnDataRecv m_Pakcer->Read error");
                        PushEvent(new NetEventError(this, NetCode_ErrorHeadRecv));
                        m_Data.Free();
                        return false;
                    }

                    m_Recv.Reset();
                    m_Data.Clear();

                    NET_FUNCTION_EXIT(2, m_nSocketFD);
                    break;
                }
                else  // 剩下的数据比这个包数据大，至少包含了两个包的数据
                {
                    NET_FUNCTION_ENTER(1002, m_nSocketFD);
                    m_Data.Push(pBufferStart, nDataNeed);

                    DataResult result;
                    if(m_Pakcer->Read(m_Data.m_pBuffer, m_Data.m_nTail, &result))
                    {
                        PushEvent(new NetEventRecv(this, m_Data, result));
                    }
                    else
                    {
                        XSF_ERROR("ConnectionTCP::OnDataRecv m_Pakcer->Read error");
                        PushEvent(new NetEventError(this, NetCode_ErrorHeadRecv));
                        m_Data.Free();
                        return false;
                    }

                    m_Data.Clear();

                    pBufferStart += nDataNeed;
                    nSizeLeft -= nDataNeed;

                    //XSF_INFO("ConnectionTCP::OnDataRecv 3, nSizeLeft=%u", nSizeLeft);

                    // 如果剩下的数据还不够一个包头，等待下一个数据接收
                    if (nSizeLeft <= SIZE_UINT32)
                    {
                        memmove( m_Recv.m_pBuffer, pBufferStart, nSizeLeft);
                        m_Recv.m_nTail = nSizeLeft;

                        NET_FUNCTION_EXIT(3, m_nSocketFD);
                        break;
                    }
                }
            }
        }

	    NET_FUNCTION_EXIT(0, m_nSocketFD);

        return true;
    }










    
} // namespace xsf


