//////////////////////////////////////////////////////////////////////////
//
// 文件：source/center/connector/src/CCExecutors.cpp
// 作者：Xoen Xie
// 时间：2020/06/12
// 描述：消息执行器
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#include "CCExecutors.h"
#include "CenterConnector.h"

namespace CC
{
    MESSAGE_EXECUTOR_EXECUTE(C_Cc_Handshake)
    {
        XSF_CAST(pConnector, pNetObj, CenterConnector);
        XSF_CAST(pLocalMsg, pMessage, xsf_msg::MSG_C_Cc_Handshake);
        pConnector->SetRemoteID(pLocalMsg->mPB.server_id());

        auto pServer = XSFCore::GetServer();
        pServer->SetID(pLocalMsg->mPB.new_id());
        XSF_INFO("收到中心服握手，更新本服ID，server id=%u [%u-%u-%u]", pServer->GetSID()->ID, pServer->GetSID()->S.server, pServer->GetSID()->S.type, pServer->GetSID()->S.index);

        for(int32 i = 0; i < EP_Max; i ++)
        {
            pServer->SetPort(i, pLocalMsg->mPB.ports(i));
        }

        pConnector->OnHandshake();
        pServer->DoStart();

        if(pServer->IsRunning())
        {
            pConnector->OnOK();
        }
    }

    MESSAGE_EXECUTOR_EXECUTE(C_Cc_ServerInfo)
    {
        XSF_CAST(pConnector, pNetObj, CenterConnector);
        XSF_CAST(pLocalMsg, pMessage, xsf_msg::MSG_C_Cc_ServerInfo);
        pConnector->AddInfo(pLocalMsg);
    }
    
    MESSAGE_EXECUTOR_EXECUTE(C_Cc_ServerLost)
    {
        XSF_CAST(pConnector, pNetObj, CenterConnector);
        XSF_CAST(pLocalMsg, pMessage, xsf_msg::MSG_C_Cc_ServerLost);
        pConnector->OnNodeLost(pLocalMsg->mPB.server_id());
    }
    
    MESSAGE_EXECUTOR_EXECUTE(C_Cc_ServerOk)
    {
        XSF_CAST(pConnector, pNetObj, CenterConnector);
        XSF_CAST(pLocalMsg, pMessage, xsf_msg::MSG_C_Cc_ServerLost);
        pConnector->OnNodeOk(pLocalMsg->mPB.server_id());
    }
    
    MESSAGE_EXECUTOR_EXECUTE(C_Cc_Stop)
    {
        XSF_INFO("【中心服连接器】收到中心服关服");
        XSFCore::GetServer()->Stop();
    }
    
}



