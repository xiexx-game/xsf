//////////////////////////////////////////////////////////////////////////
//
// 文件：schema\src\schemas\SchemaLoad.h
// 作者：Xoen Xie
// 时间：2020/05/28
// 描述：加载配置
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _SCHEMA_LOAD_H_
#define _SCHEMA_LOAD_H_

#include "DSchemaInc.h"
#include "IXSFSchema.h"
using namespace xsf;

namespace xsf_scp
{
    class SchemaModule;

    class SchemaLoad : public ISchema
    {
    public:
        void SetModule(SchemaModule *pModule)
        {
            m_pModule = pModule;
        }

    public:
        bool OnSchemaLoad(int32 nSchemaID, ISchemaReader *pReader) override;

        void * Get(uint32 nID1 = 0, uint32 nID2 = 0) { return nullptr; }

    private:
        SchemaModule *m_pModule = nullptr;
    };
}

#endif // end of _SCHEMA_LOAD_H_