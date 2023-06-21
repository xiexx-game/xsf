//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\Editor\UI\Scripts\UISceneCreateWindow.cs
// 作者：Xoen Xie
// 时间：2023/06/20
// 描述：UI工具
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using System;
using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEditor.SceneManagement;

public class UISceneCreateWindow : EditorWindow
{
    string m_sUIName = "Test";
    public UISceneCreateWindow()
    {
        this.titleContent = new GUIContent("Create UI Scene");
        this.maximized = false;
        this.minSize = this.maxSize = new Vector2(200, 80);
    }

    void OnGUI()
    {
        GUILayout.Label("Enter UI name :");
        GUILayout.Space(10f);
        GUILayout.BeginHorizontal();
        m_sUIName = GUILayout.TextField(m_sUIName, new GUIStyle(GUI.skin.textField)
        {
            
        });

        GUILayout.EndHorizontal();
        GUILayout.Space(10f);
        if (GUILayout.Button("Create"))
        {
            string sceneName = m_sUIName.StartsWith("UI") ? m_sUIName + ".unity" : "UI" + m_sUIName + ".unity";
            string sceneFullPath = $"{Application.dataPath}/Editor/UI/Scenes/{sceneName}";
            string sceneAssetPath = $"Assets/Editor/UI/Scenes/{sceneName}";
            if (File.Exists(sceneFullPath))
            {
                Close();
                EditorSceneManager.OpenScene(sceneAssetPath);

                EditorUtility.DisplayDialog("Notice", $"Scene {sceneName} Already exists. The UI scene is opened", "Ok");
            }
            else
            {
                if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    return;


                if (!AssetDatabase.CopyAsset("Assets/Editor/UI/UISample.unity", sceneAssetPath))
                {
                    Debug.LogError("AssetDatabase.CopyAsset error ...");
                }

                var s = EditorSceneManager.OpenScene(sceneAssetPath);
                
                Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(sceneAssetPath);

                Close();
            }
        }
    }
}
