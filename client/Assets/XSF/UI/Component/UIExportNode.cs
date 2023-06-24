//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\XSF\UI\Component\UIExportNode.cs
// 作者：Xoen Xie
// 时间：2023/06/21
// 描述：UI导出结点
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using System;
using UnityEngine;

#if UNITY_EDITOR
public class UIExportNode : MonoBehaviour
{
    [Header("Node Name")]
    public string Name;
    public string Namespace;
    public string Comp;

    [Header("Need Click")]
    public bool NeedClick;

    [Header("Describe")]
    public string Describe;
}
#endif