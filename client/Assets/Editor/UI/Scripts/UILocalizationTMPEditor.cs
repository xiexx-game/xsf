
using UnityEditor;
using UnityEngine;
using XSF;

[CustomEditor(typeof(UILocalizationTMP))]
public class UILocalizationTMPEditor : Editor
{
    private UILocalizationTMP _target;

    private void OnEnable()
    {
        _target = target as UILocalizationTMP;
    }

    public override void OnInspectorGUI()
    {

        DrawDefaultInspector();

        if (GUILayout.Button("Update Local Text"))
        {
            _target.UpdateText();
        }
    }
}