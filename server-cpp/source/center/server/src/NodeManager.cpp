//////////////////////////////////////////////////////////////////////////
//
// 文件：source/center/server/src/NodeManager.cpp
// 作者：Xoen Xie
// 时间：2020/02/29
// 描述：节点管理
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#include "NodeManager.h"
#include "XSF.h"
#include "DMessageInc.h"
#include "CenterExecutors.h"
#include "NetPoint.h"
#include "Server.h"

bool NodeManager::Init(ModuleInit * pInit) 
{
    auto pConfig = XSFCore::GetServer()->GetConfig();
    m_nInnerPort = pConfig->InnerPortStart;
    m_nOutPort = pConfig->OutPortStart;

    for(uint8 i = 0; i < EP_Max; i ++)
    {
        m_NodeIndex[i] = 1;
    }

    return NormalNetPointManager::Init(pInit);
}

bool NodeManager::Start(void) 
{
    m_nStartIndex = 0;
    m_nStep = RunStep_StartServer;

    return NormalNetPointManager::Start();
}

void NodeManager::OnClose(void)
{
    auto pMessage = XSFCore::GetMessage(xsf_pbid::SMSGID::C_Cc_Stop);
    Broadcast(pMessage, 0);
}

void NodeManager::DoRegist(void)
{
    XSFCore::SetMessageExecutor(xsf_pbid::SMSGID::Cc_C_Handshake, new Executor_Cc_C_Handshake());
    XSFCore::SetMessageExecutor(xsf_pbid::SMSGID::Cc_C_Heartbeat, new Executor_Cc_C_Heartbeat());
    XSFCore::SetMessageExecutor(xsf_pbid::SMSGID::Cc_C_ServerOk, new Executor_Cc_C_ServerOk());
}

uint8 NodeManager::OnStartCheck()
{
    auto pConfig = XSFCore::GetServer()->GetConfig();
    auto pInitData = XSFCore::GetServer()->GetInitData();
    if(!pConfig->AutoStart)
    {
        XSF_INFO("NodeManager::OnStartCheck no auto start ...");
        return ModuleRunCode_OK;
    }
    else
    {
        switch (m_nStep)
        {
        case RunStep_StartServer:
            {
                const ServerNode & node = pConfig->NodeList[m_nStartIndex];
                m_nCurStartEP = node.nEP;

                char sShell[BUFFER_SIZE_1024] = {0};
                sprintf(sShell, "cd ../; sh ./single_start.sh %s %s %u", pInitData->sTag, xsf::EP2Name(m_nCurStartEP), XSFCore::GetServer()->GetSID()->S.server);
                XSFCore::ShellExec(sShell);
                m_nStep = RunStep_WaitHandshake;
            }
            break;

        case RunStep_HandshakeDone:
            {
                m_nStartIndex ++;
                if(m_nStartIndex >= pConfig->NodeList.size())
                {
                    m_nStep = RunStep_OK;
                }
                else
                {
                    m_nStep = RunStep_StartServer;
                }
            }
            break;
        
        case RunStep_OK:
            return ModuleRunCode_OK;
        }
    }

    return ModuleRunCode_Wait;
}


void NodeManager::OnNetPointLost( NetPoint * pPoint )
{
    ServerInfo * pInfo = nullptr;
    auto it = m_Nodes.find(pPoint->GetSID()->ID);
    if(it != m_Nodes.end())
    {
        pInfo = it->second;
        m_Nodes.erase(it);
        m_LostList[pPoint->GetSID()->S.type].push_back(pInfo);
    }
    else
    {
        XSF_INFO("NodeManager::OnNetPointLost node not exist, id=%u", pPoint->GetSID()->ID);
        return;
    }

    XSF_CAST(pMessage, XSFCore::GetMessage(xsf_pbid::SMSGID::C_Cc_ServerLost), xsf_msg::MSG_C_Cc_ServerLost);
    pMessage->mPB.set_server_id(pPoint->GetSID()->ID);
    XSF_INFO("【中心服】有服务器节点离线, id=%u", pPoint->GetSID()->ID);
    Broadcast(pMessage, 0);
}

void NodeManager::OnNetPointConnected( NetPoint * pPoint )
{
    XSF_CAST(pMessage, XSFCore::GetMessage(xsf_pbid::SMSGID::C_Cc_ServerInfo), xsf_msg::MSG_C_Cc_ServerInfo);
    pMessage->mPB.clear_infos();
    uint32 nID = pPoint->GetSID()->ID;

    // 把当前已经收到的服务器信息下发给新加入的节点
    for(auto it = m_Nodes.begin(); it != m_Nodes.end(); it ++)
    {
        if(it->first != nID)
        {
            auto pInfo = pMessage->mPB.add_infos();
            pInfo->set_server_id(it->second->ID);
            pInfo->set_ip(it->second->sIP);
            pInfo->set_status(it->second->Status);

            for(uint8 i = 0; i < EP_Max; i ++)
                pInfo->add_ports(it->second->Ports[i]);
        }
    }

    if(pMessage->mPB.infos_size() > 0)
    {
        pPoint->SendMessage(pMessage);
    }

    // 把新加入的节点信息，广播给其他所有服务器节点
    pMessage->mPB.clear_infos();
    auto pNode = m_Nodes.find(nID)->second;
    auto pInfo = pMessage->mPB.add_infos();
    pInfo->set_server_id(pNode->ID);
    pInfo->set_ip(pNode->sIP);
    pInfo->set_status(pNode->Status);

    for(uint8 i = 0; i < EP_Max; i ++)
        pInfo->add_ports(pNode->Ports[i]);

    Broadcast(pMessage, nID);

    if(m_nCurStartEP == pPoint->GetSID()->S.type)
    {
        m_nStep = RunStep_HandshakeDone;
    }
}



