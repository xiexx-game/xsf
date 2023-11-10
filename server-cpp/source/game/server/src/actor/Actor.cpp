//////////////////////////////////////////////////////////////////////////
//
// 文件：source/game/server/src/actor/Actor.cpp
// 作者：Xoen Xie
// 时间：2020/02/29
// 描述：玩家对象
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#include "Actor.h"
#include "IGateAcceptor.h"
#include "DMessageInc.h"

void Actor::OnLoginOK(void)
{
    auto *pAcceptor = GateA::GetGateAcceptor();
    XSF_CAST(pMessage, XSFCore::GetMessage(xsf_pbid::G_Clt_LoginResult), xsf_msg::MSG_G_Clt_LoginResult);
    pMessage->mPB.set_result(xsf_pb::LoginResult::Success);
    pAcceptor->SendMessage2Client(m_nClientID, pMessage);

    XSF_CAST(pTestMessage, XSFCore::GetMessage(xsf_pbid::G_Clt_TestData), xsf_msg::MSG_G_Clt_TestData);
    pTestMessage->mPB.set_message("test1");
    pAcceptor->Broadcast2AllClient(pTestMessage);

    pTestMessage->mPB.set_message("test2");
    pAcceptor->BeginBroadcast();
    pAcceptor->PushClientID(m_nClientID);
    pAcceptor->EndBroadcast(pTestMessage);
}