//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\Editor\Proto\ProtoTools.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：pb tools
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEditor;
using UnityEngine;
using System.IO;
using System;

public static class ProtoTools
{
#if UNITY_EDITOR_WIN
    static string PROTOC_DIR = Application.dataPath + "/../../proto/bin/win/";
    static string PROTOC = PROTOC_DIR + "protoc.exe";
#else
    static string PROTOC_DIR = Application.dataPath + "/../../proto/bin/mac/";
    static string PROTOC = PROTOC_DIR + "protoc";
#endif

    [MenuItem("XSFTools/Proto/Export All", false, (int)XSFMenuID.PBExportAll)]
    public static void PBExportAll()
    {
        PBExportCS();
    }

    [MenuItem("XSFTools/Proto/Export proto to csharp", false, (int)XSFMenuID.PBExportCS)]
    public static void PBExportCS()
    {
        string outputDir = Application.dataPath + "/Scripts/Net/Proto/";

        string [] csProto = new string[] { "Client.proto", "CMessageID.proto", "Common.proto"};

        for(int i = 0; i < csProto.Length; i ++)
        {
            XSFEditorUtil.StartProcess(PROTOC,
                $"--csharp_out={outputDir}  --proto_path=../../ {csProto[i]}", PROTOC_DIR);
        }

        Debug.Log("Export to csharp done...");

        CSCodeGen();

        Debug.Log("csharp code gen done...");
    }

    [MenuItem("XSFTools/Proto/Export proto to cpp", false, (int)XSFMenuID.PBExportCpp)]
    public static void PBExportCpp()
    {
        string outputDir = Application.dataPath + "/Scripts/Net/Proto/";

        XSFEditorUtil.StartProcess(PROTOC,
            $"--cpp_out={outputDir}  --proto_path=../../ ClientCS.proto", PROTOC_DIR);
        XSFEditorUtil.StartProcess(PROTOC,
            $"--cpp_out={outputDir}  --proto_path=../../ CMessageID.proto", PROTOC_DIR);

        Debug.Log("Export to cpp done...");
    }

    private static void CSCodeGen()
    {
        string protoPath = Application.dataPath + "/../../proto/CMessageID.proto";
        string codeTemplate = Application.dataPath + "/Editor/Proto/MSGTemplate.txt";
        string TmpContent = File.ReadAllText(codeTemplate);

        string [] lines = File.ReadAllLines(protoPath);

        bool bGenCode = false;

        string messagePoolStr = "";

        for(int i = 0; i < lines.Length; i ++)
        {
            if(bGenCode)
            {
                if(lines[i].Contains("MESSAGE_ID_END"))
                {
                    break;
                }

                string rawMsgID = lines[i].Trim();

                int index = rawMsgID.IndexOf("=");
                if(index > 0)
                {
                    string msgID = rawMsgID.Substring(0, index-1);
                    msgID = msgID.Trim();
                    Debug.Log("Find Message ID:" + msgID); 

                    string[] idParts = msgID.Split("_");
                    string idName = "";
                    for(int J = 0; J < idParts.Length; J++)
                    {
                        idName += XSFEditorUtil.FirstLetterToUpper(idParts[J].ToLower());
                    }

                    string Desc = "";
                    int descIndex = rawMsgID.IndexOf("//");
                    if(descIndex > 0)
                    {
                        Desc = rawMsgID.Substring(descIndex+2);
                    }

                    string codePath = Application.dataPath + $"/Scripts/Net/Messages/MSG_{msgID}.cs";
                    if(!File.Exists(codePath))
                    {
                        string content = TmpContent.Replace("_MSG_NAME_", msgID);
                        DateTime currentDateTime = DateTime.Now;
                        string date = currentDateTime.ToShortDateString();
                        content = content.Replace("_MSG_DATE_", date);
                        content = content.Replace("_MSG_DESC_", Desc.Trim());

                        
                        content = content.Replace("_MSG_ID_NAME_", idName);
                        File.WriteAllText(codePath, content);
                    }

                    messagePoolStr += $"\t\tm_MessagePool[(int)CMSGID.{idName}] = new MSG_{msgID}();\n";
                }
            }
            else
            {
                if(lines[i].Contains("MESSAGE_ID_START"))
                {
                    bGenCode = true;
                }
            }
        }

        string codeMessagePool = Application.dataPath + "/Scripts/Net/MessagePool.cs";
        XSFEditorUtil.ReplaceContentByTag(codeMessagePool, "MESSAGE_START", "MESSAGE_END", messagePoolStr);
    }
}