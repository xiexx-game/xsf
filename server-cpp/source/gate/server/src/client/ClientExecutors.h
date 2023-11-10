//////////////////////////////////////////////////////////////////////////
//
// 文件：source/gate/server/src/client/ClientExecutors.h
// 作者：Xoen Xie
// 时间：2020/08/07
// 描述：客户端 消息执行体
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _CLIENT_EXECUTORS_H_
#define _CLIENT_EXECUTORS_H_

#include "DMessageInc.h"
using namespace xsf;

MESSAGE_EXECUTOR(Clt_Gt_Handshake);
MESSAGE_EXECUTOR(Clt_Gt_Heartbeat);



#endif      // end of _CLIENT_EXECUTORS_H_