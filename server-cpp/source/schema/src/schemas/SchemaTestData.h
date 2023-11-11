//////////////////////////////////////////////////////////////////////////
//
// 文件：schema\src\schemas\SchemaTestData.h
// 作者：Xoen Xie
// 时间：11/10/2023
// 描述：测试XML配置
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _SchemaTestData_H_
#define _SchemaTestData_H_

#include "DSchemaInc.h"
#include "IXSFSchema.h"
using namespace xsf;

namespace xsf_scp
{
    class SchemaTestData : public ISchema
    {
    public:
        bool OnSchemaLoad(int32 nSchemaID, ISchemaReader *pReader) override;

        void * Get(uint32 nID1 = 0, uint32 nID2 = 0) { return nullptr; }
    };
}

#endif // end of _SchemaTestData_H_