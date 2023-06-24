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
    [MenuItem("XSFTools/Export Lua Files", false, (int)XSFMenuID.ExportLuaFiles)]
    public static void ExportLuaFiles()
    {
        string LuaSrcDir = Application.dataPath + "/../Lua";
        LuaSrcDir = LuaSrcDir.Replace("\\", "/");

        string LuaExportDir = Application.dataPath + "/Lua";

        string[] mainDirFiles = Directory.GetFiles(LuaSrcDir, "*.lua");
        for (int i = 0; i < mainDirFiles.Length; ++i)
        {
            string newName = luaFileNameModify(LuaSrcDir, mainDirFiles[i]);
            string desName = $"{LuaExportDir}/{newName}";
            File.Copy(mainDirFiles[i], desName, true);
            Debug.Log($"copy lua file：{desName}");
        }

        string[] dirs = Directory.GetDirectories(LuaSrcDir, "*", SearchOption.AllDirectories);
        for (int i = 0; i < dirs.Length; ++i)
        {
            string dirName = dirs[i].Replace("\\", "/");

            string[] files = Directory.GetFiles(dirName, "*.lua");
            for (int J = 0; J < files.Length; ++J)
            {
                string newName = luaFileNameModify(LuaSrcDir, files[J]);
                string desName = $"{LuaExportDir}/{newName}";
                File.Copy(files[J], desName, true);
                Debug.Log($"copy lua file：{desName}");
            }
        }

        Debug.Log("Export Lua Files done ...");

        AssetDatabase.Refresh();

        AASTools.UpdateAASGroup();
    }

    private static string luaFileNameModify(string LuaSrcDir, string name)
    {
        string newName = name.Replace("\\", "/");
        newName = newName.Replace(LuaSrcDir + "/", "");
        newName = newName.Replace("/", ".");
        return newName.Replace(".lua", ".bytes").ToLower();
    }
}