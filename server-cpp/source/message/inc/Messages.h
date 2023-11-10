//////////////////////////////////////////////////////////////////////////
//
// 文件：message\inc\Messages.h
// 作者：Xoen Xie
// 时间：2020/05/29
// 描述：所有网络消息
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _MESSAGES_H_
#define _MESSAGES_H_

#include "XSFDef.h"
#include "XSFMallocHook.h"

#include "CMessageID.pb.h"
#include "SMessageID.pb.h"
#include "Center.pb.h"
#include "Gate.pb.h"
#include "Client.pb.h"
#include "Common.pb.h"

#include "IXSFMessage.h"
#include "DMessageDef.h"

#include "StreamReader.h"
#include "StreamWriter.h"
using namespace xsf;
using namespace xsf_pb;

namespace xsf_msg
{
// 不要手动修改以下部分
// 请在Unity中执行菜单 XSFTools/proto导出代码
//MESSAGE_BEGIN 
	MESSAGE(Clt_Gt_Handshake, EP_Client, xsf_pbid::CMSGID::Clt_Gt_Handshake)
	MESSAGE(Gt_Clt_Handshake, EP_Client, xsf_pbid::CMSGID::Gt_Clt_Handshake)
	MESSAGE(Clt_Gt_Heartbeat, EP_Client, xsf_pbid::CMSGID::Clt_Gt_Heartbeat)
	MESSAGE(Gt_Clt_Heartbeat, EP_Client, xsf_pbid::CMSGID::Gt_Clt_Heartbeat)
	MESSAGE(Gt_Clt_Disconnect, EP_Client, xsf_pbid::CMSGID::Gt_Clt_Disconnect)
	MESSAGE(Clt_G_Login, EP_Game, xsf_pbid::CMSGID::Clt_G_Login)
	MESSAGE(G_Clt_LoginResult, EP_Client, xsf_pbid::CMSGID::G_Clt_LoginResult)
	MESSAGE(G_Clt_TestData, EP_Client, xsf_pbid::CMSGID::G_Clt_TestData)
	MESSAGE(Cc_C_Handshake, EP_Client, xsf_pbid::SMSGID::Cc_C_Handshake)
	MESSAGE(C_Cc_Handshake, EP_Client, xsf_pbid::SMSGID::C_Cc_Handshake)
	MESSAGE(Cc_C_Heartbeat, EP_Client, xsf_pbid::SMSGID::Cc_C_Heartbeat)
	MESSAGE(Cc_C_ServerInfo, EP_Client, xsf_pbid::SMSGID::Cc_C_ServerInfo)
	MESSAGE(C_Cc_ServerInfo, EP_Client, xsf_pbid::SMSGID::C_Cc_ServerInfo)
	MESSAGE(Cc_C_ServerLost, EP_Client, xsf_pbid::SMSGID::Cc_C_ServerLost)
	MESSAGE(C_Cc_ServerLost, EP_Client, xsf_pbid::SMSGID::C_Cc_ServerLost)
	MESSAGE(Cc_C_ServerOk, EP_Client, xsf_pbid::SMSGID::Cc_C_ServerOk)
	MESSAGE(C_Cc_ServerOk, EP_Client, xsf_pbid::SMSGID::C_Cc_ServerOk)
	MESSAGE(C_Cc_Stop, EP_Client, xsf_pbid::SMSGID::C_Cc_Stop)
	MESSAGE(Gt_GtA_Handshake, EP_Client, xsf_pbid::SMSGID::Gt_GtA_Handshake)
	MESSAGE(GtA_Gt_Handshake, EP_Client, xsf_pbid::SMSGID::GtA_Gt_Handshake)
	MESSAGE(Gt_GtA_Heartbeat, EP_Client, xsf_pbid::SMSGID::Gt_GtA_Heartbeat)
	MESSAGE(Gt_GtA_ClientClose, EP_Client, xsf_pbid::SMSGID::Gt_GtA_ClientClose)
	MESSAGE(GtA_Gt_ClientDisconnect, EP_Client, xsf_pbid::SMSGID::GtA_Gt_ClientDisconnect)
	MESSAGE(GtA_Gt_ClientMessage, EP_Client, xsf_pbid::SMSGID::GtA_Gt_ClientMessage)
	MESSAGE(GtA_Gt_Broadcast, EP_Client, xsf_pbid::SMSGID::GtA_Gt_Broadcast)
	MESSAGE(GtA_Gt_SetServerId, EP_Client, xsf_pbid::SMSGID::GtA_Gt_SetServerId)
//MESSAGE_END
}




#endif      // end of _MESSAGES_H_
