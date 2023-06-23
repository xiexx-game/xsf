//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\XSF\Schema\ISchema.cs
// 作者：Xoen Xie
// 时间：2023/02/08
// 描述：配置相关定义
// 说明：
//
//////////////////////////////////////////////////////////////////////////

public interface ISchema
{
    string GetSchemaName(string name);

    bool OnSchemaLoad(ISchemaReader reader);
};

public interface ISchemaHelper
{
    ISchema Get(int nId);

    int MaxID { get; }

    ICSVData GetData(int type);

    string GetLocalText(string key);
}

public interface ICSVData
{
    void Read(int nRow, int nCol, string sData);
}
