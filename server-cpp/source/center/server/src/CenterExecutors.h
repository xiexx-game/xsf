//////////////////////////////////////////////////////////////////////////
//
// 文件：center\server\src\MessageExecutors.h
// 作者：Xoen Xie
// 时间：2020/06/12
// 描述：消息执行体
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _MESSAGE_EXECUTORS_H_
#define _MESSAGE_EXECUTORS_H_

#include "DMessageInc.h"
using namespace xsf;
#include "IXSFMessage.h"

MESSAGE_EXECUTOR(Cc_C_Handshake);
MESSAGE_EXECUTOR(Cc_C_Heartbeat);
MESSAGE_EXECUTOR(Cc_C_ServerOk);

#endif      // end of _MESSAGE_EXECUTORS_H_