//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\Editor\UI\Scripts\UITools.cs
// 作者：Xoen Xie
// 时间：2023/06/20
// 描述：UI工具
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Xml;
using System;

public static class UITools
{
    [MenuItem("XSFTools/创建UI场景 (Create UI Scene)", false, (int)XSFMenuID.CreateUIScene)]
    public static void CreateUIScene()
    {
        EditorWindow window = EditorWindow.GetWindow(typeof(UISceneCreateWindow));
        try
        {
            window.ShowModal();
        }
        catch ( Exception e )
        {
            UnityEngine.Debug.LogError(e.Message);
            window.Close();
        }
    }
}