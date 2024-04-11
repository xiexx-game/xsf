//////////////////////////////////////////////////////////////////////////
//
// 文件：schema\src\schemas\SchemaDBSql.cpp
// 作者：Xoen Xie
// 时间：2024/4/10
// 描述：DB sql 配置
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#include "SchemaDBSql.h"
#include "XSF.h"

#include "tinyxml2.h"
using namespace tinyxml2;

namespace xsf_scp
{
    bool SchemaDBSql::OnSchemaLoad(int32 nSchemaID, ISchemaReader *pReader)
    {
        //请补充数据读取代码
        //XMLElement *pSchemaEle = XML_FIRST_CHILD(pReader->GetXmlRoot(), nullptr);

        return true;
    }
}