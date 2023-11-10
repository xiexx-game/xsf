//////////////////////////////////////////////////////////////////////////
//
// 文件：base\src\net_point\XSFNetPoint.cpp
// 作者：Xoen Xie
// 时间：2023/08/14
// 描述：网络节点对象
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#include "NetPoint.h"
#include "XSFLog.h"
#include "NetPointManager.h"
#include "XSF.h"
#include "XSFServer.h"

namespace xsf
{
    bool NetPoint::Create( INetPointManager * pOwner, IConnection * pConnection )
    {
        FUNCTION_ENTER(0);
        m_pOwner = pOwner;
        m_pConnection = pConnection;

        m_nLastHTTime = XSFCore::TickCount();

        m_Timers.StartTimer(TimerID_HTCheck, this, XSFServer::Instance()->GetConfig()->HeartbeatCheck, -1, "NetPoint::Create");

        FUNCTION_EXIT(0);

        return true;
    }

    void NetPoint::Close(void)
    {
        CONNECTION_CLOSE(m_pConnection);
        m_Timers.CloseAll();
    }

    void NetPoint::SendMessage( IMessage * pMessage )
    {
        if( m_pConnection == nullptr )
        {
            return;
        }

        m_pConnection->SendMessage(pMessage);
    }

    void NetPoint::SendData( const byte * pData, uint32 nDataLen )
    {
        if( m_pConnection != nullptr )
            m_pConnection->Send(pData, nDataLen);
    }

    void NetPoint::UpdateHTTime(void)
    {
        m_nLastHTTime = XSFCore::TickCount();
    }

    const char * NetPoint::GetRemoteIP(void)
    {
        return m_pConnection->GetRemoteIP();
    }




    // 接受到连接
    INetHandler * NetPoint::OnAccept(IConnection * pIncomingConn)
    {
        XSF_ERROR("XSFNetPoint::OnAccept illegal call, owner name:%s", m_pOwner->GetModuleName() );

        return nullptr;
    }

    // 已连接到远程主机
    void NetPoint::OnConnected( IConnection * pConection )
    {
        XSF_ERROR("XSFNetPoint::OnConnected illegal call, owner name:%s", m_pOwner->GetModuleName() );
    }

    // 连接出错或断开
    void NetPoint::OnError( IConnection * pConection, uint32 nErrorCode )
    {
        XSF_ERROR("NetPoint[%s], id:[%u|%u-%u-%u] error", m_pOwner->GetModuleName(), m_ID.ID, m_ID.S.server, m_ID.S.type, m_ID.S.index );
        
        m_Timers.CloseAll();

        m_pOwner->Delete(this);
    }

    // 收到数据
    void NetPoint::OnRecv(IConnection * pConection, DataResult * pResult)
    {
        IMessage * pMessage = XSFCore::GetMessage(pResult->nMessageID);
        if(pMessage != nullptr)
        {
            pMessage->Import(pResult->pPBData, pResult->nPBLength);

            pMessage->Execute(this, pMessage, pResult);
        }
        else
        {
            XSF_WARN("NetPoint::OnRecv pMessage is null, id=%u", pResult->nMessageID);
        }

        UpdateHTTime();
    }


    TIMER_FUNCTION_START(NetPoint)
        (TIMER_CALL)&NetPoint::OnTimerHTCheck,
    TIMER_FUNCTION_END


    void NetPoint::OnTimerHTCheck(bool bLastCall)
    {
        uint32 nNow = XSFCore::TickCount();
        if( nNow > m_nLastHTTime + XSFServer::Instance()->GetConfig()->HeartbeatTimeout )
        {
            XSF_ERROR("NetPoint::OnTimerHTCheck NetPoint time out, time diff(nNow:%u-m_nLastHTTime:%u):%u, owner name:%s", nNow, m_nLastHTTime, nNow-m_nLastHTTime, m_pOwner->GetModuleName());
            Close();

            m_pOwner->Delete(this);
        }
    }

}