//////////////////////////////////////////////////////////////////////////
//
// 文件：source/gate/server/src/Main.cpp
// 作者：Xoen Xie
// 时间：2020/06/03
// 描述：入口
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#include "XSF.h"
#include "Server.h"

#include "DSchemaInc.h"
#include "DMessageInc.h"
using namespace xsf;

#include "ICenterConnector.h"
#include "ServerInfoHandler.h"
#include "ConnectorManager.h"
#include "ClientManager.h"

int main(int argc, char* argv[])
{
	XSFCore::GetServer()->Init(EP_Gate, argc, argv);

	xsf_msg::SetMessageModule();
	xsf_scp::SetSchemaModule();

	CC::SetCenterConnector(GateModule_CC, new ServerInfoHandler());
	SetClientManager();
	SetConnectorManager();

	XSFCore::GetServer()->Run();

	return 0;
}