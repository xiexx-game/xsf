//////////////////////////////////////////////////////////////////////////
//
// 文件：source/gate/server/inc/Server.h
// 作者：Xoen Xie
// 时间：2020/08/05
// 描述：服务器接口相关
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _SERVER_H_
#define _SERVER_H_

#include "XSF.h"
using namespace xsf;

enum GateModule
{
    GateModule_CC = DEFAULT_START_MODULE_ID,
    GateModule_Client,
    GateModule_Connector,
};


#endif      // end of _SERVER_H_