//////////////////////////////////////////////////////////////////////////
//
// 文件：schema\src\schemas\SchemaTestData.cpp
// 作者：Xoen Xie
// 时间：11/10/2023
// 描述：测试XML配置
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#include "SchemaTestData.h"
#include "XSF.h"

#include "tinyxml2.h"
using namespace tinyxml2;

namespace xsf_scp
{
    bool SchemaTestData::OnSchemaLoad(int32 nSchemaID, ISchemaReader *pReader)
    {
        //XMLElement *pSchemaEle = XML_FIRST_CHILD(pReader->GetXmlRoot(), nullptr);

        return true;
    }
}