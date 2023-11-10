//////////////////////////////////////////////////////////////////////////
//
// 文件：source/game/server/src/actor/ActorManager.cpp
// 作者：Xoen Xie
// 时间：2020/02/29
// 描述：玩家管理
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#include "ActorManager.h"
#include "Actor.h"
#include "ActorExecutors.h"
#include "DMessageInc.h"
#include "Server.h"

void ActorManager::DoRegist(void)
{
    XSFCore::SetMessageExecutor(xsf_pbid::Clt_G_Login, new Executor_Clt_G_Login());
}

Actor * ActorManager::DoLogin(uint32 nClientID)
{
    XSF_INFO("ActorManager::DoLogin nClientID=%u", nClientID);
    auto it = m_ClientActors.find(nClientID);
    if(it != m_ClientActors.end())
    {
        return it->second;
    }
    else
    {
        Actor * pActor = new Actor(m_nGlobalActorID++, nClientID);
        m_Actors.insert(ActorMap::value_type(pActor->m_nID, pActor));
        m_ClientActors.insert(ActorMap::value_type(nClientID, pActor));
        return pActor;
    }
}

void ActorManager::OnActorLogout(uint32 nClientID)
{
    auto it = m_ClientActors.find(nClientID);
    if(it != m_ClientActors.end())
    {
        auto pActor = it->second;
        m_ClientActors.erase(it);
        m_Actors.erase(pActor->m_nID);

        XSF_INFO("ActorManager::OnActorLogout actor=%u, client id=%u", pActor->m_nID, nClientID);
        
        pActor->Release();
    }
}


void SetActorManager(void)
{
    ModuleInit * pInit = new ModuleInit();
    pInit->nModuleID = GameModule_Actor;
    strcpy(pInit->sName, "ActorManager");

    XSFCore::GetServer()->AddModule(ActorManager::Instance(), pInit);
}