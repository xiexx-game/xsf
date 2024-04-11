//////////////////////////////////////////////////////////////////////////
//
// 文件：source/gate/server/src/connector/GateExecutors.cpp
// 作者：Xoen Xie
// 时间：2020/08/05
// 描述：消息执行体
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#include "GateExecutors.h"
#include "DMessageInc.h"
#include "ServerConnector.h"
#include "Client.h"
#include "ClientManager.h"

MESSAGE_EXECUTOR_EXECUTE(GtA_Gt_Handshake)
{
    XSF_CAST(pConnector, pNetObj, ServerConnector);
    XSF_CAST(pLocalMsg, pMessage, xsf_msg::MSG_GtA_Gt_Handshake);
    pConnector->SetRemoteID(pLocalMsg->mPB.server_id());
    pConnector->OnHandshake();
}

MESSAGE_EXECUTOR_EXECUTE(GtA_Gt_ClientDisconnect)
{
    XSF_CAST(pLocalMsg, pMessage, xsf_msg::MSG_GtA_Gt_ClientDisconnect);
    auto pClient = ClientManager::Instance()->GetClient(pLocalMsg->mPB.client_id());
    if(pClient != nullptr)
    {
        pClient->Disconnect(pLocalMsg->mPB.reason());
    }
}

MESSAGE_EXECUTOR_EXECUTE(GtA_Gt_ClientMessage)
{
    XSF_CAST(pLocalMsg, pMessage, xsf_msg::MSG_GtA_Gt_ClientMessage);
    const string & messageData = pLocalMsg->mPB.client_message();

    for(int32 i = 0; i < pLocalMsg->mPB.client_id_size(); i ++)
    {
        auto client = ClientManager::Instance()->GetClient(pLocalMsg->mPB.client_id(i));
        if(client != nullptr)
        {
            client->SendData((const byte *)messageData.c_str(), messageData.length());
        }
    }
}

MESSAGE_EXECUTOR_EXECUTE(GtA_Gt_Broadcast)
{
    XSF_CAST(pLocalMsg, pMessage, xsf_msg::MSG_GtA_Gt_Broadcast);
    const string & messageData = pLocalMsg->mPB.client_message();
    ClientManager::Instance()->Broadcast((const byte *)messageData.c_str(), messageData.length());
}

MESSAGE_EXECUTOR_EXECUTE(GtA_Gt_SetServerId)
{
    XSF_CAST(pLocalMsg, pMessage, xsf_msg::MSG_GtA_Gt_SetServerId);
    auto client = ClientManager::Instance()->GetClient(pLocalMsg->mPB.client_id());
    if(client != nullptr)
    {
        client->SetServerID(pLocalMsg->mPB.ep(), pLocalMsg->mPB.server_id());
    }
}