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
using UnityEngine;

namespace XSF
{
    public sealed class SchemaLoad : ISchema
    {
        struct SchemaLoadInfo
        {
            public int nID;
            public string sName;
            public SchemaType nType;
        }

        private List<SchemaLoadInfo> m_LoadInfos;

        public SchemaLoad()
        {

        }

        public int ID { get { return 0; } }

        public string GetSchemaName(string name)
        {
            return name;
        }

        public bool OnSchemaLoad(ISchemaReader reader)
        {
            m_LoadInfos = new List<SchemaLoadInfo>();

            XMLReader xml = reader as XMLReader;
            XmlNodeList nodeList = xml.mRootNode.ChildNodes;
            for (int i = 0; i < nodeList.Count; ++i)
            {
                XmlElement ele = nodeList[i] as XmlElement;

                int nID = XMLReader.GetInt(ele, "id");
                string sName = XMLReader.GetString(ele, "name");
                SchemaType nType = (SchemaType)XMLReader.GetUInt(ele, "type");
                int nLoad = XMLReader.GetInt(ele, "client");
                if (nLoad > 0)
                {
                    SchemaLoadInfo info;
                    info.nID = nID;
                    info.sName = sName;
                    info.nType = nType;
                    m_LoadInfos.Add(info);
                }
            }

            return true;
        }

        public bool LoadNextSchema()
        {
            SchemaLoadInfo info = m_LoadInfos[0];
            m_LoadInfos.RemoveAt(0);

            int nID = info.nID;
            string sName = info.sName;
            SchemaType nType = info.nType;

            Debug.Log("Start load, schema=" + sName);

            XSFSchema.Instance.LoadWithSchema(nID, sName, nType);

            Debug.Log("Load schema done, schema=" + sName);

            if (m_LoadInfos.Count > 0)
                return true;

            return false;
        }
    }
}