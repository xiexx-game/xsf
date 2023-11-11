//////////////////////////////////////////////////////////////////////////
//
// 文件：schema\inc\DSchemaID.h
// 作者：Xoen Xie
// 时间：2020/05/14
// 描述：配置ID
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _D_SCHEMA_ID_H_
#define _D_SCHEMA_ID_H_


// 该文件不需要手动修改，请在Unity中使用工具导出代码
namespace xsf_scp
{

// 配置ID
enum EMSchemaID
{
    SchemaID_Load = 0,

//SCHEMA_ID_BEGIN
	SchemaID_Global = 1,
	SchemaID_Item = 2,
	SchemaID_TestData = 4,
//SCHEMA_ID_END

    SchemaID_Max,
};


//SCHEMA_CREATE_BEGIN
#define SCHEMA_CREATE				\
	case SchemaID_Global:		m_SchemaList[nSchemaID] = new SchemaGlobal();	break;			\
	case SchemaID_Item:		m_SchemaList[nSchemaID] = new SchemaItem();	break;			\
	case SchemaID_TestData:		m_SchemaList[nSchemaID] = new SchemaTestData();	break;			\
//SCHEMA_CREATE_END
}

    





#endif      // end of _D_SCHEMA_ID_H_
