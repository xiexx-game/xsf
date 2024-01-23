//////////////////////////////////////////////////////////////////////////
//
// 文件：schema\src\schemas\SchemaLoad.cpp
// 作者：Xoen Xie
// 时间：2020/05/28
// 描述：加载配置
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#include "SchemaLoad.h"
#include "XSF.h"

#include "tinyxml2.h"
using namespace tinyxml2;

#include "SchemaModule.h"

namespace xsf_scp
{
    enum SchemaLoadFlag
    {
        SchemaLoadFlag_False = 0,
        SchemaLoadFlag_True,
    };

    bool SchemaLoad::OnSchemaLoad(int32 nSchemaID, ISchemaReader *pReader)
    {

        XMLElement *pSchemaEle = XML_FIRST_CHILD(pReader->GetXmlRoot(), nullptr);

        while (pSchemaEle != nullptr)
        {
            uint32 nLoadFlag = SchemaLoadFlag_False;
            XML_UINT_ATTR(pSchemaEle, "server", nLoadFlag);

            if (nLoadFlag == SchemaLoadFlag_True)
            {
                uint32 nSchemaID = 0;
                uint32 nSchemaType = 0;
                char sSchemaName[BUFFER_SIZE_128] = {0};
                uint32 nColTable = 0;

                XML_UINT_ATTR(pSchemaEle, "id", nSchemaID);
                XML_UINT_ATTR(pSchemaEle, "type", nSchemaType);
                XML_UINT_ATTR(pSchemaEle, "col_table", nColTable);
                XML_STRING_ATTR(pSchemaEle, "name", sSchemaName);

                if (m_pModule->LoadSchema(nSchemaID, sSchemaName, nSchemaType, nColTable > 0))
                {
                    XSF_INFO("Load schema success, schema:%s, id:%u", sSchemaName, nSchemaID);
                }
                else
                {
                    XSF_ERROR("Load schema failed, schema:%s, id:%u", sSchemaName, nSchemaID);
                    return false;
                }
            }

            pSchemaEle = XML_NEXT_SIBLING(pSchemaEle);
        }

        return true;
    }
}