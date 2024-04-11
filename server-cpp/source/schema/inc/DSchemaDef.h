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
#include "CSVData_Array.h"
#include "CSVData_IDAndCount.h"

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
	float fFloatData;	//浮点数
	CSVArray arUintArrayData;	//无符号整形数组
	bool bBoolData;	//布尔数据
	CSVIdCountArray icIacData;	//id和count数据
	uint32 uEnergyItem;	//体力道具ID
//ScpGlobal_END
};

//物品配置
struct ScpItem
{
//ScpItem_START
	uint32 uId;	//编号
	int32 iType;	//类型
	float fParam1;	//参数1
	uint64 ulParam2;	//参数2
	CSVArray arParam3;	//参数3
	CSVIdCountArray icParam4;	//参数4
	bool bParam5;	//参数5
//ScpItem_END
};

//测试XML配置
struct ScpTestData
{
//ScpTestData_START
//ScpTestData_END
};

//DB sql 配置
struct ScpDBSql
{
//ScpDBSql_START
//ScpDBSql_END
};
//SCHEMA_STRUCT
// 不要删除上面的标签，这是用来自动生成代码

}

#endif      // end of _D_SCHEMA_DEF_H_
