//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\Scripts\Schema\GameSchemaHelper.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：配置助手
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using System;
using XSF;

namespace XsfScp
{
    public enum SchemaID
    {
        None = 0,

    //SCHEMA_ID_BEGIN
		Global = 1,
		Item = 2,
		TestData = 4,
		DBSql = 5,
    //SCHEMA_ID_END

        Max,
    }

    public partial class SchemaModule : ISchemaHelper
    {
        public ISchema GetSchema(int nId)
        {
            if(m_SchemaList[nId] != null)
            {
                return m_SchemaList[nId];
            }

            switch ((SchemaID)nId)
            {
                //SCHEMA_BEGIN
				case SchemaID.Global: return new SchemaGlobal();
				case SchemaID.Item: return new SchemaItem();
				case SchemaID.TestData: return new SchemaTestData();
				case SchemaID.DBSql: return new SchemaDBSql();
                //SCHEMA_END
                default:
                    throw new XSFSchemaLoadException($"SchemaModule GetSchema schema null, id={(SchemaID)nId}");
            }
        }

        public ICSVData GetData(int type)
        {
            return CSVData.GetData(type);
        }
    }
}


