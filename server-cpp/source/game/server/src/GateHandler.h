//////////////////////////////////////////////////////////////////////////
//
// 文件：source/game/server/src/GateHandler.h
// 作者：Xoen Xie
// 时间：2020/02/29
// 描述：网关句柄
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _GATE_HANDLER_H_
#define _GATE_HANDLER_H_

#include "IGateAcceptor.h"

class GateHandler : public GateA::IGateHandler
{
public:
    void OnClientClose(uint32 nClientID);
};


#endif      // end of _GATE_HANDLER_H_