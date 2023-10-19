//////////////////////////////////////////////////////////////////////////
// 
// 文件：XSF/Schema/ISchema.cs
// 作者：Xoen Xie
// 时间：2023/02/08
// 描述：配置相关定义
// 说明：
//
//////////////////////////////////////////////////////////////////////////

namespace XSF
{
    public interface ISchema
    {
        string GetSchemaName(string name);

        void OnSchemaLoad(ISchemaReader reader);
    };

    public interface ISchemaHelper
    {
        ICSVData GetData(int type);

        ISchema GetSchema(int nID);
    }

    public interface ICSVData
    {
        void Read(int nRow, int nCol, string sData);
    }

    public class XSFSchemaLoadException : ApplicationException//由用户程序引发，用于派生自定义的异常类型
    {
        public XSFSchemaLoadException() { }
        public XSFSchemaLoadException(string message)
            : base(message) { }
    }
}

