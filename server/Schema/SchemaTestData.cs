//////////////////////////////////////////////////////////////////////////
// 
// 文件：Schema/SchemaTestData.cs
// 作者：Xoen Xie
// 时间：2023/6/19
// 描述：测试XML配置
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8600, CS8602, CS8603, CS8618
using XSF;

namespace XsfScp
{
    public class SchemaTestData : ISchema
    {
        public ScpTestData mData { get; private set; }

        public string GetSchemaName(string name)
        {
            return name;
        }
        
        public void OnSchemaLoad(ISchemaReader reader)
        {
            mData = new ScpTestData();

            XMLReader xml = reader as XMLReader;
            //XmlNodeList nodeList = xml.mRootNode.SelectSingleNode("Common").ChildNodes;
            
        }
    }    
}
