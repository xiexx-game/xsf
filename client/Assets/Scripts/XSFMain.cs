//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\XSF\XSFMain.cs
// 作者：Xoen Xie
// 时间：2023/06/15
// 描述：主入口脚本
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using System;
using UnityEngine;
using XSF;
using XsfNet;
using XsfUI;

public sealed class XSFMain : MonoSingleton<XSFMain>
{
    [Header("主相机")] public Camera MainCamera;
    [Header("UI相机")] public Camera UICamera;

    [Header("UI根物件")] public GameObject UIRoot;
    [Header("启动UI")] public GameObject UIStartup;

    enum MainStatus
    {
        None = 0,
        Init,
        Run,
        Error,
    }

    private MainStatus m_nStatus;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(MainCamera.gameObject);
        DontDestroyOnLoad(UIRoot);
        m_nStatus = MainStatus.Init;
    }

    private void Update()
    {
        try
        {
            switch (m_nStatus)
            {
                case MainStatus.Init:
                    {
                        XSFCore.Init();
                        XSFStartup.Instance.Init();
                        XSFUI.Instance.Init(new UIHelper(), XSFMain.Instance.UIRoot);
                        XSFNet.Instance.Init();
                        m_nStatus = MainStatus.Run;
                    }
                    break;

                case MainStatus.Run:
                    {
                        XSFCore.Update();
                    }
                    break;
            }
        }
        catch (XSFSchemaLoadException e)
        {
            Debug.LogError($"Main.Update catch schema exception, message={e.Message}, stack=\n{e.StackTrace}");
            m_nStatus = MainStatus.Error;
        }
        catch (Exception e)
        {
            Debug.LogError($"Main.Update catch exception, message={e.Message}, stack=\n{e.StackTrace}");

#if EXCEPTION_PAUSE
            m_nStatus = MainStatus.Error;
#endif
        }
    }

    private void FixedUpdate()
    {
        try
        {
            switch (m_nStatus)
            {
                case MainStatus.Run:
                    {
                        XSFCore.FixedUpdate();
                    }
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Main.FixedUpdate catch exception, message={e.Message}, stack=\n{e.StackTrace}");

#if EXCEPTION_PAUSE
            m_nStatus = MainStatus.Error;
#endif
        }
    }

    private void OnDestroy()
    {
        Debug.Log("Main Destroy");
    }

    private void OnApplicationQuit()
    {
        XSFNet.Instance.Release();
        XSFCore.Release();

        Debug.Log("App Quit");
    }
}