//////////////////////////////////////////////////////////////////////////
// 
// 文件：Scripts\Schema\SchemaTestData.cs
// 作者：Xoen Xie
// 时间：2023/6/19
// 描述：测试XML配置
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.Xml;

public class SchemaTestData : ISchema
{
    public ScpTestData mData { get; private set; }

    public bool OnSchemaLoad(ISchemaReader reader)
    {
        mData = new ScpTestData();

        XMLReader xml = reader as XMLReader;
        //XmlNodeList nodeList = xml.mRootNode.SelectSingleNode("Common").ChildNodes;
        
        return true;
    }
}