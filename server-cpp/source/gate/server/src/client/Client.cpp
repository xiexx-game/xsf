//////////////////////////////////////////////////////////////////////////
//
// 文件：gate\server\src\client\Client.cpp
// 作者：Xoen Xie
// 时间：2020/08/05
// 描述：客户端对象
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#include "Client.h"
#include "DMessageInc.h"
#include "ClientManager.h"
#include "ConnectorManager.h"
#include "ServerConnector.h"

#define DELAY_DISCONNECT    300             // 3秒之后断开客户端

bool Client::Create( IConnection * pConnection )
{
    m_pConnection = pConnection;
    m_nLastHTTime = XSFCore::TickCount();

    m_Timers.StartTimer(TimerID_Check, this, XSFCore::GetServer()->GetConfig()->ClientHeartbeatCheck, -1, "Client::Create");

    for( uint8 i = 0; i < EP_Max; ++ i )
    {
        m_ConnectorIDs[i] = 0;
    }

    return true;
}

void Client::Clear(void)
{
    m_Timers.CloseAll();

    m_bHandshake = false;
    m_nLastHTTime = 0;

    for( uint8 i = 0; i < EP_Max; ++ i )
    {
        m_ConnectorIDs[i] = 0;
    }

    m_SID.ID = 0;
}

void Client::Close(void)
{
    CONNECTION_CLOSE(m_pConnection);
}

void Client::BroadcastClose(void)
{
    XSF_CAST(pMessage, XSFCore::GetMessage(xsf_pbid::Gt_GtA_ClientClose), xsf_msg::MSG_Gt_GtA_ClientClose);
    pMessage->mPB.set_client_id(m_SID.ID);

    for(uint8 i = 0; i < EP_Max; i ++)
    {
        if(m_ConnectorIDs[i] > 0)
        {
            auto pConnector = ConnectorManager::Instance()->GetConnector(i, m_ConnectorIDs[i]);
            if(pConnector != nullptr)
            {
                pConnector->SendMessage(pMessage);
            }
        }
    }
}

void Client::Disconnect(uint32 nReason)
{
    XSF_CAST(pMessage, XSFCore::GetMessage(xsf_pbid::Gt_Clt_Disconnect), xsf_msg::MSG_Gt_Clt_Disconnect);
    pMessage->mPB.set_reason(nReason);
    SendMessage(pMessage);

    m_Timers.StartTimer(TimerID_Disconnect, this, DELAY_DISCONNECT, 1, "Client::Disconnect");
}

void Client::OnHandshake(void)
{
    m_bHandshake = true;
    UpdateHTTime();
}

ServerConnector * Client::GetConnector(uint8 nEP)
{
    if(m_ConnectorIDs[nEP] > 0)
    {
        return ConnectorManager::Instance()->GetConnector(nEP, m_ConnectorIDs[nEP]);
    }
    else
    {
        auto pConnector = ConnectorManager::Instance()->GetConnector(nEP, m_ConnectorIDs[nEP]);
        if(pConnector != nullptr)
        {
            m_ConnectorIDs[nEP] = pConnector->GetRemoteID()->ID;
        }

        return pConnector;
    }
}

void Client::SendMessage( IMessage * pMessage )
{
    if( m_pConnection == nullptr )
        return;

    m_pConnection->SendMessage(pMessage);
}

void Client::SendData( const byte * pData, uint32 nDataLen )
{
    if( m_pConnection != nullptr )
    {
        m_pConnection->Send(pData, nDataLen);
    }
}

// 接受到连接
INetHandler * Client::OnAccept(IConnection * pIncomingConn)
{
    XSF_ERROR("Client::OnAccept illegal call");
    return nullptr;
}

// 已连接到远程主机
void Client::OnConnected( IConnection * pConection )
{
    XSF_ERROR("Client::OnConnected illegal call");
}

// 连接出错或断开
void Client::OnError( IConnection * pConection, uint32 nErrorCode )
{
    XSF_ERROR("Client::OnError nErrorCode=%u, client:%u [%u-%u]", nErrorCode, m_SID.ID, m_SID.C.id, m_SID.C.key);

    BroadcastClose();
    
    ClientManager::Instance()->Delete(this);

    Clear();
}

// 收到数据
void Client::OnRecv(IConnection * pConection, DataResult * pResult)
{
    switch (pResult->nMessageID)
    {
    case xsf_pbid::Clt_Gt_Handshake:
    case xsf_pbid::Clt_Gt_Heartbeat:
        {
            IMessage * pMessage = XSFCore::GetMessage(pResult->nMessageID);
            pMessage->Import(pResult->pPBData, pResult->nPBLength);

            pMessage->Execute(this, pMessage, pResult);
        }
        break;
    
    default:
        {
            if(m_bHandshake)
            {
                IMessage * pMessage = XSFCore::GetMessage(pResult->nMessageID);
                auto pConnector = GetConnector(pMessage->GetEP());
                if(pConnector == nullptr)
                {
                    Disconnect(xsf_pb::DisconnectReason::MsgInvalid);
                    return;
                }

                byte * pRawIDPos = pResult->pRawData + 6;
                memcpy(pRawIDPos, &m_SID.ID, sizeof(uint32));
                
                pConnector->SendData(pResult->pRawData, pResult->nRawLength);
            }
            else
            {
                Disconnect(xsf_pb::DisconnectReason::MsgInvalid);
            }
        }
        break;
    }
}


TIMER_FUNCTION_START(Client)
    (TIMER_CALL)&Client::OnTimerHTCheck,
    (TIMER_CALL)&Client::OnTimerDisconnect,
TIMER_FUNCTION_END


void Client::OnTimerHTCheck(bool bLastCall)
{
    uint32 nNow = XSFCore::TickCount();
    if( nNow > m_nLastHTTime + XSFCore::GetServer()->GetConfig()->ClientHeartbeatTimeout )
    {
        XSF_ERROR("Client::OnTimerHTCheck NetPoint time out, time diff(nNow:%u-m_nLastHTTime:%u):%u, client:%u [%u-%u]", nNow, m_nLastHTTime, nNow-m_nLastHTTime, m_SID.ID, m_SID.C.id, m_SID.C.key);

        BroadcastClose();
        Close();
        ClientManager::Instance()->Delete(this);
        Clear();
    }
}

void Client::OnTimerDisconnect(bool bLastCall)
{
    BroadcastClose();
    Close();
    ClientManager::Instance()->Delete(this);
    Clear();
}

