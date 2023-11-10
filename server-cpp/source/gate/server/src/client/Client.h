//////////////////////////////////////////////////////////////////////////
//
// 文件：gate\server\src\client\Client.h
// 作者：Xoen Xie
// 时间：2020/08/05
// 描述：客户端对象
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _CLIENT_H_
#define _CLIENT_H_

#include "XSF.h"
using namespace xsf;

class ServerConnector;

class Client : public INetHandler, public ITimerHandler
{
    friend class ClientManager;
    friend class ConnectorModule;

    TIMER_ID_START
        TimerID_Check,
        TimerID_Disconnect,
    TIMER_ID_END
public:
    Client(void) 
        : m_Timers(TimerID_Max)
    {
        m_SID.ID = 0;
    }

    ~Client(void) {}

    const SID * GetSID(void) { return &m_SID; }

    bool Create( IConnection * pConnection );

    void Clear(void);

    void Close(void);

    void BroadcastClose(void);

    void Disconnect(uint32 nReason);

    void OnHandshake(void);

    ServerConnector * GetConnector(uint8 nEP);

    void SendMessage( IMessage * pMessage );

    void SendData( const byte * pData, uint32 nDataLen );

    void SetServerID( uint8 nEP, uint32 nServerID )
    {
        m_ConnectorIDs[nEP] = nServerID;
    }

    void UpdateHTTime(void)
    {
        m_nLastHTTime = XSFCore::TickCount();
    }
    
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
    void OnTimerHTCheck(bool bLastCall);
    void OnTimerDisconnect(bool bLastCall);

private:
    TimerManager m_Timers;

    IConnection * m_pConnection = nullptr;

    SID m_SID;

    bool m_bHandshake = false;
    uint32 m_nLastHTTime = 0;

private:
    uint32 m_ConnectorIDs[EP_Max] = {0};
};





#endif      // end of _CLIENT_H_