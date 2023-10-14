//////////////////////////////////////////////////////////////////////////
// 
// 文件：Schema/SchemaModule.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：配置模块
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.Xml;
using XSF;
using Serilog;

namespace XsfScp
{
    public partial class SchemaModule : IModule
    {
        private ISchema[] m_SchemaList;

        public SchemaModule()
        {
            m_SchemaList = new ISchema[(int)SchemaID.Max];
            m_SchemaList[0] = new SchemaLoad(this);
        }

        public override bool Start() 
        {
            Log.Information("开始加载配置");
            XSFUtil.schemaHelper = new GameSchemaHelper();

            try
            {
                LoadWithSchema(0, "Load", SchemaType.XML);
            }
            catch(Exception e)
            {
                Log.Error("配置加载失败，message={0}, stack={1}", e.Message, e.StackTrace);
                return false;
            }

            return true;
        }

        internal void LoadWithSchema(int nID, string sName, SchemaType nType)
        {
            if (nID != (int)SchemaID.None)
            {
                m_SchemaList[nID] = GetSchema(nID);
            }

            sName = m_SchemaList[nID].GetSchemaName(sName);

            Log.Information("XSFSchema.LoadWithSchema load schema, name=" + sName);

            string sContent = "";

            string sSuffix = nType == SchemaType.CSV ? "csv" : "xml";
            string sFilename = $"{XSFUtil.Server.InitData.WorkDir}/scp/{sName}.{sSuffix}";
            if (!File.Exists(sFilename))
            {
                throw new XSFSchemaLoadException($"XSFSchema.LoadWithSchema file not exist, id={nID}, Name={sFilename}");
            }

            sContent = File.ReadAllText(sFilename);
            
            LoadWithReader(nID, sName, m_SchemaList[nID], nType, sContent);
        }

        // 针对具体的配置类型，使用对应的Reader进行加载
        private void LoadWithReader(int nID, string sName, ISchema schema, SchemaType nType, string sContent)
        {
            ISchemaReader? reader = null;
            switch (nType)
            {
                case SchemaType.CSV: reader = new CSVReader(); break;
                case SchemaType.XML: reader = new XMLReader(); break;
                default:
                    throw new XSFSchemaLoadException($"XSFSchema.LoadWithReader OnSchemaLoad return false type error, id={nID}, name={sName}, type={nType}");
            }

            reader.Read(sContent);

            schema.OnSchemaLoad(reader);
        }

        public static void CreateModule()
        {
            SchemaModule module = new SchemaModule();

            ModuleInit init = new ModuleInit();
            init.ID = 0;
            init.Name = "Schema";
            init.NoWaitStart = true;

            XSFUtil.Server.AddModule(module, init);
        }
    }
}