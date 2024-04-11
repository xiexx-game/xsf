//////////////////////////////////////////////////////////////////////////
//
// 文件：source/xsf/src/net_point/NetPointManager.cpp
// 作者：Xoen Xie
// 时间：2023/08/18
// 描述：网点管理器
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#include "NetPointManager.h"
#include "NetPoint.h"
#include "XSF.h"

#include <algorithm>

namespace xsf
{
    bool INetPointManager::Init( ModuleInit * pInit )
    {
        FUNCTION_ENTER(0);
        IModule::Init(pInit);

        NetPointManagerInit * pLocalInit = (NetPointManagerInit*)pInit;
        m_nListenPort = pLocalInit->nPort;
        m_Packer = pLocalInit->packer;

        return true;
    }

    bool INetPointManager::Start()
    {
        XSF_INFO("INetPointManager::Start name:%s, port=%u", m_sName, GetListenPort());

        m_pConnection = XSFCore::CreateConnection(this, m_Packer != nullptr ? m_Packer : XSFCore::GetServerPacker());
        if( m_pConnection == nullptr )
        {
            XSF_ERROR("INetPointManager::Start name:%s m_pConnection == nullptr", m_sName);
            FUNCTION_EXIT(2);
            return false;
        }

        FUNCTION_EXIT(0);

        return m_pConnection->Listen(GetListenPort());
    }

    void INetPointManager::Release(void)
    {
        FUNCTION_ENTER(0);

        CONNECTION_CLOSE(m_pConnection);

        FUNCTION_EXIT(0);

        delete this;
    }


    bool INetPointManager::DeleteFromWaitList(NetPoint * pPoint)
    {
        FUNCTION_ENTER(0);

        auto it = std::find( m_WaitList.begin(), m_WaitList.end(), pPoint );
        if( it == m_WaitList.end() )
        {
            const SID * pID = pPoint->GetSID();
            XSF_ERROR("INetPointManager::DeleteFromWaitList name:%s, id:%u-%u-%u, it == m_WaitList.end()", m_sName, pID->S.server, pID->S.type, pID->S.index);
            return false;
        }

        m_WaitList.erase(it);

        FUNCTION_EXIT(0);

        return true;
    }

    NetPoint * INetPointManager::NewNP(void)
    {
        return new NetPoint();
    }



    // 接受到连接
    INetHandler * INetPointManager::OnAccept(IConnection * pIncomingConn)
    {
        FUNCTION_ENTER(0);

        if( pIncomingConn == nullptr )
        {
            XSF_ERROR("INetPointManager::OnAccept pIncomingConn == nullptr, name:%s", m_sName);
            FUNCTION_EXIT(1);
            return nullptr;
        }

        if( !CanConnected() )
        {
            FUNCTION_EXIT(2);
            return nullptr;
        }

        XSF_INFO("NetPointManager[%s] accept connection, port=%u", m_sName, m_nListenPort);

        NetPoint * pPoint = NewNP();
        pPoint->Create(this, pIncomingConn);

        m_WaitList.push_back(pPoint);

        FUNCTION_EXIT(0);

        return pPoint;
    }

    // 已连接到远程主机
    void INetPointManager::OnConnected( IConnection * pConection )
    {
        XSF_ERROR("XSFNetPoint::OnConnected illegal call, name:%s", m_sName);
    }

    // 连接出错或断开
    void INetPointManager::OnError( IConnection * pConection, uint32 nErrorCode )
    {
        XSF_ERROR("INetPointManager::OnError name:%s, error:%u", m_sName, nErrorCode);
    }

    // 收到数据
    void INetPointManager::OnRecv(IConnection * pConection, DataResult * pResult)
    {
        XSF_ERROR("INetPointManager::OnRecv illegal call, name:%s", m_sName);
    }





    bool NormalNetPointManager::Add( NetPoint * pPoint )
    {
        FUNCTION_ENTER(0);

        if( !DeleteFromWaitList(pPoint) )
        {
            pPoint->Close();
            delete pPoint;

            FUNCTION_EXIT(1);
            return false;
        }

        if( !m_NetPoints.insert(NetPointMap::value_type(pPoint->m_ID.ID, pPoint)).second )
        {
            pPoint->Close();
            delete pPoint;

            FUNCTION_EXIT(2);
            return false;
        }

        XSF_WARN("Manager[%s] NetPoint login, id:[%s(%u) %u-%u-%u], ip:[%s:%u]", m_sName
            , EP2Name(pPoint->m_ID.S.type), pPoint->m_ID.ID, pPoint->m_ID.S.server, pPoint->m_ID.S.type, pPoint->m_ID.S.index
            , pPoint->m_pConnection->GetRemoteIP(), pPoint->m_pConnection->GetRemotePort() );

        OnNetPointConnected(pPoint);

        FUNCTION_EXIT(0);

        return true;
    }