ServerInfo * NodeManager::AddNode(uint32 nID, const char * ip, vector<uint> & ports)
{
    ServerInfo * pNewNode = nullptr;
    SID sid;
    sid.ID = nID;
    XSF_INFO("NodeManager::AddNode id=%u %u-%s-%u", nID, sid.S.server, EP2CNName(sid.S.type), sid.S.index);

    if(sid.S.index == 0)
    {
        ServerInfo * nodeLost = GetLostNode(sid.S.type, nID, false);
        if(nodeLost == nullptr)
        {
            pNewNode = new ServerInfo();
            GetPort(sid.S.type, pNewNode);
            sid.S.index = GetIndex(sid.S.type);

            pNewNode->ID = sid.ID;
            strcpy(pNewNode->sIP, ip);
            pNewNode->Status = NodeStatus_New;

            XSF_INFO("NodeManager::AddNode Add new id=%u, ip=%s", pNewNode->ID, ip); 
        }
        else
        {
            pNewNode = nodeLost;
            pNewNode->Status = NodeStatus_New;
            strcpy(pNewNode->sIP, ip);
            XSF_INFO("NodeManager::AddNode Add new, find lost server node, id=%u, ip lost=%s, ip=%s", pNewNode->ID, pNewNode->sIP, ip); 
        }

        if(!m_Nodes.insert(InfoMap::value_type(pNewNode->ID, pNewNode)).second)
        {
            XSF_ERROR("NodeManager::AddNode m_Nodes.insert insert error");
            return nullptr;
        }
    }
    else
    {
        auto it = m_Nodes.find(nID);
        if(it != m_Nodes.end())
        {
            pNewNode = it->second;
            XSF_INFO("NodeManager AddNode find exist 1, id=%u, ip=%s", nID, ip);
        }
        else
        {
            ServerInfo * nodeLost = GetLostNode(sid.S.type, nID, true);
            if(nodeLost == nullptr)
            {
                XSF_ERROR("NodeManager AddNode nodeLost not found, id=%u, ip=%s", nID, ip);
                return nullptr;
            }
            else
            {
                if (strncmp(pNewNode->sIP, ip, strlen(ip)) != 0)
                {
                    XSF_ERROR("NodeManager AddNode ip error, id=%u, node ip=%s, ip=%s", nID, pNewNode->sIP, ip);
                    return nullptr;
                }

                for(uint32 i = 0; i < EP_Max; i ++)
                {
                    if(pNewNode->Ports[i] != ports[i])
                    {
                        XSF_ERROR("NodeManager AddNode port error, id=%u, i=%u, node port=%u, port=%u", nID, i, pNewNode->Ports[i], ports[i]);
                        return nullptr;
                    }
                }
            }

            pNewNode = nodeLost;
            if(!m_Nodes.insert(InfoMap::value_type(pNewNode->ID, pNewNode)).second)
            {
                XSF_ERROR("NodeManager::AddNode m_Nodes.insert insert error 2");
                return nullptr;
            }
        }
    }

    return pNewNode;
}

void NodeManager::OnNodeOK(uint32 nID)
{
    auto it = m_Nodes.find(nID);
    if(it != m_Nodes.end())
    {
        it->second->Status = NodeStatus_Ok;
    }
    else
    {
        XSF_WARN("NodeManager::OnNodeOK, node not exist, id=%u", nID);
        return;
    }

    XSF_INFO("【中心服】收到服务器已准备好 id=%u", nID);

    XSF_CAST(pMessage, XSFCore::GetMessage(xsf_pbid::SMSGID::C_Cc_ServerOk), xsf_msg::MSG_C_Cc_ServerOk);
    pMessage->mPB.set_server_id(nID);
    Broadcast(pMessage, nID);
}

ServerInfo * NodeManager::GetLostNode(byte nEP, uint32 nID, bool CheckEqual)
{
    list<ServerInfo*> & l = m_LostList[nEP];
    for(auto it = l.begin(); it != l.end(); it ++)
    {
        auto * pInfo = (*it);
        if(CheckEqual)
        {
            if(pInfo->ID == nID)
            {
                l.erase(it);
                return pInfo;
            }
        }
        else
        {
            l.erase(it);
            return pInfo;
        }
    }

    return nullptr;
}

void NodeManager::GetPort(byte nEP, ServerInfo * pInfo)
{
    switch(nEP)
    {
    case EP_Gate:
        pInfo->Ports[EP_Client] = GetNextPort(false);
        break;

    case EP_Game:
        pInfo->Ports[EP_Gate] = GetNextPort(true);
        break;

    case EP_Login:
        pInfo->Ports[EP_Gate] = GetNextPort(true);
        break;

    default:
        break;
    }
}

void SetNodeManager(void)
{
    NetPointManagerInit * init = new NetPointManagerInit();
    init->nModuleID = CenterModule_NodeManager;
    init->NoWaitStart = true;
    strcpy(init->sName, "NodeManager");
    init->nPort = XSFCore::GetServer()->GetConfig()->CenterPort;

    XSFCore::GetServer()->AddModule(NodeManager::Instance(), init);
}