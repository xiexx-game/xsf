//////////////////////////////////////////////////////////////////////////
//
// 文件：source/gate/server/src/ServerInfoHandler.cpp
// 作者：Xoen Xie
// 时间：2020/10/12
// 描述：中心服处理器
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#include "ServerInfoHandler.h"
#include "XSF.h"
using namespace xsf;
#include "ConnectorManager.h"

void ServerInfoHandler::OnServerNew(ServerInfo *pInfo)
{

}

void ServerInfoHandler::OnServerLost(uint nID)
{

}

void ServerInfoHandler::OnServerOk(ServerInfo *pInfo)
{
    SID sid;
    sid.ID = pInfo->ID;
    if(sid.S.type == EP_Game)
    {
        ConnectorManager::Instance()->CreateConnector(pInfo->ID, pInfo->sIP, pInfo->Ports[EP_Gate]);
    }
}
