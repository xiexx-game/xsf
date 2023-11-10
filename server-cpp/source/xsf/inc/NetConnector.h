//////////////////////////////////////////////////////////////////////////
//
// 文件：source/xsf/inc/NetConnector.h
// 作者：Xoen Xie
// 时间：2023/08/14
// 描述：网络连接器
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _NET_CONNECTOR_H_
#define _NET_CONNECTOR_H_

#include "XSFDef.h"
#include "IXSFServer.h"
#include "IXSFNet.h"
#include "XSFMallocHook.h"
#include "IXSFTimer.h"
#include "IXSFMessage.h"
#include "StreamReader.h"

namespace xsf
{
    struct NetConnectorInit : public ModuleInit
    {
        bool NeedReconnect;
    };

    class NetConnector : public IModule, public INetHandler, public ITimerHandler
    {
        TIMER_ID_START
            TimerID_HT,
            TimerID_Reconnect,
        TIMER_ID_END

    public:
        NetConnector(void)
            : m_Timers(TimerID_Max)
        {
            m_RemoteID.ID = 0;
        }

        virtual ~NetConnector(void) {}
        
        JEMALLOC_HOOK;

        const SID * GetRemoteID(void) { return &m_RemoteID; }
        void SetRemoteID( uint32 nID ) { m_RemoteID.ID = nID; }

        bool Connect( const char * sIP, uint16 nPort );

        void OnHandshake(void);

        void SendMessage( IMessage * pMessage );

        void SendData( const byte * pData, uint32 nDataLen );

        bool IsHandshake(void) { return m_bHandshake; }

    public:
        virtual bool Init( ModuleInit * pInit ) override;
        virtual void OnClose(void) override;

    public:
        // 接受到连接
        INetHandler * OnAccept(IConnection * pIncomingConn) override;

        // 已连接到远程主机
        void OnConnected( IConnection * pConection ) override;

        // 连接出错或断开
        void OnError( IConnection * pConection, uint32 nErrorCode ) override;

        // 收到数据
        virtual void OnRecv(IConnection * pConection, DataResult * pResult) override;

    public:
        TIMER_FUNCTION_DEFINE;

    private:
        void OnTimerHT(bool bLastCall);

        void OnTimerReconnect(bool bLastCall);

    protected:
        virtual void SendHandshake(void) = 0;
        virtual void SendHeartbeat(void) = 0;

        virtual void OnHandshakeDone(void) {}
        virtual void OnNetError(void) {}

    protected:
        SID m_RemoteID;

        uint8 m_nDestEP = 0;
        char m_sIP[MAX_IP_SIZE] = {0};
        uint16 m_nPort = 0;

        IConnection * m_pConnection = nullptr;

        TimerManager m_Timers;

        bool m_bHandshake = false;
        bool m_bNeedReconnect = false;
    };
}


#endif      // end of _NET_CONNECTOR_H_