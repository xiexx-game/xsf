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
#include "IXSFSchema.h"
using namespace xsf;

namespace xsf_scp
{
    class _SCHEMA_NAME_ : public ISchema
    {
    public:
        bool OnSchemaLoad(int32 nSchemaID, ISchemaReader *pReader) override;

        void * Get(uint32 nID1 = 0, uint32 nID2 = 0) { return nullptr; }
    };
}

#endif // end of __SCHEMA_NAME__H_