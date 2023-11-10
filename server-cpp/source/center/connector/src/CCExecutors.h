//////////////////////////////////////////////////////////////////////////
//
// 文件：source/center/connector/src/CCExecutors.h
// 作者：Xoen Xie
// 时间：2020/06/12
// 描述：消息执行器
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _CC_MESSAGE_EXECUTORS_H_
#define _CC_MESSAGE_EXECUTORS_H_

#include "DMessageInc.h"
using namespace xsf;

namespace CC
{
    MESSAGE_EXECUTOR(C_Cc_Handshake);
    MESSAGE_EXECUTOR(C_Cc_ServerInfo);
    MESSAGE_EXECUTOR(C_Cc_ServerLost);
    MESSAGE_EXECUTOR(C_Cc_ServerOk);
    MESSAGE_EXECUTOR(C_Cc_Stop);
}



#endif      // end of _CC_MESSAGE_EXECUTORS_H_