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
#include "SchemaIndex.h"

#include "XSF.h"
#include "CSVDataInc.h"

namespace xsf_scp
{
    bool _SCHEMA_NAME_::OnSchemaLoad(int32 nSchemaID, ISchemaReader *pReader)
    {
        请补充数据管理代码，用map或这用vector，具体请根据需求补充
        for(uint32 i = 0; i < pReader->RowCount(); i ++)
        {
            _SCP_NAME_ * scp = new _SCP_NAME_();
            //_CSV_LIST_BEGIN_

            //_CSV_LIST_END_
        }

        return true;
    }

    void *_SCHEMA_NAME_::Get(uint32 nID1, uint32 nID2)
    {
        请补充获取数据代码
    }
}