//////////////////////////////////////////////////////////////////////////
//
// 文件：schema\src\schemas\_SCHEMA_NAME_.cpp
// 作者：_SCHEMA_AUTHOR_
// 时间：_SCHEMA_DATE_
// 描述：_SCHEMA_DESC_
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#include "_SCHEMA_NAME_.h"
#include "XSF.h"

#include "tinyxml2.h"
using namespace tinyxml2;

namespace xsf_scp
{
    bool _SCHEMA_NAME_::OnSchemaLoad(int32 nSchemaID, ISchemaReader *pReader)
    {
        请补充数据读取代码
        //XMLElement *pSchemaEle = XML_FIRST_CHILD(pReader->GetXmlRoot(), nullptr);

        return true;
    }
}