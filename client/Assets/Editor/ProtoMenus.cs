//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Editor/ProtoMenus.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：pb tools
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEditor;
using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;

public static class ProtoMenus
{


    [MenuItem("XSFTools/生成proto代码(Gen Proto Code)", false, (int)XSFMenuID.GenProtoCode)]
    public static void GenProtoCode()
    {
#if UNITY_EDITOR_WIN
        XSFTools.Platform p = XSFTools.Platform.Windows;
#elif UNITY_EDITOR_OSX
        XSFTools.Platform p = XSFTools.Platform.Mac;
#endif

        string rootPath = XSFEditorUtil.GetRootPath();
        Debug.Log(rootPath);

        XSFTools.Helper.Instance.Init(p, new ToolLogger(), rootPath, false);
        XSFTools.ProtoTool.ProtoExport();
        XSFTools.ProtoTool.CodeGen();

        Debug.Log("proto gen done ...");
    }
}