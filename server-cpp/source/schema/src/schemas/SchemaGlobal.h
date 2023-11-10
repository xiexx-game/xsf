//////////////////////////////////////////////////////////////////////////
//
// 文件：schema\src\schemas\SchemaGlobal.h
// 作者：Unity editor
// 时间：2020/09/16
// 描述：全局变量配置
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _SchemaGlobal_H_
#define _SchemaGlobal_H_

#include "DSchemaInc.h"
#include <unordered_map>
#include <vector>

#include "IXSFSchema.h"
using namespace xsf;

namespace xsf_scp
{
    class SchemaGlobal : public ISchema
    {
    public:
        bool OnSchemaLoad(int32 nSchemaID, ISchemaReader *pReader) override;

        void *Get(uint32 nID1 = 0, uint32 nID2 = 0) override { return &GlobalData; }

    private:
        ScpGlobal GlobalData;
    };
}

#endif // end of _SchemaGlobal_H_