    void NormalNetPointManager::Delete( NetPoint * pPoint )
    {
        FUNCTION_ENTER(0);

        if( pPoint == nullptr )
        {
            FUNCTION_EXIT(1);
            return;
        }

        if( pPoint->m_ID.ID == 0 )
        {
            DeleteFromWaitList(pPoint);
        }
        else
        {
            m_NetPoints.erase(pPoint->m_ID.ID);

            OnNetPointLost(pPoint);
        }

        XSF_WARN("Manager[%s] NetPoint delete, id:[%s(%u) %u-%u-%u]", m_sName
            , EP2Name(pPoint->m_ID.S.type), pPoint->m_ID.ID, pPoint->m_ID.S.server, pPoint->m_ID.S.type, pPoint->m_ID.S.index );

        FUNCTION_EXIT(0);

        delete pPoint;
    }

    void NormalNetPointManager::Broadcast(IMessage * pMessage, uint32 nSkipID)
    {
        for(auto it = m_NetPoints.begin(); it != m_NetPoints.end(); it ++)
        {
            if(it->second->GetSID()->ID != nSkipID)
            {
                it->second->SendMessage(pMessage);
            }
        }
    }





    bool FastNetPointManager::Init( ModuleInit * pInit )
    {
        FUNCTION_ENTER(0);

        FastNetPointManagerInit * pLocalInit = (FastNetPointManagerInit*)pInit;
        m_nSize = pLocalInit->nMaxSize;

        m_pNetPoints = new NetPoint*[m_nSize];
        memset( m_pNetPoints, 0, sizeof(NetPoint*)*m_nSize);

        FUNCTION_EXIT(0);

        return INetPointManager::Init(pInit);
    }

    void FastNetPointManager::Release(void)
    {
        FUNCTION_ENTER(0);

        if( m_nSize > 0 && m_pNetPoints != nullptr )
        {
            delete [] m_pNetPoints;
            m_pNetPoints = nullptr;
        }

        m_nSize = 0;
        
        FUNCTION_EXIT(0);

        INetPointManager::Release();
    }

    bool FastNetPointManager::Add( NetPoint * pPoint )
    {
        FUNCTION_ENTER(0);

        if( !DeleteFromWaitList(pPoint) )
        {
            pPoint->Close();
            delete pPoint;

            FUNCTION_EXIT(1);
            return false;
        }

        do
        {
            if( m_nCount >= m_nSize )
            {
                XSF_ERROR("Manager[%s] is full, m_nCount:%u >= m_nSize:%u", m_sName, m_nCount, m_nSize );
                break;
            }

            SID & sid = pPoint->m_ID;

            if( sid.S.index >= m_nSize )
            {
                XSF_ERROR("Manager[%s] netpoint ep error, sid.S.index:%u != m_nSize:%u", m_sName, sid.S.index, m_nSize);
                break;
            }

            if( m_pNetPoints[sid.S.index] != nullptr )
            {
                XSF_ERROR("Manager[%s] netpoint ep error, m_pNetPoints[sid.S.index:%u] != nullptr", m_sName, sid.S.index);
                break;
            }

            m_pNetPoints[sid.S.index] = pPoint;

            m_nCount ++;

            XSF_WARN("Manager[%s] NetPoint login, id:[%s(%u) %u-%u-%u], ip:[%s:%u]", m_sName
                , EP2Name(pPoint->m_ID.S.type), pPoint->m_ID.ID, pPoint->m_ID.S.server, pPoint->m_ID.S.type, pPoint->m_ID.S.index
                , pPoint->m_pConnection->GetRemoteIP(), pPoint->m_pConnection->GetRemotePort() );


            OnNetPointConnected(pPoint);

            FUNCTION_EXIT(1);

            return true;

        } while (false);
        
        pPoint->Close();
        delete pPoint;

        FUNCTION_EXIT(0);

        return false;
    }

    void FastNetPointManager::Delete( NetPoint * pPoint )
    {
        FUNCTION_ENTER(0);

        if( pPoint == nullptr )
        {
            FUNCTION_EXIT(1);
            return;
        }

        if( pPoint->m_ID.ID == 0 )
        {
            DeleteFromWaitList(pPoint);
        }
        else
        {
            uint32 nIndex = pPoint->m_ID.S.index;

            if( nIndex >= m_nSize )
            {
                XSF_WARN("FastNetPointManager::Delete nIndex:%u >= m_nSize:%u, name:%s", nIndex, m_nSize, m_sName);
                return;
            }

            if( m_pNetPoints[nIndex] != pPoint )
            {
                XSF_WARN("FastNetPointManager::Delete m_pNetPoints[nIndex] != pPoint, name:%s", m_sName);
                return;
            }

            m_pNetPoints[nIndex] = nullptr;

            -- m_nCount;

            OnNetPointLost(pPoint);
        }

        XSF_WARN("Manager[%s] NetPoint delete, id:[%s(%u) %u-%u-%u]", m_sName
            , EP2Name(pPoint->m_ID.S.type), pPoint->m_ID.ID, pPoint->m_ID.S.server, pPoint->m_ID.S.type, pPoint->m_ID.S.index );

        delete pPoint;

        FUNCTION_EXIT(0);
    }

    void FastNetPointManager::Broadcast(IMessage * pMessage, uint32 nSkipID)
    {
        for(uint32 i = 0; i < m_nSize; i ++)
        {
            if(m_pNetPoints[i] != nullptr && m_pNetPoints[i]->GetSID()->ID != nSkipID)
            {
                m_pNetPoints[i]->SendMessage(pMessage);
            }
        }
    }


}

