//////////////////////////////////////////////////////////////////////////
//
// 文件：source/xsf/src/net/ConnectionTCP.h
// 作者：Xoen Xie
// 时间：2023/08/14
// 描述：TCP连接
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _CONNECTION_TCP_H_
#define _CONNECTION_TCP_H_

#include "IXSFNet.h"
#include "XSFQueue.h"
#include "XSFMallocHook.h"
#include "XSFNetDef.h"

namespace xsf
{   
    struct INetPacker;

    class ConnectionTCP : public IConnection
    {
        using SendQueue = queue<NetBuffer>;

        friend class XSFNet;
        friend class NetOperationAdd;
        friend class NetOperationConnect;
        friend class NetOperationSend;
        friend class NetOperationClose;
    public:
        ConnectionTCP(void);
        ~ConnectionTCP(void);

        JEMALLOC_HOOK;

        bool Create(INetPacker * packer);

    public:
        // 监听一个端口
        bool Listen(uint16 nPort) override;

        // 连接一个地址
        bool Connect( const char * sIP, uint16 nPort) override;

        // 发送消息
        bool SendMessage(IMessage * pMessage) override;

        // 发送数据
        bool Send( const byte * pData, uint32 nDataLen ) override;
        
        // 设置事件句柄
        void SetNetHandler( INetHandler * pHandler ) override { m_pNetHandler = pHandler; }

        // 获取事件句柄
        INetHandler * GetNetHandler(void) const override { return m_pNetHandler; }

        // 获取对端IP
        const char * GetRemoteIP(void) override { return m_sIP; }

        // 获取对端连入的端口
        uint32 GetRemotePort(void) override { return m_nPort; }

        // 关闭连接
        void Close(void) override;


        int32 GetSocketID(void) override { return m_nSocketFD; }

    private:
        uint8 EpollConnect(void);
        bool EpollCheckConnect(void);
        void EpollAccept(void);
        bool EpollSetSocket(int32 socket, void * sockaddr);
        void EpollRelease(void);
        bool EpollRecv(void);
        int EpollSend(void);
        bool OnDataRecv(void);

    private:
        void PushEvent(INetEvent * pEvent);

        bool Send( NetBuffer & buffer )
        {
            if (m_nNetStatus == NetStatus_Release)
            {
                buffer.Free();
                return false;
            }

            m_SendQueue.push(buffer);

            return true;
        }

    private:
        int32 m_nSocketFD = INVALID_FD;
        INetHandler * m_pNetHandler = nullptr;
        char m_sIP[MAX_IP_SIZE] = {0};
	    uint32 m_nPort = 0;

        volatile bool m_bWriteEnable = false;
        volatile bool m_bInEpoll = false;

        volatile uint8 m_nNetStatus = NetStatus_None;

        NetBuffer m_Recv;
        NetBuffer m_Data;
        NetBuffer m_Send;		// 发送缓存

        SendQueue m_SendQueue;

        INetPacker * m_Pakcer = nullptr;
    };



} // namespace xsf


#endif      // end of _CONNECTION_TCP_H_