//////////////////////////////////////////////////////////////////////////
//
// 文件：message\src\Messages.cpp
// 作者：Xoen Xie
// 时间：2020/05/29
// 描述：所有消息类
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#include "Messages.h"


namespace xsf_msg
{
// 不要手动修改以下部分
//MESSAGE_BEGIN
	MESSAGE_FUNCTIONS(Clt_Gt_Handshake)
	MESSAGE_FUNCTIONS(Gt_Clt_Handshake)
	MESSAGE_FUNCTIONS(Clt_Gt_Heartbeat)
	MESSAGE_FUNCTIONS(Gt_Clt_Heartbeat)
	MESSAGE_FUNCTIONS(Gt_Clt_Disconnect)
	MESSAGE_FUNCTIONS(Clt_G_Login)
	MESSAGE_FUNCTIONS(G_Clt_LoginResult)
	MESSAGE_FUNCTIONS(G_Clt_TestData)
	MESSAGE_FUNCTIONS(Cc_C_Handshake)
	MESSAGE_FUNCTIONS(C_Cc_Handshake)
	MESSAGE_FUNCTIONS(Cc_C_Heartbeat)
	MESSAGE_FUNCTIONS(Cc_C_ServerInfo)
	MESSAGE_FUNCTIONS(C_Cc_ServerInfo)
	MESSAGE_FUNCTIONS(Cc_C_ServerLost)
	MESSAGE_FUNCTIONS(C_Cc_ServerLost)
	MESSAGE_FUNCTIONS(Cc_C_ServerOk)
	MESSAGE_FUNCTIONS(C_Cc_ServerOk)
	MESSAGE_FUNCTIONS(C_Cc_Stop)
	MESSAGE_FUNCTIONS(Gt_GtA_Handshake)
	MESSAGE_FUNCTIONS(GtA_Gt_Handshake)
	MESSAGE_FUNCTIONS(Gt_GtA_Heartbeat)
	MESSAGE_FUNCTIONS(Gt_GtA_ClientClose)
	MESSAGE_FUNCTIONS(GtA_Gt_ClientDisconnect)
	MESSAGE_FUNCTIONS(GtA_Gt_ClientMessage)
	MESSAGE_FUNCTIONS(GtA_Gt_Broadcast)
	MESSAGE_FUNCTIONS(GtA_Gt_SetServerId)
//MESSAGE_END
}