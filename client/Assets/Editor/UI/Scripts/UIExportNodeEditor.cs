using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIExportNode))]
public class UIExportNodeEditor : Editor
{
    private UIExportNode _target;

    public void OnEnable()
    {
        _target = target as UIExportNode;
    }

    public override void OnInspectorGUI()
    {
        string newName = EditorGUILayout.TextField("Node Name", _target.Name);
        if (newName != _target.Name)
        {
            _target.Name = newName;
            EditorUtility.SetDirty(_target);
        }

        Component[] comps = _target.GetComponents<Component>();
    
        int index = -1;
        List<string> strs = new List<string>();
        strs.Add("GameObject");
        for (int i = 0; i < comps.Length; i++)
        {
            System.Type type = comps[i].GetType();

            if (type == _target.GetType()) continue;

            strs.Add(type.Name);

            if (_target.Comp == type.Name)
                index = strs.Count - 1;
        }

        if (index == -1)
        {
            index = 0;
            _target.Comp = strs[0];
            EditorUtility.SetDirty(_target);
        }

        int newIndex = EditorGUILayout.Popup("Export Class", index, strs.ToArray());

        if (newIndex != index)
        {
            _target.Comp = strs[newIndex];
            EditorUtility.SetDirty(_target);
        }

        bool b = EditorGUILayout.Toggle("Need Click Event", _target.NeedClick);
        if (b != _target.NeedClick)
        {
            _target.NeedClick = b;
            EditorUtility.SetDirty(_target);
        }

        string desc = EditorGUILayout.TextField("Code Describe", _target.Describe);
        if (desc != _target.Describe)
        {
            _target.Describe = desc;
            EditorUtility.SetDirty(_target);
        }         
    }
}
