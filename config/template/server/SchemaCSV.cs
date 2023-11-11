//////////////////////////////////////////////////////////////////////////
// 
// 文件：Schema/_SCHEMA_NAME_.cs
// 作者：_SCHEMA_AUTHOR_
// 时间：_SCHEMA_DATE_
// 描述：_SCHEMA_DESC_
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8600, CS8602, CS8603, CS8618
using XSF;

namespace XsfScp
{
    public class _SCHEMA_NAME_ : ISchema
    {
        public string GetSchemaName(string name)
        {
            return name;
        }

        public void OnSchemaLoad(ISchemaReader reader)
        {
            CSVReader csv = reader as CSVReader;

            请补充数据管理代码
            for (int i = 0; i < csv.mRowCount; ++i)
            {
                ScpItem scp = new ScpItem();

//_CSV_LIST_BEGIN_
//_CSV_LIST_END_
            }
        }
    }
}

