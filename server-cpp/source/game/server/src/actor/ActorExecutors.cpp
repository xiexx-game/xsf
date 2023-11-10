//////////////////////////////////////////////////////////////////////////
//
// 文件：source/game/server/src/actor/ActorExecutors.cpp
// 作者：Xoen Xie
// 时间：2020/02/29
// 描述：玩家消息处理
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#include "ActorExecutors.h"
#include "ActorManager.h"
#include "Actor.h"

MESSAGE_EXECUTOR_EXECUTE(Clt_G_Login)
{
    Actor * pActor = ActorManager::Instance()->DoLogin(pResult->nRawID);
    pActor->OnLoginOK();
}