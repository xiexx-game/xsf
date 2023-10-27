//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\XSF\Edtor\UI\FullBgImageEditor.cs
// 作者：Xoen Xie
// 时间：2023/06/16
// 描述：全屏适配图片
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEditor;
using XSF;

[CustomEditor(typeof(FullBgImage))]
public class FullBgImageEditor : Editor
{
    public override void OnInspectorGUI() 
    {
        DrawDefaultInspector();
 
        FullBgImage image = (FullBgImage)target;
 
        if(GUILayout.Button("Preview")) {
 
            image.UpdateFit();
        }
    }
}