//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\XSF\Schema\XSFSchema.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：配置模块
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using System;
using YooAsset;

namespace XSF
{

    public sealed class XSFSchema : Singleton<XSFSchema>, IUpdateNode
    {
        private ISchema[] m_SchemaList;
        private SchemaLoad m_SchemaLoad;

        private Dictionary<string, string> m_ScpContent;

        public ISchemaHelper Helper { get; private set; }

        public XSFSchema()
        {
            
        }

        // 开始加载配置，通常都是从这里开始出发加载配置
        public void StartLoad(ISchemaHelper helper)
        {
            Helper = helper;
            m_SchemaList = new ISchema[helper.MaxID];

            m_SchemaLoad = new SchemaLoad();
            m_SchemaList[m_SchemaLoad.ID] = m_SchemaLoad;

            LoadScp();
        }

        // 从AAS中加载配置资源
        void LoadScp()
        {
            Debug.Log("XSFSchema LoadScp start");
            m_ScpContent = new Dictionary<string, string>();

            var package = YooAssets.GetPackage(XSFConfig.Instance.YooAssetPackage);
            var handle = package.LoadAllAssetsAsync<UnityEngine.TextAsset>("Scp_Load");
            handle.Completed += OnScpLoadDone;
        }

        // 配置资源全部加载完了
        private void OnScpLoadDone(AllAssetsHandle handle)
        {
            Debug.Log("XSFSchema.OnAssetsLoadDone all scp assets load done ....");

            foreach(var assetObj in handle.AllAssetObjects)
            {    
                TextAsset textAsset = assetObj as TextAsset;
                m_ScpContent.Add(assetObj.name, textAsset.text); 
            }

            StartLoadSchema();

            handle.Release();
        }

        // 开始加载配置
        private void StartLoadSchema()
        {
            LoadWithSchema(0, "Load", SchemaType.XML, false);

            IsUpdateWroking = true;
            XSFUpdate.Instance.Add(this);
        }

        #region  UpdateNode
        public bool IsUpdateWroking
        {
            get; private set;
        }

        public void OnUpdate()
        {
            if (m_SchemaLoad.LoadNextSchema())
            {

            }
            else
            {
                IsUpdateWroking = false;

                Debug.Log("All Schema Load done ...");

                //XSFEvent.Instance.Fire(XSFCore.SCHEMA_EVENT_ID);
            }
        }

        public void OnFixedUpdate() {}
        #endregion

        // 找到配置对应的配置对象进行配置加载
        public void LoadWithSchema(int nID, string sName, SchemaType nType, bool IsColTable)
        {
            if (nID != m_SchemaLoad.ID)
            {
                m_SchemaList[nID] = Helper.Get(nID);
            }

            sName = m_SchemaList[nID].GetSchemaName(sName);

            Debug.Log("XSFSchema.LoadWithSchema load schema, name=" + sName);

            if (m_ScpContent.TryGetValue(sName, out string sContent))
            {
                m_ScpContent.Remove(sName);
            }
            else
            {
                throw new XSFSchemaLoadException($"XSFSchema.LoadWithSchema Schema asset not found, id={nID}, Name={sName}");
            }

            LoadWithReader(m_SchemaList[nID], nType, sContent, IsColTable);
        }

        // 针对具体的配置类型，使用对应的Reader进行加载
        private void LoadWithReader(ISchema schema, SchemaType nType, string sContent, bool IsColTable)
        {
            ISchemaReader reader = null;
            switch (nType)
            {
                case SchemaType.CSV: reader = new CSVReader(IsColTable); break;
                case SchemaType.XML: reader = new XMLReader(); break;
                default:
                    throw new XSFSchemaLoadException($"XSFSchema.LoadWithReader Schema Type Error, Type:{nType}");
            }

            reader.Read(sContent);

            if (!schema.OnSchemaLoad(reader))
            {
                throw new XSFSchemaLoadException($"XSFSchema.LoadWithReader OnSchemaLoad return false");
            }
        }

    }


    public class XSFSchemaLoadException : ApplicationException//由用户程序引发，用于派生自定义的异常类型
    {
        public XSFSchemaLoadException() { }
        public XSFSchemaLoadException(string message)
            : base(message) { }
    }
}