//////////////////////////////////////////////////////////////////////////
//
// 文件：source/gate/server/src/connector/ServerConnector.cpp
// 作者：Xoen Xie
// 时间：2020/08/06
// 描述：连接器
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#include "ServerConnector.h"
#include "DMessageInc.h"
#include "ConnectorManager.h"

void ServerConnector::SendHandshake(void)
{
    XSF_CAST(pMessage, XSFCore::GetMessage(xsf_pbid::Gt_GtA_Handshake), xsf_msg::MSG_Gt_GtA_Handshake);
    pMessage->mPB.set_server_id(XSFCore::GetServer()->GetSID()->ID);
    SendMessage(pMessage);
}

void ServerConnector::SendHeartbeat(void)
{
    SendMessage(XSFCore::GetMessage(xsf_pbid::Gt_GtA_Heartbeat));
}

void ServerConnector::OnNetError(void)
{
    ConnectorManager::Instance()->DeleteConnector(m_RemoteID.ID);
}