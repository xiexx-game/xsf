//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Editor/SchemaMenus.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：配置工具
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Xml;
using System;
using XSF;
using XsfScp;

public static class SchemaMenus
{
    [MenuItem("XSFTools/导出配置 (Export Schema)", false, (int)XSFMenuID.ExportSchema)]
    public static void ExportSchema()
    {   
#if UNITY_EDITOR_WIN
        XSFTools.Platform p = XSFTools.Platform.Windows;
#elif UNITY_EDITOR_OSX
        XSFTools.Platform p = XSFTools.Platform.Mac;
#endif

        string rootPath = XSFEditorUtil.GetRootPath();
        Debug.Log(rootPath);

        XSFTools.Helper.Instance.Init(p, new ToolLogger(), rootPath, true);
        XSFTools.SchemaTools.DoExport();

        Debug.Log("schema export done ...");
    }

    
    [MenuItem("XSFTools/设置本地化 (Set Localization)/清除 (Clear)", false, (int)XSFMenuID.Localization_Clear)]
    public static void Localization_Clear()
    {
        PlayerPrefs.SetString(XSFLocalization.Instance.LOCAL_PREFS, "");
        Debug.Log("Clear Localization");
        XSFLocalization.Instance.Init();
    }

    [MenuItem("XSFTools/设置本地化 (Set Localization)/英文 (English)", false, (int)XSFMenuID.Localization_En)]
    public static void Localization_En()
    {
        PlayerPrefs.SetString(XSFLocalization.Instance.LOCAL_PREFS, "English");
        Debug.Log("Set Localization: English");
        XSFLocalization.Instance.Init();
    }

    [MenuItem("XSFTools/设置本地化 (Set Localization)/中文简体 (Chinese)", false, (int)XSFMenuID.Localization_Ch)]
    public static void Localization_Ch()
    {
        PlayerPrefs.SetString(XSFLocalization.Instance.LOCAL_PREFS, "Chinese");
        Debug.Log("Set Localization: Chinese");
        XSFLocalization.Instance.Init();
    }
}