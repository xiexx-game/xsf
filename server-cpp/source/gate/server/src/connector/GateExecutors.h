//////////////////////////////////////////////////////////////////////////
//
// 文件：source/gate/server/src/connector/GateExecutors.h
// 作者：Xoen Xie
// 时间：2020/08/07
// 描述：消息执行体
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _GATE_EXECUTORS_H_
#define _GATE_EXECUTORS_H_

#include "DMessageInc.h"
using namespace xsf;

MESSAGE_EXECUTOR(GtA_Gt_Handshake);
MESSAGE_EXECUTOR(GtA_Gt_ClientDisconnect);
MESSAGE_EXECUTOR(GtA_Gt_ClientMessage);
MESSAGE_EXECUTOR(GtA_Gt_Broadcast);
MESSAGE_EXECUTOR(GtA_Gt_SetServerId);


#endif      // end of _GATE_EXECUTORS_H_