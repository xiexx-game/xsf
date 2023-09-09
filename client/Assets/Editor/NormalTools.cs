//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\Editor\NormalTools.cs
// 作者：Xoen Xie
// 时间：2023/06/21
// 描述：一般工具菜单
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEditor;
using UnityEngine;
using System.IO;

public static class NormalTools
{
    [MenuItem("XSFTools/Clear PlayerRefs", false, (int)XSFMenuID.Localization_Ch)]
    public static void ClearPlayerRefs()
    {
        PlayerPrefs.DeleteAll();
    }
}