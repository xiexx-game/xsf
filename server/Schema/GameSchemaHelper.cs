//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\Scripts\Schema\GameSchemaHelper.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：配置助手
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8618
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
				case SchemaID.Global: Global = new SchemaGlobal(); return Global;
				case SchemaID.Item: Item = new SchemaItem(); return Item;
				case SchemaID.TestData: TestData = new SchemaTestData(); return TestData;
				case SchemaID.DBSql: DBSql = new SchemaDBSql(); return DBSql;
                //SCHEMA_END
                default:
                    throw new XSFSchemaLoadException($"SchemaModule GetSchema schema null, id={(SchemaID)nId}");
            }
        }

        public ICSVData GetData(int type)
        {
            return CSVData.GetData(type);
        }

        //SCHEMA_STATIC_BEGIN
		public static SchemaGlobal Global { get; private set;}
		public static SchemaItem Item { get; private set;}
		public static SchemaTestData TestData { get; private set;}
		public static SchemaDBSql DBSql { get; private set;}
        //SCHEMA_STATIC_END
    }
}


