//////////////////////////////////////////////////////////////////////////
//
// 文件：center\server\src\MessageExecutors.cpp
// 作者：Xoen Xie
// 时间：2020/06/12
// 描述：消息执行体
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#include "CenterExecutors.h"
#include "DMessageInc.h"
using namespace xsf_msg;
#include "NetPoint.h"
#include "NodeManager.h"

MESSAGE_EXECUTOR_EXECUTE(Cc_C_Handshake)
{
    XSF_CAST(pLocalMsg, pMessage, MSG_Cc_C_Handshake);
    vector<uint32> ports;
    for(int32 i = 0; i < pLocalMsg->mPB.ports_size(); i ++)
    {
        ports.push_back(pLocalMsg->mPB.ports(i));
    }

    XSF_CAST(pNetPoint, pNetObj, NetPoint);
    ServerInfo * pInfo = NodeManager::Instance()->AddNode(pLocalMsg->mPB.server_id(), pNetPoint->GetRemoteIP(), ports);
    if(pInfo == nullptr)
    {
        pNetPoint->Close();
        return;
    }

    pNetPoint->SetID(pInfo->ID);

    if(NodeManager::Instance()->Add(pNetPoint))
    {
        XSF_CAST(pRespMsg, XSFCore::GetMessage(xsf_pbid::SMSGID::C_Cc_Handshake), xsf_msg::MSG_C_Cc_Handshake);
        pRespMsg->mPB.clear_ports();
        pRespMsg->mPB.set_server_id(XSFCore::GetServer()->GetSID()->ID);
        pRespMsg->mPB.set_new_id(pInfo->ID);

        for(uint8 i = 0; i < EP_Max; i ++)
        {
            pRespMsg->mPB.add_ports(pInfo->Ports[i]);
        }

        pNetPoint->SendMessage(pRespMsg);
    }
}

MESSAGE_EXECUTOR_EXECUTE(Cc_C_Heartbeat)
{
    XSF_CAST(pNetPoint, pNetObj, NetPoint);
    pNetPoint->UpdateHTTime();
}

MESSAGE_EXECUTOR_EXECUTE(Cc_C_ServerOk)
{
    XSF_CAST(pNetPoint, pNetObj, NetPoint);
    NodeManager::Instance()->OnNodeOK(pNetPoint->GetSID()->ID);
}