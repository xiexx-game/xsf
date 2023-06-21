
//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\Editor\UI\Scripts\XSFUIScene.cs
// 作者：Xoen Xie
// 时间：2023/06/20
// 描述：UI场景脚本
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;

using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

using UnityEditor;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;


public class YCompare : IComparer<GameObject>
{
    public int Compare(GameObject a, GameObject b)
    {
        if (a.transform.position.y > b.transform.position.y)
            return 1;
        else
            return 0;
    }
}

[InitializeOnLoad]
public class XSFUIScene : MonoBehaviour
{
    static XSFUIScene()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }


    static bool IsUIScene(Scene? targetScene = null)
    {
        string path = targetScene == null ? EditorSceneManager.GetActiveScene().path : targetScene.Value.path;
        if (string.IsNullOrEmpty(path))
            return false;

        if (!path.Contains("Editor/UI/Scenes"))
        {
            return false;
        }
            

        return true;
    }

    static void OnSceneGUI(SceneView scnView)
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode )
        {
            return;
        }

        if (IsUIScene())
        {
            Handles.BeginGUI();

            GUI.color = Color.green;
            if (GUI.Button(new Rect(10, Screen.height - 80, 80, 30), "Export"))
            {
                string path = ExportUIPrefab();
                if (!string.IsNullOrEmpty(path))
                    Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(path);
            }

            GUI.color = Color.white;
            if (GUI.Button(new Rect(100, Screen.height - 80, 120, 30), "Add Export Node"))
            {
                AddUIExportNode();
                Event.current.Use();
            }

            if (GUI.Button(new Rect(Screen.width - 130, Screen.height - 80, 120, 30), "Open Main Scene"))
            {
                if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    return;

                EditorSceneManager.OpenScene("Assets/Scenes/Main.unity");
            }

            Handles.EndGUI();
        }
    }

    public static string ExportUIPrefab(bool showExist = true)
    {
        GameObject root = GameObject.Find("UIRoot/Root");
        if (!root || !root.transform.Find("UIPanel"))
        {
            Debug.LogError("UI node structure error，" + EditorSceneManager.GetActiveScene().path + "  There is no GameObject named \"UIPanel\"");
            return null;
        }

        // 预制体对象名称
        string UIName = Path.GetFileNameWithoutExtension(EditorSceneManager.GetActiveScene().path);
        string name = UIName + ".prefab";

        string path = $"Assets/UI/Prefabs/{name}";

        string sFullPath = Path.GetFullPath(path);

        if (File.Exists(sFullPath))
        {
            // 如果有重复文件，并且选择不替换原来文件，则应用新的名称
            if (UnityEditor.EditorUtility.DisplayDialog("Notice", "UI Prefab exists, name:" + name + ", need to update？", "Yes", "No"))
            {

            }
            else
            {
                return null;
            }
        }

        //*
        GameObject exportObj = root.transform.Find("UIPanel").gameObject;

        GameObject finnalObj = PrefabUtility.SaveAsPrefabAsset(exportObj, path);

        UIRaycastRect [] rect = finnalObj.GetComponentsInChildren<UIRaycastRect>();
        for (int i = 0; i < rect.Length; ++i)
        {
            DestroyImmediate(rect[i], true);
        }

        AssetDatabase.Refresh();
        //*/

        return path;
    }

    private static void AddUIExportNode()
    {
        foreach (var VARIABLE in Selection.gameObjects)
        {
            AddUIExportNode(VARIABLE);
        }
    }

    private static void AddUIExportNode(GameObject obj)
    {
        if (obj == null) return;

        int length = obj.GetComponents<Component>().Length;

        UIExportNode node = obj.AddComponent<UIExportNode>();

        node.Name = obj.name;
        node.Comp = typeof(GameObject).Name;
    }
}
