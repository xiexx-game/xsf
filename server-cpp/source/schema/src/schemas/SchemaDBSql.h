//////////////////////////////////////////////////////////////////////////
//
// 文件：schema\src\schemas\SchemaDBSql.h
// 作者：Xoen Xie
// 时间：2024/4/10
// 描述：DB sql 配置
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _SchemaDBSql_H_
#define _SchemaDBSql_H_

#include "DSchemaInc.h"
#include "IXSFSchema.h"
using namespace xsf;

namespace xsf_scp
{
    class SchemaDBSql : public ISchema
    {
    public:
        bool OnSchemaLoad(int32 nSchemaID, ISchemaReader *pReader) override;

        void * Get(uint32 nID1 = 0, uint32 nID2 = 0) { return nullptr; }
    };
}

#endif // end of _SchemaDBSql_H_