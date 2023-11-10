//////////////////////////////////////////////////////////////////////////
//
// 文件：source/game/server/src/ServerInfoHandler.h
// 作者：Xoen Xie
// 时间：2020/10/12
// 描述：中心服处理器
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _CENTER_HANDLER_H_
#define _CENTER_HANDLER_H_

#include "ICenterConnector.h"
using namespace CC;

class ServerInfoHandler : public IServerInfoHandler
{
public:
    void OnServerNew(ServerInfo *pInfo) override; // 有一个服务器连入到集群
    void OnServerLost(uint nID) override;         // 有一个服务器断开
    void OnServerOk(ServerInfo *pInfo) override;
};




#endif      // end of _CENTER_HANDLER_H_