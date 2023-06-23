using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIExportNode))]
public class UIExportNodeEditor : Editor
{
    private UIExportNode _target;

    private int _selectedIndex;

    List<string> _strs;
    List<string> _nsStrs;

    public void OnEnable()
    {
        _target = target as UIExportNode;
        _strs = new List<string>();
        _nsStrs = new List<string>();

        _strs.Add("GameObject");
        _nsStrs.Add("UnityEngine");

        _selectedIndex = -1;

        Component[] comps = _target.GetComponents<Component>();
        for (int i = 0; i < comps.Length; i++)
        {
            System.Type type = comps[i].GetType();

            _strs.Add(type.Name);
            _nsStrs.Add(type.Namespace);
        }

        for(int i = 0; i < _strs.Count; i ++)
        {
            if(_strs[i] == _target.Comp)
            {   
                _selectedIndex = i;
            }
        }

        if(_selectedIndex == -1)
        {
            _selectedIndex = 0;
            _target.Comp = _strs[0];
            _target.Namespace = _nsStrs[0];
            EditorUtility.SetDirty(_target);
        }
    }

    public override void OnInspectorGUI()
    {
        string newName = EditorGUILayout.TextField("Node Name", _target.Name);
        if (newName != _target.Name)
        {
            _target.Name = newName;
            EditorUtility.SetDirty(_target);
        }

        int nOldIndex = _selectedIndex;
        _selectedIndex = EditorGUILayout.Popup("Export Class", _selectedIndex, _strs.ToArray());

        if(nOldIndex != _selectedIndex) 
        {
            _target.Comp = _strs[_selectedIndex];
            _target.Namespace = _nsStrs[_selectedIndex];
            EditorUtility.SetDirty(_target);

            if(_target.Comp != "GameObject")
            {
                _target.NeedClick = false;
                EditorUtility.SetDirty(_target);
            }
        }

        bool b = EditorGUILayout.Toggle("Need Click Event", _target.NeedClick);
        if (b != _target.NeedClick)
        {
            if(b)
            {
                if(_target.Comp != "GameObject") {
                    b = false;
                    Debug.LogWarning("Click Event need GameObject, please set [Export Class] as GameObject first.");
                }
            }

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

