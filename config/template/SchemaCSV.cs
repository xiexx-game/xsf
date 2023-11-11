//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\Scripts\Schema\_SCHEMA_NAME_.cs
// 作者：_SCHEMA_AUTHOR_
// 时间：_SCHEMA_DATE_
// 描述：_SCHEMA_DESC_
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using XSF;

namespace XsfScp
{
public class _SCHEMA_NAME_ : ISchema
{
    public string GetSchemaName(string name)
    {
        return name;
    }

    public bool OnSchemaLoad(ISchemaReader reader)
    {
        CSVReader csv = reader as CSVReader;

        请补充数据管理代码
        for (int i = 0; i < csv.mRowCount; ++i)
        {
            _SCP_NAME_ scp = new _SCP_NAME_();

//_CSV_LIST_BEGIN_
//_CSV_LIST_END_
        }

        return true;
    }
}
}