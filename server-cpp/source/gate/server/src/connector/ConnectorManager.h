//////////////////////////////////////////////////////////////////////////
//
// 文件：source/gate/server/src/connector/ConnectorManager.h
// 作者：Xoen Xie
// 时间：2020/08/06
// 描述：connector 管理
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _CONNECTOR_MANAGER_H_
#define _CONNECTOR_MANAGER_H_

#include "XSF.h"
using namespace xsf;

#include <unordered_map>
#include <vector>
#include "Singleton.h"


class ServerConnector;
class Client;

class ConnectorManager : public IModule, public Singleton<ConnectorManager>
{
    using ConnMap = unordered_map<uint32, ServerConnector*>;
    using ConnVct = vector<ServerConnector*>;
public:
    ConnectorManager(void) {}
    ~ConnectorManager(void) {}

    void CreateConnector(uint32 nID, const char * ip, uint32 port);

    void DeleteConnector(uint32 nID);

    ServerConnector * GetConnector(uint8 nEP, uint32 nID);

public:
    void DoRegist() override;

    void DoClose(void) override;
    
private:
    ConnMap m_Connectors;
    ConnVct m_EPConnectors[EP_Max];
};

void SetConnectorManager(void);




#endif      // end of _CONNECTOR_MANAGER_H_