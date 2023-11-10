//////////////////////////////////////////////////////////////////////////
//
// 文件：source/gate/server/src/connector/ServerConnector.h
// 作者：Xoen Xie
// 时间：2020/08/06
// 描述：连接器
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _SERVER_CONNECTOR_H_
#define _SERVER_CONNECTOR_H_

#include "NetConnector.h"
using namespace xsf;


class ServerConnector : public NetConnector
{
public:
    ServerConnector(void) {}
    ~ServerConnector(void) {}

protected:
    void SendHandshake(void) override;
    void SendHeartbeat(void) override;

    void OnNetError(void) override;
};



#endif      // end of _SERVER_CONNECTOR_H_