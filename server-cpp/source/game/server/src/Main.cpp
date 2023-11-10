//////////////////////////////////////////////////////////////////////////
//
// 文件：source/game/server/src/Main.cpp
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
#include "IGateAcceptor.h"
#include "GateHandler.h"
#include "ActorManager.h"

int main(int argc, char* argv[])
{
	XSFCore::GetServer()->Init(EP_Game, argc, argv);

	xsf_msg::SetMessageModule();
	xsf_scp::SetSchemaModule();

	CC::SetCenterConnector(GameModule_CC, new ServerInfoHandler());
	GateA::SetGateAcceptor(GameModule_GateA, new GateHandler());
	SetActorManager();

	XSFCore::GetServer()->Run();

	return 0;
}