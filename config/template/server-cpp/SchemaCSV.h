//////////////////////////////////////////////////////////////////////////
//
// 文件：schema\src\schemas\_SCHEMA_NAME_.h
// 作者：_SCHEMA_AUTHOR_
// 时间：_SCHEMA_DATE_
// 描述：_SCHEMA_DESC_
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef __SCHEMA_NAME__H_
#define __SCHEMA_NAME__H_

#include "DSchemaInc.h"
#include <unordered_map>
#include <vector>

#include "IXSFSchema.h"
using namespace xsf;

namespace xsf_scp
{
    class _SCHEMA_NAME_ : public ISchema
    {
        using DataVct = vector<_SCP_NAME_*>;
        using DataMap = unordered_map<uint32, _SCP_NAME_*>;
    public:
        bool OnSchemaLoad(int32 nSchemaID, ISchemaReader *pReader) override;

        void *Get(uint32 nID1 = 0, uint32 nID2 = 0) override;

    private:
        //DataVct m_Datas;
        //DataMap m_Datas;
    };
}

#endif // end of _SchemaGlobal_H_