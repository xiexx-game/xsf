//////////////////////////////////////////////////////////////////////////
//
// 文件：schema\inc\DSchemaDef.h
// 作者：Xoen Xie
// 时间：2020/05/28
// 描述：配置相关定义
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _D_SCHEMA_DEF_H_
#define _D_SCHEMA_DEF_H_

#include "XSFDef.h"
using namespace xsf;

namespace xsf_scp
{

#define GET_SCHEMA_MODULE                              GET_MODULE(pSchemaModule, SCHEMA_MODULE_ID, ISchemaModule)
#define GET_SCHEMA(_DEST, _SCHEMA_ID)			       SchemaBase * _DEST = pSchemaModule->GetSchema(_SCHEMA_ID)
#define GET_SCHEMA_SCP(_SCP_CLASS, _DEST, _SCHEMA_ID, _ID0, _ID1 )   _SCP_CLASS * _DEST = static_cast<_SCP_CLASS*>(pSchemaModule->GetSchema(_SCHEMA_ID)->Get(_ID0, _ID1))


//全局变量配置
struct ScpGlobal
{
//ScpGlobal_START
	int32 iIntData;	//有符号整形
	string sStringData;	//字符串数据
	uint32 uUintData;	//uint整形
	uint64 ulUlongData;	//长整形数据
//ScpGlobal_END
};


//SCHEMA_STRUCT
// 不要删除上面的标签，这是用来自动生成代码

}

#endif      // end of _D_SCHEMA_DEF_H_
