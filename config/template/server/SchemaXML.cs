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
        public _SCP_NAME_ mData { get; private set; }

        public string GetSchemaName(string name)
        {
            return name;
        }
        
        public void OnSchemaLoad(ISchemaReader reader)
        {
            mData = new _SCP_NAME_();

            请补充xml数据读取代码
            XMLReader xml = reader as XMLReader;
            //XmlNodeList nodeList = xml.mRootNode.SelectSingleNode("Common").ChildNodes;
            
        }
    }    
}
