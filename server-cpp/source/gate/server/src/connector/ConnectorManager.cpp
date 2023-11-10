//////////////////////////////////////////////////////////////////////////
//
// 文件：source/gate/server/src/connector/ConnectorManager.cpp
// 作者：Xoen Xie
// 时间：2022/08/31
// 描述：connector 管理
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#include "ConnectorManager.h"
#include "DMessageInc.h"
#include "GateExecutors.h"
#include "ServerConnector.h"
#include "Server.h"

void ConnectorManager::DoRegist()
{
    XSFCore::SetMessageExecutor(xsf_pbid::GtA_Gt_Handshake, new Executor_GtA_Gt_Handshake());
    XSFCore::SetMessageExecutor(xsf_pbid::GtA_Gt_ClientDisconnect, new Executor_GtA_Gt_ClientDisconnect());
    XSFCore::SetMessageExecutor(xsf_pbid::GtA_Gt_ClientMessage, new Executor_GtA_Gt_ClientMessage());
    XSFCore::SetMessageExecutor(xsf_pbid::GtA_Gt_Broadcast, new Executor_GtA_Gt_Broadcast());
    XSFCore::SetMessageExecutor(xsf_pbid::GtA_Gt_SetServerId, new Executor_GtA_Gt_SetServerId());
}

void ConnectorManager::OnClose(void) 
{
    for(auto it = m_Connectors.begin(); it != m_Connectors.end(); it ++)
    {
        it->second->OnClose();
    }
}


void ConnectorManager::CreateConnector(uint32 nID, const char * ip, uint32 port)
{
    auto it = m_Connectors.find(nID);
    if(it != m_Connectors.end())
    {
        XSF_INFO("ConnectorManager::CreateConnector connector exist, id=%u", nID);
        return;
    }

    ServerConnector * pConnector = new ServerConnector();
    pConnector->SetRemoteID(nID);

    NetConnectorInit init;
    init.NeedReconnect = false;
    init.nModuleID = 0;
    sprintf(init.sName, "%u:%u-%u-%u", nID, pConnector->GetRemoteID()->S.server, pConnector->GetRemoteID()->S.type, pConnector->GetRemoteID()->S.index);
    pConnector->Init(&init);

    pConnector->Connect(ip, port);

    m_Connectors.insert(ConnMap::value_type(nID, pConnector));

    m_EPConnectors[pConnector->GetRemoteID()->S.type].push_back(pConnector);
}

void ConnectorManager::DeleteConnector(uint32 nID)
{
    auto it = m_Connectors.find(nID);
    if(it != m_Connectors.end())
    {
        auto & v = m_EPConnectors[it->second->GetRemoteID()->S.type];
        for(auto it2 = v.begin(); it2 != v.end(); it2 ++)
        {
            if(*it2 == it->second)
            {
                v.erase(it2);
                break;
            }
        }

        it->second->Release();

        m_Connectors.erase(it);
    }
}

ServerConnector * ConnectorManager::GetConnector(uint8 nEP, uint32 nID)
{
    if(nID > 0)
    {
        auto it = m_Connectors.find(nID);
        if(it != m_Connectors.end())
            return it->second;

        return nullptr;
    }
    else
    {
        auto & v = m_EPConnectors[nEP];
        int nTotal = v.size();
        if(nTotal <= 0)
            return nullptr;

        int nIndex = XSFCore::RandomRange(0, nTotal-1);
        return v[nIndex];
    }
}

void SetConnectorManager(void)
{
    ModuleInit * pInit = new ModuleInit();
    pInit->nModuleID = GateModule_Connector;
    strcpy(pInit->sName, "ConnectorManager");

    XSFCore::GetServer()->AddModule(ConnectorManager::Instance(), pInit);
}