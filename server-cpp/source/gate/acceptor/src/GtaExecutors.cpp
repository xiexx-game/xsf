//////////////////////////////////////////////////////////////////////////
//
// 文件：source\gate\acceptor\src\GTAMessageExecutors.cpp
// 作者：Xoen Xie
// 时间：2020/08/07
// 描述：消息执行体
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#include "GtaExecutors.h"
#include "NetPoint.h"
#include "DMessageInc.h"
#include "GateAcceptor.h"

namespace GateA
{
    MESSAGE_EXECUTOR_EXECUTE(Gt_GtA_Handshake)
    {
        XSF_CAST(pNetPoint, pNetObj, NetPoint);
        XSF_CAST(pLocalMsg, pMessage, xsf_msg::MSG_Gt_GtA_Handshake);
        pNetPoint->SetID(pLocalMsg->mPB.server_id());

        if(GateAcceptor::Instance()->Add(pNetPoint))
        {
            XSF_CAST(pRespMsg, XSFCore::GetMessage(xsf_pbid::GtA_Gt_Handshake), xsf_msg::MSG_GtA_Gt_Handshake);
            pRespMsg->mPB.set_server_id(XSFCore::GetServer()->GetSID()->ID);
            pNetPoint->SendMessage(pRespMsg);
        }
    }

    MESSAGE_EXECUTOR_EXECUTE(Gt_GtA_Heartbeat)
    {
        XSF_CAST(pNetPoint, pNetObj, NetPoint);
        pNetPoint->UpdateHTTime();
    }
    
    MESSAGE_EXECUTOR_EXECUTE(Gt_GtA_ClientClose)
    {
        XSF_CAST(pLocalMsg, pMessage, xsf_msg::MSG_Gt_GtA_ClientClose);
        GateAcceptor::Instance()->GetHandler()->OnClientClose(pLocalMsg->mPB.client_id());
    }
}