//////////////////////////////////////////////////////////////////////////
//
// 文件：schema\src\schemas\SchemaItem.h
// 作者：Xoen Xie
// 时间：11/10/2023
// 描述：物品配置
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _SchemaItem_H_
#define _SchemaItem_H_

#include "DSchemaInc.h"
#include <unordered_map>
#include <vector>

#include "IXSFSchema.h"
using namespace xsf;

namespace xsf_scp
{
    class SchemaItem : public ISchema
    {
        using DataMap = unordered_map<uint32, ScpItem*>;
    public:
        bool OnSchemaLoad(int32 nSchemaID, ISchemaReader *pReader) override;

        void *Get(uint32 nID1 = 0, uint32 nID2 = 0) override;

    private:
        DataMap m_Datas;
    };
}

#endif // end of _SchemaGlobal_H_