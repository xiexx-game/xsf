
using System.IO;
using System.Collections.Generic;
using System;

namespace XSFTools // Note: actual namespace depends on the project name.
{
    public class ProtoTool
    {
        public static void ProtoExport()
        {
            string outputDir = Helper.Instance.ClientDir + "/Assets/Scripts/Net/Proto/";
            string serverOutputDir = Helper.Instance.ServerDir + "/Message/Proto/";
            string cppOutputDir = Helper.Instance.CppServerDir + "/source/message/src/pb";

            bool IsClientDirExist = Directory.Exists(outputDir);
            bool IsServerDirExist = Directory.Exists(serverOutputDir);
            bool IsCppDirExist = Directory.Exists(cppOutputDir);

            Helper.Instance.Logger.Log($"protoc: {Helper.Instance.Protoc}");
            Helper.Instance.Logger.Log($"ProtocDir: {Helper.Instance.ProtocDir}");

            string [] csProto = new string[] { "Client.proto", "CMessageID.proto", "Common.proto"};

            for(int i = 0; i < csProto.Length; i ++)
            {
                if(IsClientDirExist)
                {
                    Helper.Instance.Logger.Log($"protoc export file: {csProto[i]}");
                    Helper.Instance.StartProcess(Helper.Instance.Protoc,
                        $"--csharp_out={outputDir}  --proto_path=../../ {csProto[i]}", Helper.Instance.ProtocDir);
                }

                if(IsServerDirExist)
                {
                    Helper.Instance.Logger.Log($"server protoc export file: {csProto[i]}");
                    Helper.Instance.StartProcess(Helper.Instance.Protoc,
                    $"--csharp_out={serverOutputDir}  --proto_path=../../ {csProto[i]}", Helper.Instance.ProtocDir);
                }

                if(IsCppDirExist)
                {
                    Helper.Instance.Logger.Log($"cpp server protoc export file: {csProto[i]}");
                    Helper.Instance.StartProcess(Helper.Instance.Protoc,
                    $"-I=../../ --cpp_out={cppOutputDir} {csProto[i]}", Helper.Instance.ProtocDir);

                    int nIndex = csProto[i].IndexOf(".proto");
                    string name = csProto[i].Substring(0, nIndex);
                    string SrcH = cppOutputDir + $"/{name}.pb.h";
                    string SrcC = cppOutputDir + $"/{name}.pb.cc";
                    string DescH = Helper.Instance.CppServerDir + "/source/message/inc" + $"/{name}.pb.h";
                    string DescC = cppOutputDir + $"/{name}.pb.cpp";

                    File.Delete(DescH);
                    File.Delete(DescC);
                    File.Move(SrcH, DescH);
                    File.Move(SrcC, DescC);
                }
            }


            string serverProtoDir = Helper.Instance.ProtoDir + "/server/";
            var protos = Directory.GetFiles(serverProtoDir, "*.proto");
            for(int i = 0; i < protos.Length; i ++)
            {
                FileInfo info = new FileInfo(protos[i]);

                if(IsServerDirExist)
                {
                    Helper.Instance.Logger.Log($"2 server protoc export file: {info.Name}");
                    Helper.Instance.StartProcess(Helper.Instance.Protoc,
                        $"--csharp_out={serverOutputDir}  --proto_path=../../server  {info.Name}", Helper.Instance.ProtocDir);
                }

                if(IsCppDirExist)
                {
                    Helper.Instance.Logger.Log($"cpp server protoc export file: {info.Name}");
                    Helper.Instance.StartProcess(Helper.Instance.Protoc,
                        $"-I=../../server --cpp_out={cppOutputDir} {info.Name}", Helper.Instance.ProtocDir);

                    int nIndex = info.Name.IndexOf(".proto");
                    string name = info.Name.Substring(0, nIndex);
                    string SrcH = cppOutputDir + $"/{name}.pb.h";
                    string SrcC = cppOutputDir + $"/{name}.pb.cc";
                    string DescH = Helper.Instance.CppServerDir + "/source/message/inc" + $"/{name}.pb.h";
                    string DescC = cppOutputDir + $"/{name}.pb.cpp";

                    File.Delete(DescH);
                    File.Delete(DescC);
                    File.Move(SrcH, DescH);
                    File.Move(SrcC, DescC);
                }
            }

            Helper.Instance.Logger.Log("Export to proto done...");
        }

        public static void CodeGen()
        {
            string CProtoPath = Helper.Instance.ProtoDir + "/CMessageID.proto";
            string SProtoPath = Helper.Instance.ProtoDir + "/server/SMessageID.proto";

            string CCodeTemplate = Helper.Instance.ProtoDir + "/template/MSGTemplate.cs";
            string CTmpContent = File.ReadAllText(CCodeTemplate);

            string SCodeTemplate = Helper.Instance.ProtoDir + "/template/server/MSGTemplate.cs";
            string STmpContent = File.ReadAllText(SCodeTemplate);

            string COutputDir = Helper.Instance.ClientDir + "/Assets/Scripts/Net/Messages";
            string SOutputDir = Helper.Instance.ServerDir + "/Message/Messages";
            string CppOutputDir = Helper.Instance.CppServerDir + "/source/message";

            bool IsClientDirExist = Directory.Exists(COutputDir);
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
                    if(IsClientDirExist)
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
                    }

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

            if(IsClientDirExist)
            {
                string cPoolFile = Helper.Instance.ClientDir + "/Assets/Scripts/Net/MessagePool.cs";
                Helper.ReplaceContentByTag(cPoolFile, "MESSAGE_START", "MESSAGE_END", CMessagePoolStr);
            }

            if(IsServerDirExist)
            {
                string sPoolFile = Helper.Instance.ServerDir + "/Message/MessageModule.cs";
                Helper.ReplaceContentByTag(sPoolFile, "MESSAGE_START", "MESSAGE_END", SMessagePoolStr);
            }

            if(IsCppDirExist)
            {
                Helper.ReplaceContentByTag(CppHFile, "MESSAGE_BEGIN", "MESSAGE_END", CppHStr);
                Helper.ReplaceContentByTag(CppCFile, "MESSAGE_BEGIN", "MESSAGE_END", CppCStr);
                Helper.ReplaceContentByTag(CppModuleFile, "MESSAGE_BEGIN", "MESSAGE_END", CppModuleStr);
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
                Helper.Instance.Logger.LogError("GetDestEP error, ep not found, message id=" + messageID);
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
                        Helper.Instance.Logger.Log("Find Message ID:" + msgID); 

                        string[] idParts = msgID.Split("_");
                        string idName = "";
                        for(int J = 0; J < idParts.Length; J++)
                        {
                            idName += Helper.FirstLetterToUpper(idParts[J]);
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
}