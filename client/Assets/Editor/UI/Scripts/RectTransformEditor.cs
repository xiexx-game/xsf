//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\XSF\Edtor\UI\RectTransformEditor.cs
// 作者：Xoen Xie
// 时间：2023/06/16
// 描述：UI工具
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEditor;
 
using System.Linq;
using System.Reflection;
 

[CustomEditor(typeof(RectTransform)), CanEditMultipleObjects]
public sealed class RectTransformInspector : Editor
{
    private Editor instance;

    private void OnEnable()
    {
        var editorType = Assembly.GetAssembly(typeof(Editor)).GetTypes().FirstOrDefault(m => m.Name == "RectTransformEditor");
        instance = CreateEditor(targets, editorType);
    }

    public override void OnInspectorGUI()
    {
        if (instance)
        {
            instance.OnInspectorGUI();
        }
        GUILayout.Space(20f);

        if(GUILayout.Button("Reset"))
        {
            for (int i = 0; i < targets.Length; i++)
            {
                RectTransform tempTarget = targets[i] as RectTransform;
                Undo.RecordObject(tempTarget, "Reset");

                tempTarget.anchorMin = new Vector2(0.5f, 0.5f);
                tempTarget.anchorMax = new Vector2(0.5f, 0.5f);
                tempTarget.anchoredPosition = new Vector2(0f, 0f);
            }
        }
    }

    private void OnDisable()
    {
        if(instance)
        {
            DestroyImmediate(instance);
        }
    }
}