//////////////////////////////////////////////////////////////////////////
//
// 文件：source/game/server/src/actor/Actor.h
// 作者：Xoen Xie
// 时间：2020/02/29
// 描述：玩家对象
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _ACTOR_H_
#define _ACTOR_H_

#include "XSF.h"
using namespace xsf;

class Actor
{
    friend class ActorManager;
public:
    JEMALLOC_HOOK

    Actor(uint32 nID, uint32 nClientID)
        : m_nID(nID)
        , m_nClientID(nClientID)
    {

    }

    void Release(void)
    {
        delete this;
    }

    uint32 GetID(void) { return m_nID; }
    uint32 GetClientID(void) { return m_nClientID; }

    void OnLoginOK(void);

private:
    uint32 m_nID;
    uint32 m_nClientID;
};





#endif      // end of _ACTOR_H_