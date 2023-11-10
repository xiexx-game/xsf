//////////////////////////////////////////////////////////////////////////
//
// 文件：schema\src\SchemaModule.h
// 作者：Xoen Xie
// 时间：2020/05/21
// 描述：配置模块
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _SCHEMA_MODULE_H_
#define _SCHEMA_MODULE_H_

#include "DSchemaID.h"
#include "XSF.h"
using namespace xsf;

namespace xsf_scp
{
    class SchemaModule : public IModule, public ISchemaHelper
    {
    public:
        SchemaModule(void) {}
        ~SchemaModule(void) {}

        bool LoadSchema(uint32 nSchemaID, const char *sName, uint8 nType);

    public:
        bool Start(void) override;

        void Release(void) override;

    public:
        ICSVData *GetData(byte type) override;

        ISchema *GetSchema(uint32 nID) override;

    private:
        ISchema *m_SchemaList[SchemaID_Max] = {nullptr};
    };
}

#endif // end of _SCHEMA_MODULE_H_
