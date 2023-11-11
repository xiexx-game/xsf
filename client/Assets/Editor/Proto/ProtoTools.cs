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

    [MenuItem("XSFTools/生成proto代码(Gen Proto Code)", false, (int)XSFMenuID.GenProtoCode)]
    public static void GenProtoCode()
    {
        ProtoExport();
        CodeGen();
    }

    public static void ProtoExport()
    {
        string outputDir = Application.dataPath + "/Scripts/Net/Proto/";
        string serverOutputDir = Application.dataPath + "/../../server/Message/Proto/";
        string cppOutputDir = Application.dataPath + "/../../server-cpp/source/message/src/pb";
        bool IsServerDirExist = Directory.Exists(serverOutputDir);
        bool IsCppDirExist = Directory.Exists(cppOutputDir);

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

            if(IsCppDirExist)
            {
                XSFEditorUtil.StartProcess(PROTOC,
                $"-I=../../ --cpp_out={cppOutputDir} {csProto[i]}", PROTOC_DIR);

                int nIndex = csProto[i].IndexOf(".proto");
                string name = csProto[i].Substring(0, nIndex);
                string SrcH = cppOutputDir + $"/{name}.pb.h";
                string SrcC = cppOutputDir + $"/{name}.pb.cc";
                string DescH = Application.dataPath + "/../../server-cpp/source/message/inc" + $"/{name}.pb.h";
                string DescC = cppOutputDir + $"/{name}.pb.cpp";

                File.Delete(DescH);
                File.Delete(DescC);
                File.Move(SrcH, DescH);
                File.Move(SrcC, DescC);
            }
        }


        string serverProtoDir = Application.dataPath + "/../../proto/server/";
        var protos = Directory.GetFiles(serverProtoDir, "*.proto");
        for(int i = 0; i < protos.Length; i ++)
        {
            FileInfo info = new FileInfo(protos[i]);

            if(IsServerDirExist)
            {
                XSFEditorUtil.StartProcess(PROTOC,
                    $"--csharp_out={serverOutputDir}  --proto_path=../../server  {info.Name}", PROTOC_DIR);
            }

            if(IsCppDirExist)
            {
                XSFEditorUtil.StartProcess(PROTOC,
                    $"-I=../../server --cpp_out={cppOutputDir} {info.Name}", PROTOC_DIR);

                int nIndex = info.Name.IndexOf(".proto");
                string name = info.Name.Substring(0, nIndex);
                string SrcH = cppOutputDir + $"/{name}.pb.h";
                string SrcC = cppOutputDir + $"/{name}.pb.cc";
                string DescH = Application.dataPath + "/../../server-cpp/source/message/inc" + $"/{name}.pb.h";
                string DescC = cppOutputDir + $"/{name}.pb.cpp";

                File.Delete(DescH);
                File.Delete(DescC);
                File.Move(SrcH, DescH);
                File.Move(SrcC, DescC);
            }
        }

        Debug.Log("Export to proto done...");
    }

    
    private static void CodeGen()
    {
        string CProtoPath = Application.dataPath + "/../../proto/CMessageID.proto";
        string SProtoPath = Application.dataPath + "/../../proto/server/SMessageID.proto";

        string CCodeTemplate = Application.dataPath + "/../../proto/template/MSGTemplate.cs";
        string CTmpContent = File.ReadAllText(CCodeTemplate);

        string SCodeTemplate = Application.dataPath + "/../../proto/template/server/MSGTemplate.cs";
        string STmpContent = File.ReadAllText(SCodeTemplate);

        string COutputDir = Application.dataPath + "/Scripts/Net/Messages";
        string SOutputDir = Application.dataPath + "/../../server/Message/Messages";
        string CppOutputDir = Application.dataPath + "/../../server-cpp/source/message";
        bool IsServerDirExist = Directory.Exists(SOutputDir);
        bool IsCppDirExist = Directory.Exists(CppOutputDir);

        string CppHFile = CppOutputDir + "/inc/Messages.h";
        string CppCFile = CppOutputDir + "/src/Messages.cpp";
        string CppModuleFile = CppOutputDir + "/src/MessageModule.cpp";

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

        string CppHStr = "";
        string CppCStr = "";
        string CppModuleStr = "";

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
                    content = content.Replace("_MSG_DESC_", md.Desc.Trim());
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
                        content = content.Replace("_MSG_DESC_", md.Desc.Trim());
                        content = content.Replace("_MSG_ID_NAME_", md.CodeID);
                        content = content.Replace("_ID_PREFIX_", "CMSGID");
                        content = content.Replace("_EP_NAME_", GetDestEP(md.MessageID));
                        File.WriteAllText(SCodePath, content);
                    }

                    SMessagePoolStr += $"\t\t\tm_MessagePool[(int)CMSGID.{md.CodeID}] = new MSG_{md.MessageID}();\n";
                }

                if(IsCppDirExist)
                {
                    CppHStr += $"\tMESSAGE({md.MessageID}, EP_{GetDestEP(md.MessageID)}, xsf_pbid::CMSGID::{md.MessageID})\n";
                    CppCStr += $"\tMESSAGE_FUNCTIONS({md.MessageID})\n";
                    CppModuleStr += $"\t\tNEW_MESSAGE({md.MessageID}, xsf_pbid::CMSGID::{md.MessageID})\n";
                }
            }
            else 
            {
                if(IsServerDirExist)
                {
                    string SCodePath = SOutputDir + $"/MSG_{md.MessageID}.cs";
                    if(!File.Exists(SCodePath))
                    {
                        string content = STmpContent.Replace("_MSG_NAME_", md.MessageID);
                        content = content.Replace("_MSG_DATE_", DateStr);
                        content = content.Replace("_MSG_DESC_", md.Desc.Trim());
                        content = content.Replace("_MSG_ID_NAME_", md.CodeID);
                        content = content.Replace("_ID_PREFIX_", "SMSGID");
                        content = content.Replace("_EP_NAME_", GetDestEP(md.MessageID));
                        File.WriteAllText(SCodePath, content);
                    }

                    SMessagePoolStr += $"\t\t\tm_MessagePool[(int)SMSGID.{md.CodeID}] = new MSG_{md.MessageID}();\n";
                }

                if(IsCppDirExist)
                {
                    CppHStr += $"\tMESSAGE({md.MessageID}, EP_{GetDestEP(md.MessageID)}, xsf_pbid::SMSGID::{md.MessageID})\n";
                    CppCStr += $"\tMESSAGE_FUNCTIONS({md.MessageID})\n";
                    CppModuleStr += $"\t\tNEW_MESSAGE({md.MessageID}, xsf_pbid::SMSGID::{md.MessageID})\n";
                }
            }
        }

        string cPoolFile = Application.dataPath + "/Scripts/Net/MessagePool.cs";
        XSFEditorUtil.ReplaceContentByTag(cPoolFile, "MESSAGE_START", "MESSAGE_END", CMessagePoolStr);

        if(IsServerDirExist)
        {
            string sPoolFile = Application.dataPath + "/../../server/Message/MessageModule.cs";
            XSFEditorUtil.ReplaceContentByTag(sPoolFile, "MESSAGE_START", "MESSAGE_END", SMessagePoolStr);
        }

        if(IsCppDirExist)
        {
            XSFEditorUtil.ReplaceContentByTag(CppHFile, "MESSAGE_BEGIN", "MESSAGE_END", CppHStr);
            XSFEditorUtil.ReplaceContentByTag(CppCFile, "MESSAGE_BEGIN", "MESSAGE_END", CppCStr);
            XSFEditorUtil.ReplaceContentByTag(CppModuleFile, "MESSAGE_BEGIN", "MESSAGE_END", CppModuleStr);
        }
    }

    private static string GetDestEP(string messageID)
    {
        if(messageID.Contains("G_"))
        {
            return "Game";
        }
        else if(messageID.Contains("Gt_"))
        {
            return "Gate";
        }
        else if(messageID.Contains("C_"))
        {
            return "Center";
        }
        else
        {
            Debug.LogError("GetDestEP error, ep not found, message id=" + messageID);
            return "None";
        }
    }

    struct MessageIDData
    {
        public string MessageID;
        public string CodeID;

        public string Desc;
    }

    private static List<MessageIDData> GetMessageIDs(string protoPath)
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
                        idName += XSFEditorUtil.FirstLetterToUpper(idParts[J]);
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