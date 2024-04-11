//////////////////////////////////////////////////////////////////////////
//
// 文件：source/gate/server/src/client/ClientManager.cpp
// 作者：Xoen Xie
// 时间：2020/08/05
// 描述：client 管理
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#include "ClientManager.h"
#include "DMessageInc.h"
#include "Client.h"
#include "ClientExecutors.h"
#include "LocalPacker.h"
#include "Server.h"


Client * ClientManager::New(void)
{
    if( m_nTotal >= m_nSize )
    {
        XSF_WARN("ClientManager::New m_nCount[%u] >= m_nSize[%u]", m_nTotal, m_nSize);
        return nullptr;
    }

    Client * pNewClient = nullptr;
    uint32 nID = 0xFFFFFFFF;

    for( uint32 i = 0; i < m_nSize; ++ i )
    {
        if( m_Clients[i] == nullptr )       // 当前位置为空，直接可用
        {
            Client * pClient = new Client();
            m_Clients[i] = pClient;
            pNewClient = pClient;
            nID = i;
            break;
        }
        else
        {
            Client * pClient = m_Clients[i];
            if( pClient->m_SID.ID == 0 )         // 如果ID为0，则表示当前Client已经被释放掉
            {
                pNewClient = pClient;
                nID = i;
                break;
            }
        }
    }

    if( pNewClient != nullptr )
    {
        pNewClient->m_SID.C.gate = XSFCore::GetServer()->GetSID()->S.index;
        pNewClient->m_SID.C.id = nID;
        pNewClient->m_SID.C.key = m_nGlobalKey ++;
        m_nTotal ++;

        return pNewClient;
    }

    XSF_ERROR("ClientManager::New can't found an empty client, m_nCount[%u], m_nSize[%u]", m_nTotal, m_nSize);

    return nullptr;
}

void ClientManager::Delete( Client * pClient )
{
    if( pClient->m_SID.ID == 0 )
    {
        XSF_ERROR("ClientModule::Delete pClient->m_ID.ID == 0");
        return;
    }

    uint32 nIndex = pClient->m_SID.C.id;
    if( m_Clients[nIndex] != pClient )
    {
        XSF_WARN("ClientModule::Delete m_Clients[nIndex] != pClient");
        return;
    }

    pClient->m_SID.ID = 0;
    m_nTotal --;
}

Client * ClientManager::GetClient( uint32 nClientID )
{
    SID sid;
    sid.ID = nClientID;

    if( sid.C.gate != XSFCore::GetServer()->GetSID()->S.index )
    {
        XSF_ERROR("ClientModule::Get sid.C.gate[%u] != XSFCore::GetServer()->GetID()->S.index[%u]", sid.C.gate, XSFCore::GetServer()->GetSID()->S.index);
        return nullptr;
    }
    
    uint32 nIndex = sid.C.id;
    if( m_Clients[nIndex] == nullptr )
        return nullptr;

    if( m_Clients[nIndex]->m_SID.C.key != sid.C.key )
        return nullptr;

    if( !m_Clients[nIndex]->m_bHandshake )
        return nullptr;

    return m_Clients[nIndex];
}

void ClientManager::Broadcast( const byte * pData, uint32 nDataLen )
{
    for( uint32 i = 0; i < m_nSize; ++ i )
    {
        Client * pClient = m_Clients[i];
        if( pClient != nullptr && pClient->m_bHandshake )
        {
            pClient->SendData(pData, nDataLen);
        }
    }
}

bool ClientManager::Init( ModuleInit * pInit )
{
    IServer * pServer = XSFCore::GetServer();
    auto pConfig = pServer->GetConfig();

    m_nSize = pConfig->GateMaxCount;
    if( m_nSize <= 0 )
        m_nSize = 1024;

    m_Clients = new Client * [m_nSize];
    memset( m_Clients, 0, sizeof(Client *)*m_nSize);

    m_pPakcer = new LocalPacker();

    return IModule::Init(pInit);
}

void ClientManager::Release(void)
{
    CONNECTION_CLOSE(m_pConnection);

    if( m_Clients != nullptr )
    {
        for( uint32 i = 0; i < m_nSize; ++ i )
        {
            if( m_Clients[i] != nullptr )
            {
                delete m_Clients[i];
            }
        }

        delete [] m_Clients;
        m_Clients = nullptr;
    }

    XSF_DELETE(m_pPakcer);
    
    delete this;
}

void ClientManager::DoRegist()
{
    XSFCore::SetMessageExecutor(xsf_pbid::Clt_Gt_Handshake, new Executor_Clt_Gt_Handshake());
    XSFCore::SetMessageExecutor(xsf_pbid::Clt_Gt_Heartbeat, new Executor_Clt_Gt_Heartbeat());
}

bool ClientManager::Start()
{
    m_pConnection = XSFCore::CreateConnection(this, m_pPakcer);
    if(m_pConnection == nullptr)
        return false;

    auto ports = XSFCore::GetServer()->GetPorts();
    return m_pConnection->Listen(ports[EP_Client]);
}

void ClientManager::OnStartClose(void)
{
    for( uint32 i = 0; i < m_nSize; ++ i )
    {
        if( m_Clients[i] != nullptr && m_Clients[i]->m_SID.ID > 0 )
        {
            m_Clients[i]->Disconnect(xsf_pb::DisconnectReason::ServerDown);
        }
    }
}

uint8 ClientManager::OnCloseCheck()
{
    if(m_nTotal > 0)
        return ModuleRunCode_Wait;

    return ModuleRunCode_OK;
}


// 接受到连接
INetHandler * ClientManager::OnAccept(IConnection * pIncomingConn)
{
    FUNCTION_ENTER(0);
    Client * pNewClient = New();
    if(pNewClient == nullptr)
    {
        return nullptr;
    }

    pNewClient->Create(pIncomingConn);
    const SID * pID = pNewClient->GetSID();
    XSF_WARN("client login, ip=[%s:%u], client=[%u %u-%u]", pIncomingConn->GetRemoteIP(), pIncomingConn->GetRemotePort(), pID->ID, pID->C.id, pID->C.key);

    return pNewClient;

    FUNCTION_EXIT(0);
}

// 已连接到远程主机
void ClientManager::OnConnected( IConnection * pConection )
{
    XSF_ERROR("ClientManager::OnConnected illegal call");
}

// 连接出错或断开
void ClientManager::OnError( IConnection * pConection, uint32 nErrorCode )
{
    XSF_ERROR("ClientManager::OnError error:%u", nErrorCode);
}

// 收到数据
void ClientManager::OnRecv(IConnection * pConection, DataResult * pResult)
{
    XSF_ERROR("ClientManager::OnRecv illegal call");
}

void SetClientManager(void)
{
    ModuleInit * pInit = new ModuleInit();
    pInit->nModuleID = GateModule_Client;
    strcpy(pInit->sName, "ClientManager");

    XSFCore::GetServer()->AddModule(ClientManager::Instance(), pInit);
}