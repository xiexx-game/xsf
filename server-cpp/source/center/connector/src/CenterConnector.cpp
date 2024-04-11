//////////////////////////////////////////////////////////////////////////
//
// 文件：center\connector\src\CenterConnector.cpp
// 作者：Xoen Xie
// 时间：2020/06/11
// 描述：中心服连接器
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#include "CenterConnector.h"
#include "DMessageInc.h"
#include "CCExecutors.h"
#include "ICenterConnector.h"
 
namespace CC
{
    void CenterConnector::DoRegist()
    {
        XSFCore::SetMessageExecutor(xsf_pbid::C_Cc_Handshake, new Executor_C_Cc_Handshake());
        XSFCore::SetMessageExecutor(xsf_pbid::C_Cc_ServerInfo, new Executor_C_Cc_ServerInfo());
        XSFCore::SetMessageExecutor(xsf_pbid::C_Cc_ServerLost, new Executor_C_Cc_ServerLost());
        XSFCore::SetMessageExecutor(xsf_pbid::C_Cc_ServerOk, new Executor_C_Cc_ServerOk());
        XSFCore::SetMessageExecutor(xsf_pbid::C_Cc_Stop, new Executor_C_Cc_Stop());
    }

    bool CenterConnector::Start()
    {
        auto pConfig = XSFCore::GetServer()->GetConfig();

        return Connect(pConfig->MainCenterIP, pConfig->CenterPort);
    }

    void CenterConnector::Release(void)
    {
        XSF_DELETE(m_pHandler);
        
        NetConnector::Release();
    }

    uint8 CenterConnector::OnStartCheck() 
    {
        if(m_bHandshake)
        {
            return ModuleRunCode_OK;
        }

        return ModuleRunCode_Wait;
    }

    void CenterConnector::OnOK(void)
    {
        XSF_CAST(pMessage, XSFCore::GetMessage(xsf_pbid::Cc_C_ServerOk), xsf_msg::MSG_Cc_C_ServerOk);
        pMessage->mPB.set_server_id(XSFCore::GetServer()->GetSID()->ID);
        SendMessage(pMessage);
    }

    void CenterConnector::SendHandshake(void)
    {
        XSF_CAST(pMessage, XSFCore::GetMessage(xsf_pbid::Cc_C_Handshake), xsf_msg::MSG_Cc_C_Handshake);
        pMessage->mPB.set_server_id(XSFCore::GetServer()->GetSID()->ID);

        pMessage->mPB.clear_ports();

        auto pPorts = XSFCore::GetServer()->GetPorts();
        for(uint8 i = 0; i < EP_Max; i ++)
        {
            pMessage->mPB.add_ports(pPorts[i]);
        }

        SendMessage(pMessage);
    }

    void CenterConnector::SendHeartbeat(void)
    {
        SendMessage(XSFCore::GetMessage(xsf_pbid::Cc_C_Heartbeat));
    }

    void CenterConnector::AddInfo(xsf_msg::MSG_C_Cc_ServerInfo * pMessage)
    {
        for(int32 i = 0; i < pMessage->mPB.infos_size(); i ++)
        {
            auto & pbInfo = pMessage->mPB.infos(i);
            bool IsNewAdd = false;

            ServerInfo *pInfoFind = nullptr;
            auto it = m_Nodes.find(pbInfo.server_id());
            if(it != m_Nodes.end())
            {
                pInfoFind = it->second;
            }
            else
            {
                IsNewAdd = true;
                pInfoFind = new ServerInfo();
                pInfoFind->ID = pbInfo.server_id();
                m_Nodes.insert(InfoMap::value_type(pInfoFind->ID, pInfoFind));
            }

            strcpy(pInfoFind->sIP, pbInfo.ip().c_str());
            for(uint8 J = 0; J < EP_Max; J ++)
            {
                pInfoFind->Ports[J] = pbInfo.ports(J);
            }

            pInfoFind->Status = pbInfo.status();
            SID sid;
            sid.ID = pInfoFind->ID;
            XSF_INFO("【中心服连接器】收到服务器信息, id=%u [%u-%u-%s] status=%u", pInfoFind->ID, sid.S.server, sid.S.index, xsf::EP2CNName(sid.S.type), pInfoFind->Status);

            if(IsNewAdd)
            {
                XSF_INFO("【中心服连接器】新增服务器节点, id=%u [%u-%u-%s] status=%u", pInfoFind->ID, sid.S.server, sid.S.index, xsf::EP2CNName(sid.S.type), pInfoFind->Status);
                if(m_pHandler != nullptr)
                    m_pHandler->OnServerNew(pInfoFind);

                if(pInfoFind->Status == NodeStatus_Ok)
                {
                    XSF_INFO("【中心服连接器】收到服务器节点已准备好, id=%u [%u-%u-%s]", pInfoFind->ID, sid.S.server, sid.S.index, xsf::EP2CNName(sid.S.type));
                    if(m_pHandler != nullptr)
                        m_pHandler->OnServerOk(pInfoFind);
                }
            }
        }
    }

    void CenterConnector::OnNodeLost(uint32 nID)
    {
        auto it = m_Nodes.find(nID);
        if(it != m_Nodes.end())
        {
            delete it->second;
            m_Nodes.erase(it);
        }
        
        SID sid;
        sid.ID = nID;

        XSF_INFO("【中心服连接器】有服务器节点离线, id=%u [%u-%u-%s]", nID, sid.S.server, sid.S.index, xsf::EP2CNName(sid.S.type));
        if(m_pHandler != nullptr)
            m_pHandler->OnServerLost(nID);
    }

    void CenterConnector::OnNodeOk(uint32 nID)
    {
        auto it = m_Nodes.find(nID);
        if(it != m_Nodes.end())
        {
            it->second->Status = NodeStatus_Ok;

            SID sid;
            sid.ID = nID;

            XSF_INFO("【中心服连接器】收到服务器节点已准备好, id=%u [%u-%u-%s]", nID, sid.S.server, sid.S.index, xsf::EP2CNName(sid.S.type));
            if(m_pHandler != nullptr)
                m_pHandler->OnServerOk(it->second);
        }
        else
        {
            XSF_ERROR("服务器已准备好，但是本地未找到服务器信息, id=%u", nID);
        }
    }

    void SetCenterConnector(uint32 nModuleID, IServerInfoHandler *pInfoHandler)
    {
        NetConnectorInit * pInit = new NetConnectorInit();
        pInit->nModuleID = nModuleID;
        strcpy(pInit->sName, "CenterConnector");
        pInit->NeedReconnect = true;
        pInit->NoWaitStart = true;

        CenterConnector * pConnector = new CenterConnector();
        pConnector->SetHandler(pInfoHandler);
        XSFCore::GetServer()->AddModule(pConnector, pInit);
    }
}
