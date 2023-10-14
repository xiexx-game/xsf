//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\XSF\Schema\SchemaLoad.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：主加载配置
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.Xml;
using XSF;
using Serilog;

namespace XsfScp
{
    public sealed class SchemaLoad : ISchema
    {
        private SchemaModule m_Module;

        public SchemaLoad(SchemaModule module)
        {
            m_Module = module;
        }

        public int ID { get { return 0; } }

        public string GetSchemaName(string name)
        {
            return name;
        }

        public void OnSchemaLoad(ISchemaReader reader)
        {
            XMLReader? xml = reader as XMLReader;
            XmlNodeList? nodeList = xml?.mRootNode?.ChildNodes;
            for (int i = 0; i < nodeList?.Count; ++i)
            {
                XmlElement? ele = nodeList?[i] as XmlElement;
                if(ele == null)
                {
                    throw new XSFSchemaLoadException("SchemaLoad OnSchemaLoad ele is null");
                }

                int nID = XMLReader.GetInt(ele, "id");
                string sName = XMLReader.GetString(ele, "name");
                SchemaType nType = (SchemaType)XMLReader.GetUInt(ele, "type");
                int nLoad = XMLReader.GetInt(ele, "server");
                if(nLoad > 0)
                {
                    Log.Information("Start load, schema=" + sName);

                    m_Module.LoadWithSchema(nID, sName, nType);

                    Log.Information("Load schema done, schema=" + sName);
                }
            }
        }
    }
}

