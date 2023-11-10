//////////////////////////////////////////////////////////////////////////
//
// 文件：center\connector\inc\ICenterConnector.h
// 作者：Xoen Xie
// 时间：2020/06/11
// 描述：中心服连接器
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _I_CENTER_CONNECTOR_H_
#define _I_CENTER_CONNECTOR_H_

#include "XSF.h"
using namespace xsf;

namespace CC
{
    // 中心服服务器节点信息同步处理
    struct IServerInfoHandler
    {
        virtual ~IServerInfoHandler(void) {}

        virtual void OnServerNew(ServerInfo *pInfo) = 0; // 有一个服务器连入到集群
        virtual void OnServerLost(uint nID) = 0;         // 有一个服务器断开
        virtual void OnServerOk(ServerInfo *pInfo) = 0;
    };

    void SetCenterConnector(uint32 nModuleID, IServerInfoHandler *pInfoHandler);
};

#endif // end of _I_CENTER_CONNECTOR_H_