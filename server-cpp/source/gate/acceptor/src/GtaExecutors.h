//////////////////////////////////////////////////////////////////////////
//
// 文件：source/gate/acceptor/src/GtaExecutors.h
// 作者：Xoen Xie
// 时间：2020/08/05
// 描述：消息执行体
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _GTA_EXECUTORS_H_
#define _GTA_EXECUTORS_H_

#include "DMessageInc.h"
using namespace xsf;

namespace GateA
{
    MESSAGE_EXECUTOR(Gt_GtA_Handshake);
    MESSAGE_EXECUTOR(Gt_GtA_Heartbeat);
    MESSAGE_EXECUTOR(Gt_GtA_ClientClose);
}

#endif // end of _GTA_MESSAGE_EXECUTORS_H_