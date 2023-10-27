//////////////////////////////////////////////////////////////////////////
// 
// 文件：Scripts\Schema\_SCHEMA_NAME_.cs
// 作者：_SCHEMA_AUTHOR_
// 时间：_SCHEMA_DATE_
// 描述：_SCHEMA_DESC_
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.Xml;
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

    public bool OnSchemaLoad(ISchemaReader reader)
    {
        mData = new _SCP_NAME_();

        XMLReader xml = reader as XMLReader;
        //XmlNodeList nodeList = xml.mRootNode.SelectSingleNode("Common").ChildNodes;
        
        return true;
    }
}
}