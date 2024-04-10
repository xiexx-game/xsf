//////////////////////////////////////////////////////////////////////////
// 
// 文件：Schema/SchemaDBSql.cs
// 作者：Xoen Xie
// 时间：4/9/2024
// 描述：DB sql 配置
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8600, CS8602, CS8603, CS8618, CS8604
using XSF;
using System.Xml;

namespace XsfScp
{
    public class SchemaDBSql : ISchema
    {
        Dictionary<uint, ScpDBSql> m_Datas;

        public string GetSchemaName(string name)
        {
            return name;
        }

        public ScpDBSql Get(uint nID)
        {
            if(m_Datas.TryGetValue(nID, out ScpDBSql scp ))
                return scp;

            return null;
        }
        
        public void OnSchemaLoad(ISchemaReader reader)
        {
            m_Datas = new Dictionary<uint, ScpDBSql>();

            XMLReader xml = reader as XMLReader;
            XmlNodeList dbList = xml.mRootNode.ChildNodes;
            for(int i = 0; i < dbList.Count; i ++)
            {
                var dbEle = dbList[i] as XmlElement;
                string dbName = XMLReader.GetString(dbEle, "name");

                var sqlList = dbEle.ChildNodes;

                Serilog.Log.Information("db=" + dbName + ", sql count=" + sqlList.Count);
                for(int J = 0; J < sqlList.Count; J ++)
                {
                    var sqlEle = sqlList[J] as XmlElement;
                    ScpDBSql scp = new ScpDBSql();
                    scp.sDBName = dbName;
                    scp.uID = XMLReader.GetUInt(sqlEle, "id");

                    if (m_Datas.ContainsKey(scp.uID))
                    {
                        throw new XSFSchemaLoadException("SchemaDBSql.OnSchemaLoad key exist, id=" + scp.uID);
                    }

                    scp.sContent = XMLReader.GetString(sqlEle, "content");

                    Serilog.Log.Information(scp.sContent);

                    var inNode = sqlEle.SelectSingleNode("in");
                    if(inNode != null)
                    {
                        var ins = inNode.ChildNodes;
                        if(ins.Count > 0)
                        {
                            scp.inParams = new string[ins.Count];
                            for(int Z = 0; Z < ins.Count; Z ++)
                            {
                                var inEle = ins[Z] as XmlElement;
                                scp.inParams[Z] = XMLReader.GetString(inEle, "name");
                                Serilog.Log.Information("in:" + scp.inParams[Z]);
                            }
                        }
                    }
                    
                    var outNode = sqlEle.SelectSingleNode("out");
                    if(outNode != null)
                    {
                        var outs = outNode.ChildNodes;
                        if(outs.Count > 0)
                        {
                            scp.outParams = new string[outs.Count];
                            for(int Z = 0; Z < outs.Count; Z ++)
                            {
                                var outEle = outs[Z] as XmlElement;
                                scp.outParams[Z] = XMLReader.GetString(outEle, "name");
                                Serilog.Log.Information("out:" + scp.outParams[Z]);
                            }
                        }
                    }
                    

                    m_Datas.Add(scp.uID, scp);
                }
            }
            
        }
    }    
}
