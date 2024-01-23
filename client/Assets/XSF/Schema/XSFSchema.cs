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
using UnityEngine.AddressableAssets;
using System.IO;
using System;
using UnityEngine.ResourceManagement.AsyncOperations;

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

#if UNITY_EDITOR
            if (Helper.LoadScpInFiles)
            {
                Debug.Log("XSFSchema.StartLoad Load in files");
                StartLoadSchema();      // 从文件中直接加载配置
            }
            else
            {
#endif
                Debug.Log("XSFSchema.StartLoad Load in aas");
                LoadScpFromAAS();     // 先从AAS中加载配置资源

#if UNITY_EDITOR
            }
#endif
        }

        AsyncOperationHandle<IList<TextAsset>> m_LoadHandle;
        // 从AAS中加载配置资源
        void LoadScpFromAAS()
        {
            Debug.Log("XSFSchema LoadScpFromAAS start");
            m_ScpContent = new Dictionary<string, string>();

            m_LoadHandle = Addressables.LoadAssetsAsync<TextAsset>("scp", OnPerComplete, true);
            m_LoadHandle.Completed += op => { OnAssetsLoadDone(); };
        }

        // 有配置资源加载完了
        private void OnPerComplete(TextAsset text)
        {
            m_ScpContent.Add(text.name, text.text);
        }

        // 配置资源全部加载完了
        private void OnAssetsLoadDone()
        {
            Debug.Log("XSFSchema.OnAssetsLoadDone all scp assets load done ....");

            StartLoadSchema();

            Addressables.Release(m_LoadHandle);
            m_LoadHandle = default;
        }

        // 开始加载配置
        private void StartLoadSchema()
        {
            LoadWithSchema(0, "Load", SchemaType.XML, false);

            IsUpdateWroking = true;
            XSFUpdate.Instance.Add(this);
        }

        // 获取一个配置对象
        public T Get<T>(int id) where T : class, ISchema
        {
            if (m_SchemaList == null)
                return null;

            return m_SchemaList[id] as T;
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

#if UNITY_EDITOR
                if (!Helper.LoadScpInFiles)
                {
#endif
                    if (m_LoadHandle.IsValid())
                        Addressables.Release(m_LoadHandle);

                    m_LoadHandle = default;
#if UNITY_EDITOR
                }
#endif

                Debug.Log("All Schema Load done ...");

                XSFEvent.Instance.Fire(XSFCore.SCHEMA_EVENT_ID);
            }
        }
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

            string sContent = null;

#if UNITY_EDITOR
            if (Helper.LoadScpInFiles)
            {
                string sSuffix = nType == SchemaType.CSV ? "csv" : "xml";
                string sFilename = $"{Application.dataPath}/Scp/{sName}.{sSuffix}";
                if (!File.Exists(sFilename))
                {
                    throw new XSFSchemaLoadException($"XSFSchema.LoadWithSchema file not exist, id={nID}, Name={sName}");
                }

                sContent = File.ReadAllText(sFilename);
            }
            else
            {
#endif
                if (m_ScpContent.TryGetValue(sName, out sContent))
                {
                    m_ScpContent.Remove(sName);
                }
                else
                {
                    throw new XSFSchemaLoadException($"XSFSchema.LoadWithSchema Schema asset not found, id={nID}, Name={sName}");
                }

#if UNITY_EDITOR
            }
#endif

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