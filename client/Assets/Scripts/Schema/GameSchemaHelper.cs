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
using XsfScp;

public enum SchemaID
{
    None = 0,

//SCHEMA_ID_BEGIN
	Global = 1,
	Item = 2,
	Language = 3,
	TestData = 4,
//SCHEMA_ID_END

    Max,
}

public class GameSchemaHelper : ISchemaHelper
{
#if UNITY_EDITOR
    SchemaLanguage m_Schema;
#endif

    public ISchema Get(int nId)
    {
        switch ((SchemaID)nId)
        {
            //SCHEMA_BEGIN
			case SchemaID.Global: Global = new SchemaGlobal(); return Global;
			case SchemaID.Item: Item = new SchemaItem(); return Item;
			case SchemaID.Language: Language = new SchemaLanguage(); return Language;
			case SchemaID.TestData: TestData = new SchemaTestData(); return TestData;
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

    public string GetLocalText(string key)
    {
        return Language.Get(key);
    }

    //SCHEMA_STATIC_BEGIN
	public static SchemaGlobal Global { get; private set;}
	public static SchemaItem Item { get; private set;}
	public static SchemaLanguage Language { get; private set;}
	public static SchemaTestData TestData { get; private set;}
    //SCHEMA_STATIC_END
}
