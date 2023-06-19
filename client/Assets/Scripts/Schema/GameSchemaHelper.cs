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

public enum SchemaID
{
    None = 0,

//SCHEMA_ID_BEGIN
	Global = 1,
	Item = 2,
	TestData = 3,
//SCHEMA_ID_END

    Max,
}

public class GameSchemaHelper : ISchemaHelper
{
    public ISchema Get(int nId)
    {
        switch ((SchemaID)nId)
        {
            //SCHEMA_BEGIN
			case SchemaID.Global: return new SchemaGlobal();
			case SchemaID.Item: return new SchemaItem();
			case SchemaID.TestData: return new SchemaTestData();
			//SCHEMA_END
            default:
                throw new XSFSchemaLoadException($"GameSchemaHelper.Get schema id error, id={nId}");
        }
    }

    public int MaxID { get { return (int)SchemaID.Max; } }

    public ICSVData GetData(int type)
    {
        return CSVData.GetData(type);
    }
}
