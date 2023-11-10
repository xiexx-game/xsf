//////////////////////////////////////////////////////////////////////////
//
// 文件：base\src\net_point\NetConnector.cpp
// 作者：Xoen Xie
// 时间：2023/08/14
// 描述：类描述
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#include "NetConnector.h"
#include "XSF.h"
#include "XSFMallocHook.h"
#include "XSFServer.h"

namespace xsf
{
    bool NetConnector::Connect( const char * sIP, uint16 nPort )
    {
        strcpy( m_sIP, sIP );

        m_nPort = nPort;

        if( m_pConnection == nullptr )
        {
            m_pConnection = XSFCore::CreateConnection(this, XSFCore::GetServerPacker());
        }

        if( m_pConnection == nullptr )
        {
            FUNCTION_EXIT(1);
            return false;
        }

        return m_pConnection->Connect( m_sIP, m_nPort );
    }

    void NetConnector::OnHandshake(void)
    {
        m_Timers.StartTimer(TimerID_HT, this, XSFCore::GetServer()->GetConfig()->HeartbeatInterval, -1, "NetConnector::OnHandshake");

        m_bHandshake = true;

        OnHandshakeDone();
    }

    void NetConnector::SendMessage( IMessage * pMessage )
    {   
        if( m_pConnection == nullptr )
        {
            XSF_ERROR("NetConnector::SendMessage [%s] m_pConnection == nullptr, msg id=%u", m_sName, pMessage->GetID());
            return;
        }

        m_pConnection->SendMessage(pMessage);
    }

    void NetConnector::SendData( const byte * pData, uint32 nDataLen )
    {
        if( m_pConnection != nullptr )
            m_pConnection->Send(pData, nDataLen);
    }

    bool NetConnector::Init( ModuleInit * pInit )
    {
        XSF_CAST(pLocalInit, pInit, NetConnectorInit);
        m_bNeedReconnect = pLocalInit->NeedReconnect;

        return IModule::Init(pInit);
    }

    void NetConnector::OnClose(void)
    {
        m_bHandshake = false;
        CONNECTION_CLOSE(m_pConnection);
        m_Timers.CloseAll();
        m_bNeedReconnect = false;
    }

    // 接受到连接
    INetHandler * NetConnector::OnAccept(IConnection * pIncomingConn)
    {
        XSF_ERROR("NetConnector::OnAccept illegal call, name:%s", m_sName );
        return nullptr;
    }

    // 已连接到远程主机
    void NetConnector::OnConnected( IConnection * pConection )
    {
        XSF_WARN("Connector[%s] connected", m_sName);
        
        m_Timers.DelTimer(TimerID_Reconnect);
        
        SendHandshake();
    }

    // 连接出错或断开
    void NetConnector::OnError( IConnection * pConection, uint32 nErrorCode )
    {
        XSF_ERROR("Connector[%s] error, code=%u", m_sName, nErrorCode);

        m_Timers.DelTimer(TimerID_HT);
        
        m_bHandshake = false;

        if(m_bNeedReconnect)
            m_Timers.StartTimer(TimerID_Reconnect, this, XSFServer::Instance()->GetConfig()->ReconnectInterval, -1, "NetConnector::OnError");

        OnNetError();
    }

    // 收到数据
    void NetConnector::OnRecv(IConnection * pConection, DataResult * pResult)
    {
        IMessage * pMessage = XSFCore::GetMessage(pResult->nMessageID);
        if(pMessage != nullptr)
        {
            pMessage->Import(pResult->pPBData, pResult->nPBLength);

            pMessage->Execute(this, pMessage, pResult);
        }
        else
        {
            XSF_WARN("NetConnector::OnRecv pMessage is null, id=%u", pResult->nMessageID);
        }
    }


    TIMER_FUNCTION_START(NetConnector)
        (TIMER_CALL)&NetConnector::OnTimerHT,
        (TIMER_CALL)&NetConnector::OnTimerReconnect,
    TIMER_FUNCTION_END


    void NetConnector::OnTimerHT(bool bLastCall)
    {
        SendHeartbeat();
    }

    void NetConnector::OnTimerReconnect(bool bLastCall)
    {
        FUNCTION_ENTER(0);

        m_pConnection = XSFCore::CreateConnection(this, XSFCore::GetServerPacker());
        if( m_pConnection != nullptr &&  m_pConnection->Connect( m_sIP, m_nPort ) )
        {
            m_Timers.DelTimer(TimerID_Reconnect);

            FUNCTION_EXIT(0);
            return;
        }

        FUNCTION_EXIT(1);
    }
}
