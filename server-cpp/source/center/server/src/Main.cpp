//////////////////////////////////////////////////////////////////////////
//
// 文件：center\server\src\Main.cpp
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
#include "NodeManager.h"

int main(int argc, char* argv[])
{
	XSFCore::GetServer()->Init(EP_Center, argc, argv);

	xsf_msg::SetMessageModule();
	xsf_scp::SetSchemaModule();
	SetNodeManager();

	XSFCore::GetServer()->Run();

	return 0;
}