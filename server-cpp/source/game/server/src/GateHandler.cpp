//////////////////////////////////////////////////////////////////////////
//
// 文件：source/game/server/src/GateHandler.cpp
// 作者：Xoen Xie
// 时间：2020/02/29
// 描述：网关句柄
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#include "GateHandler.h"
#include "XSF.h"
using namespace xsf;
#include "ActorManager.h"

void GateHandler::OnClientClose(uint32 nClientID)
{
    XSF_INFO("GateHandler::OnClientClose client id=%u", nClientID);
    ActorManager::Instance()->OnActorLogout(nClientID);
}