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
using System.Collections.Generic;

public static class ProtoTools
{
#if UNITY_EDITOR_WIN
    static string PROTOC_DIR = Application.dataPath + "/../../proto/bin/win/";
    static string PROTOC = PROTOC_DIR + "protoc.exe";
#else
    static string PROTOC_DIR = Application.dataPath + "/../../proto/bin/mac/";
    static string PROTOC = PROTOC_DIR + "protoc";
#endif

    [MenuItem("XSFTools/生成pb代码(Gen Proto Code)", false, (int)XSFMenuID.GenProtoCode)]
    public static void GenProtoCode()
    {
        ProtoExport();
        CSCodeGen();
    }

    public static void ProtoExport()
    {
        string outputDir = Application.dataPath + "/Scripts/Net/Proto/";
        string serverOutputDir = Application.dataPath + "/../../server/Message/Proto/";
        bool IsServerDirExist = Directory.Exists(serverOutputDir);

        string [] csProto = new string[] { "Client.proto", "CMessageID.proto", "Common.proto"};

        for(int i = 0; i < csProto.Length; i ++)
        {
            XSFEditorUtil.StartProcess(PROTOC,
                $"--csharp_out={outputDir}  --proto_path=../../ {csProto[i]}", PROTOC_DIR);

            if(IsServerDirExist)
            {
                XSFEditorUtil.StartProcess(PROTOC,
                $"--csharp_out={serverOutputDir}  --proto_path=../../ {csProto[i]}", PROTOC_DIR);
            }
        }

        if(IsServerDirExist)
        {
            string serverProtoDir = Application.dataPath + "/../../proto/server/";
            var protos = Directory.GetFiles(serverProtoDir, "*.proto");
            for(int i = 0; i < protos.Length; i ++)
            {
                FileInfo info = new FileInfo(protos[i]);

                XSFEditorUtil.StartProcess(PROTOC,
                    $"--csharp_out={serverOutputDir}  --proto_path=../../ {info.Name}", PROTOC_DIR);
            }
        }

        Debug.Log("Export to proto done...");
    }

    
    private static void CSCodeGen()
    {
        string CProtoPath = Application.dataPath + "/../../proto/CMessageID.proto";
        string SProtoPath = Application.dataPath + "/../../proto/server/SMessageID.proto";

        string CCodeTemplate = Application.dataPath + "/../../proto/template/MSGTemplate.cs";
        string CTmpContent = File.ReadAllText(CCodeTemplate);

        string SCodeTemplate = Application.dataPath + "/../../proto/template/server/MSGTemplate.cs";
        string STmpContent = File.ReadAllText(SCodeTemplate);

        string COutputDir = Application.dataPath + "/Scripts/Net/Messages";
        string SOutputDir = Application.dataPath + "/../../server/Message/Messages";
        bool IsServerDirExist = Directory.Exists(SOutputDir);

        List<MessageIDData> messageIDs = new List<MessageIDData>();
        messageIDs.AddRange(GetMessageIDs(CProtoPath));
        if(IsServerDirExist)
        {
            messageIDs.AddRange(GetMessageIDs(SProtoPath));
        }

        DateTime currentDateTime = DateTime.Now;
        string DateStr = currentDateTime.ToShortDateString();

        string CMessagePoolStr = "";
        string SMessagePoolStr = "";

        for(int i = 0; i < messageIDs.Count; i ++)
        {
            var md = messageIDs[i];

            if(md.MessageID.Contains("Clt_"))
            {
                string codePath = COutputDir + $"/MSG_{md.MessageID}.cs";
                if(!File.Exists(codePath))
                {
                    string content = CTmpContent.Replace("_MSG_NAME_", md.MessageID);
                    content = content.Replace("_MSG_DATE_", DateStr);
                    content = content.Replace("_MSG_DESC_", Desc.Trim());
                    content = content.Replace("_MSG_ID_NAME_", md.CodeID);
                    File.WriteAllText(codePath, content);
                }

                CMessagePoolStr += $"\t\tm_MessagePool[(int)CMSGID.{md.CodeID}] = new MSG_{md.MessageID}();\n";

                if(IsServerDirExist)
                {
                    string SCodePath = SOutputDir + $"/MSG_{md.MessageID}.cs";
                    if(!File.Exists(SCodePath))
                    {
                        string content = STmpContent.Replace("_MSG_NAME_", md.MessageID);
                        content = content.Replace("_MSG_DATE_", DateStr);
                        content = content.Replace("_MSG_DESC_", Desc.Trim());
                        content = content.Replace("_MSG_ID_NAME_", md.CodeID);
                        File.WriteAllText(SCodePath, content);
                    }

                    CMessagePoolStr += $"\t\t\tm_MessagePool[(int)CMSGID.{md.CodeID}] = new MSG_{md.MessageID}();\n";
                }
            }
            else if(IsServerDirExist)
            {

            }
            

            
            


        }
            
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

    struct MessageIDData
    {
        public string MessageID;
        public string CodeID;

        public string Desc;
    }

    private List<MessageIDData> GetMessageIDs(string protoPath)
    {
        List<MessageIDData> result = new List<MessageIDData>();
        string [] lines = File.ReadAllLines(protoPath);

        bool bGenCode = false;

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

                    MessageIDData md;
                    md.MessageID = msgID;
                    md.CodeID = idName;
                    md.Desc = Desc;
                    result.Add(md);
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

        return result;
    }
}