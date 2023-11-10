//////////////////////////////////////////////////////////////////////////
//
// 文件：source/game/server/src/actor/ActorManager.h
// 作者：Xoen Xie
// 时间：2020/02/29
// 描述：玩家管理
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _ACTOR_MANAGER_H_
#define _ACTOR_MANAGER_H_

#include "XSF.h"
#include "Singleton.h"
using namespace xsf;

class Actor;

class ActorManager : public IModule, public Singleton<ActorManager>
{
    using ActorMap = unordered_map<uint32, Actor*>;
public:
    void DoRegist(void) override;

public:
    Actor * DoLogin(uint32 nClientID);
    void OnActorLogout(uint32 nClientID);

private:
    ActorMap m_Actors;
    ActorMap m_ClientActors;

    uint32 m_nGlobalActorID = 1;
};

void SetActorManager(void);






#endif      // end of _ACTOR_MANAGER_H_