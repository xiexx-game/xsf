//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\XSF\XSFLua.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：xlua集成模块
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using System.IO;
using XLua;
using XLua.LuaDLL;
using UnityEngine;
using System.Collections.Generic;

using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[CSharpCallLua]
public delegate void LuaBaseFunc();

[CSharpCallLua]
public delegate void OnProtoLoadFunc(string name, byte[] data);

public sealed class XSFLua : Singleton<XSFLua>, IUpdateNode
{
    LuaEnv m_xlua;

    public LuaEnv XLua { get { return m_xlua; } }

    LuaBaseFunc m_LuaUpdate;

    LuaBaseFunc m_LuaStart;

    OnProtoLoadFunc m_ProtoLoadedFun;

    AsyncOperationHandle<IList<TextAsset>> m_LoadHandle;

    private Dictionary<string, byte[]> m_LuaContent;

    public void Init()
    {
        m_xlua = new LuaEnv();
        m_xlua.AddLoader(XSFLoader);
        m_xlua.AddBuildin("pb", Lua.LoadLuaProfobuf);
    }

    public void LuaStart()
    {
#if UNITY_EDITOR
        if (XSFConfig.Instance.LoadLuaInFiles)
        {
            XSF.Log("Lua load in files");
            Start();
        }
        else
        {
#endif
            LoadLuaFromAAS();

#if UNITY_EDITOR
        }
#endif
    }

#region LuaAssets
    void LoadLuaFromAAS()
    {
        XSF.Log("XSFLua.LoadLuaAssetsFromAAS start ....");
        m_LuaContent = new Dictionary<string, byte[]>();

        m_LoadHandle = Addressables.LoadAssetsAsync<TextAsset>("lua", OnPerComplete, true);
        m_LoadHandle.Completed += op => { OnAssetsLoadDone(); };
    }

    private void OnPerComplete(TextAsset text)
    {
        XSF.Log($"XSFLua.OnPerComplete {text.name} loaded");
        m_LuaContent.Add(text.name, text.bytes);
    }

    private void OnAssetsLoadDone()
    {
        XSF.Log("XSFLua.OnAssetsLoadDone all lua assets load done ....");

        Start();

        if(m_LoadHandle.IsValid())
            Addressables.Release(m_LoadHandle);

        m_LoadHandle = default;
    }
#endregion

#region ProtoAssets
    public void ProtoStart()
    {
        XSF.Log("XSFLua.ProtoStart start ....");
        m_LoadHandle = Addressables.LoadAssetsAsync<TextAsset>("proto", OnPerProtoComplete, true);
        m_LoadHandle.Completed += op => { OnProtoAssetsLoadDone(); };
    }

    private void OnPerProtoComplete(TextAsset text)
    {
        XSF.Log($"XSFLua.OnPerProtoComplete {text.name} loaded");
        m_ProtoLoadedFun(text.name, text.bytes);
    }

    private void OnProtoAssetsLoadDone()
    {
        XSF.Log("XSFLua.OnProtoAssetsLoadDone all proto assets load done ....");

        if(m_LoadHandle.IsValid())
            Addressables.Release(m_LoadHandle);

        m_LoadHandle = default;

        XSFEvent.Instance.Fire(XSF.PROTO_EVENT_ID);

        OnLuaOK();
    }
#endregion

    public void Release()
    {
        m_xlua.Dispose();
    }

    private void Start()
    {
        XSF.Log("XSFLua xlua start");

        m_xlua.DoString("require \"Main\"");

        m_xlua.Global.Get("OnUpdate", out m_LuaUpdate);
        m_xlua.Global.Get("Start", out m_LuaStart);
        m_xlua.Global.Get("OnProtoLoaded", out m_ProtoLoadedFun);

        XSFEvent.Instance.Fire(XSF.LUA_EVENT_ID);
    }

    public void OnLuaOK()
    {
        m_LuaStart();
        XSFUpdate.Instance.Add(this);
    }


    private byte[] XSFLoader(ref string filePath)
    {
#if UNITY_EDITOR
        if (XSFConfig.Instance.LoadLuaInFiles)
        {
            filePath = filePath.Replace(".", "/");
            string path = Application.dataPath + "/../Lua";
            string filename = $"{path}/{filePath}.lua";
            XSF.Log("load lua file. filename=" + filename);

            return File.ReadAllBytes(filename);
        }
        else
        {
#endif  

            filePath = filePath.ToLower();

            XSF.Log("load lua bytes. name=" + filePath);

            byte[] content;
            m_LuaContent.TryGetValue(filePath, out content);
            m_LuaContent.Remove(filePath);

            return content;

#if UNITY_EDITOR
        }
#endif
    }

    #region IUpdateNode相关实现

    public bool IsUpdateWroking
    {
        get { return true; }
    }

    public void OnUpdate()
    {
        m_LuaUpdate();
        m_xlua.Tick();
    }

    public void OnFixedUpdate()
    {
        
    }

    public void OnLateUpdate()
    {

    }

    #endregion
}