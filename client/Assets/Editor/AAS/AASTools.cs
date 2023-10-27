//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\Editor\AAS\AASTools.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：AAS工具
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using System.Xml;
using System.IO;
using System.Linq;
using XSF;

public static class AASTools
{
    private struct AASConfig
    {
        public string group;
        public string folder;
        public string extension;
        public string label;
    }

    [MenuItem("XSFTools/刷新AAS组 (Refresh AAS Group)", false, (int)XSFMenuID.UpdateAASGroup)]
    public static void UpdateAASGroup()
    {
        AssetDatabase.Refresh();

        var m_Settings = AddressableAssetSettingsDefaultObject.Settings;
        //加载XML配置
        var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Editor/AAS/GroupConfig.xml");

        XMLReader reader = new XMLReader();
        reader.Read(textAsset.text);

        var kv = new Dictionary<string, List<AASConfig>>();

        XmlNodeList nodeList = reader.mRootNode.ChildNodes;
        for (int i = 0; i < nodeList.Count; ++i)
        {
            var element = (XmlElement)nodeList[i];

            string group = XMLReader.GetString(element, "group");

            List<AASConfig> list = null;
            if (!kv.TryGetValue(group, out list))
            {
                list = new List<AASConfig>();
                kv.Add(group, list);
            }

            AASConfig data = new AASConfig
            {
                extension = XMLReader.GetString(element, "extension"),
                label = XMLReader.GetString(element, "label"),
                folder = XMLReader.GetString(element, "folder"),
                group = group
            };
            list.Add(data);
        }

        foreach (var item in kv)
        {
            //将每一条配置进行分组处理
            AddDirAssetsToGroup(m_Settings, item.Key, item.Value.ToArray());
        }

        UnityEngine.Debug.Log("AAS groups fresh done ...");
    }

    private static void AddDirAssetsToGroup(AddressableAssetSettings settings, string groupName, AASConfig[] configs)
    {
        var group = settings.FindGroup(groupName);
        if (group == null)
        {
            //可以代码创建分组，根据需求来
            Debug.LogError($"刷新AAS资源组 group[{groupName}] is not exist, please create first");
        }
        else
        {
            var fileHash = new HashSet<string>();

            foreach (var data in configs)
            {
                //获取到对应目录
                var path = $"{Application.dataPath}/{data.folder}";
                //处理extension字段，拆分为多个后缀名
                var extensionNames = data.extension.Split(",");
                var directoryInfo = new DirectoryInfo(path);
                var fileInfos = new List<FileInfo>();

                foreach (var ext in extensionNames)
                {
                    fileInfos.AddRange(directoryInfo.GetFiles(ext, SearchOption.TopDirectoryOnly));
                }

                foreach (var file in fileInfos)
                {
                    var fullPath = file.FullName.Replace("\\", "/");
                    var relativePath = fullPath.Replace(Application.dataPath, "Assets");

                    //资源的guid
                    var fileGuid = AssetDatabase.AssetPathToGUID(relativePath);
                    if (string.IsNullOrEmpty(fileGuid))
                    {
                        Debug.LogError($"刷新AAS资源组 resource[{relativePath}], empty guid, please try it again");
                        continue;
                    }

                    //添加到判重列表里面
                    fileHash.Add(fileGuid);
                    var entry = settings.CreateOrMoveEntry(fileGuid, group);

                    var labels = new string[entry.labels.Count];
                    entry.labels.CopyTo(labels);

                    foreach (var t in labels)
                        entry.SetLabel(t, false);

                    var labelNames = data.label.Split(",");
                    foreach (var t in labelNames)
                        entry.SetLabel(t, true, true);

                    //设置名字
                    var aasName = Path.GetFileNameWithoutExtension(fullPath);

                    entry.SetAddress(aasName);
                }
            }

            //循环已经有的entry，移除配置没有定义的
            var results = new List<AddressableAssetEntry>();
            group.GatherAllAssets(results, true, true, true);
            foreach (var t in results.Where(t => !fileHash.Contains(t.guid)))
            {
                settings.RemoveAssetEntry(t.guid);
            }
        }
    }
}