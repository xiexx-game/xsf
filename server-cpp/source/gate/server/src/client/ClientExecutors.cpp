//////////////////////////////////////////////////////////////////////////
//
// 文件：source/gate/server/src/client/ClientExecutors.cpp
// 作者：Xoen Xie
// 时间：2020/08/05
// 描述：客户端 消息执行体
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#include "ClientExecutors.h"
#include "Client.h"
#include "DMessageInc.h"

//////////////////////////////////////////////////////////////////////////
//
// 客户端消息执行体
//
//////////////////////////////////////////////////////////////////////////
MESSAGE_EXECUTOR_EXECUTE(Clt_Gt_Handshake)
{
    XSF_CAST(pClient, pNetObj, Client);

    pClient->OnHandshake();
	pClient->SendMessage(XSFCore::GetMessage(xsf_pbid::Gt_Clt_Handshake));
}


MESSAGE_EXECUTOR_EXECUTE(Clt_Gt_Heartbeat)
{
    XSF_CAST(pClient, pNetObj, Client);
    pClient->UpdateHTTime();
    
    XSF_CAST( pLocalMessage, pMessage, xsf_msg::MSG_Clt_Gt_Heartbeat);
    XSF_CAST(pRespMessage, XSFCore::GetMessage(xsf_pbid::Gt_Clt_Heartbeat), xsf_msg::MSG_Gt_Clt_Heartbeat);
    pRespMessage->mPB.set_client_time(pLocalMessage->mPB.time());
    pRespMessage->mPB.set_server_time(XSFCore::TimeMS());
    
    pClient->SendMessage(pRespMessage);
}