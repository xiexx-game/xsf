#pragma warning disable CS8600, CS8602, CS8618, CS8604, CS8603

using System.IO;
using System.Diagnostics;
using System.Xml;
using System.Collections.Generic;
using System;

namespace XSFTools // Note: actual namespace depends on the project name.
{
    public enum Platform
    {
        Windows = 0,
        Mac,
        Linux,
    }

    public interface IToolLogger
    {
        void Log(string message);
        void LogError(string message);
    }

    public class ConfigAttribute
    {
        public string Name;
        public string Suffix;
        public string Prefix;
        public string ValueStr;
        public string CsTypeName;
        public string CppTypeName;
    }

    public class Helper
    {
        private static Helper m_Instance;
        public static Helper Instance 
        {
            get
            {
                if(m_Instance == null)
                {
                    m_Instance = new Helper();
                }

                return m_Instance;
            }
        }

        private IToolLogger m_Logger;

        public IToolLogger Logger { get { return m_Logger; } }

        private Dictionary<string, ConfigAttribute> m_Attributes;

        public string ClientDir { get; private set; }
        public string ServerDir { get; private set; }
        public string CppServerDir { get; private set; }
        public Platform CurrentPlatform { get; private set; }
        public string ProtoDir { get; private set; }
        public string ConfigDir { get; private set; }

        public string ProtocDir { get; private set; }
        public string Protoc { get; private set; }

        public void Init(Platform p, IToolLogger logger, string sRootPath, bool IsConfigTool)
        {
            CurrentPlatform = p;
            m_Logger = logger;
            ClientDir = sRootPath + "/client";
            ServerDir = sRootPath + "/server";
            CppServerDir = sRootPath + "/server-cpp";

            ProtoDir = sRootPath + "/proto";
            ConfigDir = sRootPath + "/config";

            switch (p)
            {
                case Platform.Mac:
                    ProtocDir = ProtoDir + "/bin/mac/";
                    Protoc = ProtocDir + "protoc";
                    break;

                case Platform.Linux:
                    ProtocDir = ProtoDir + "/bin/linux/";
                    Protoc = ProtocDir + "protoc";
                    break;

                default:
                    ProtocDir = ProtoDir + "/bin/win/";
                    Protoc = ProtocDir + "protoc.exe";
                    break;
            }

            if(IsConfigTool)
            {   
                m_Attributes = new Dictionary<string, ConfigAttribute>();

                string sContent = File.ReadAllText(ConfigDir + "/CodeConfig.xml");

                XMLReader reader = new XMLReader();
                reader.Read(sContent);
                
                XmlNodeList nodeList = reader.mRootNode.ChildNodes;

                for(int i = 0; i < nodeList.Count; i ++)
                {
                    XmlElement ele = nodeList[i] as XmlElement;
                    ConfigAttribute ca = new ConfigAttribute();
                    ca.Name = XMLReader.GetString(ele, "name");
                    ca.Suffix = XMLReader.GetString(ele, "suffix");
                    ca.Prefix = XMLReader.GetString(ele, "prefix");
                    ca.ValueStr = XMLReader.GetString(ele, "value_str");
                    ca.CsTypeName = XMLReader.GetString(ele, "cs_type_name");
                    ca.CppTypeName = XMLReader.GetString(ele, "cpp_type_name");
                    m_Attributes.Add(ca.Name, ca);
                }
            }
        }

        public ConfigAttribute GetConfigAttribute(string name)
        {
            ConfigAttribute ca = null;
            if(m_Attributes.TryGetValue(name, out ca))
            {
                return ca;
            }

            return null;
        }

        public bool StartProcess(string sFilename, string sArgs, string sWorkDir)
        {
            Process ps = null;
            bool fail = false;

            try
            {
                ProcessStartInfo StartInfo = new ProcessStartInfo();
                StartInfo.FileName = sFilename;
                StartInfo.Arguments = sArgs;
                StartInfo.CreateNoWindow = true;
                StartInfo.UseShellExecute = false;
                if (!string.IsNullOrEmpty(sWorkDir))
                    StartInfo.WorkingDirectory = sWorkDir;
                StartInfo.RedirectStandardError = true;
                ps = new Process();
                ps.StartInfo = StartInfo;
                ps.Start();
                StreamReader readerErr = ps.StandardError; //截取错误流

                string line = readerErr.ReadLine();
                while (!readerErr.EndOfStream)
                {
                    line = readerErr.ReadLine();
                    line = line + "\r\n";
                }

                ps.WaitForExit();


                string fileName = null;
                string arguments = null;

                if (ps.ExitCode != 0 && !fail)
                {
                    fail = true;
                    fileName = ps.StartInfo.FileName;
                    arguments = ps.StartInfo.Arguments;
                }

                if (fail)
                {
                    throw new Exception(string.Format("ExitCode:{0}]\nStartProcess Fail.FileName=[{1}]\nArg=[{2}\n{3}",
                        ps.ExitCode, fileName, arguments, line));
                }
            }
            catch (Exception e)
            {
                m_Logger.LogError("StartProcess Error :" + e.Message);
            }
            finally
            {
                ps.Dispose();
            }

            return !fail;
        }

        enum FindOp
        {
            None = 0,
            Start,
            End,
        }

        public static bool ReplaceContentByTag(string sFilename, string sTagStart, string sTagEnd, string sNewContent)
        {
            bool bFindStart = false;
            bool bFindEnd = false;

            string[] lines = File.ReadAllLines(sFilename);

            string sSrcContent = "";

            FindOp nFindFlag = FindOp.Start;

            for (uint i = 0; i < lines.Length; ++i)
            {
                switch (nFindFlag)
                {
                    case FindOp.Start:
                        {
                            sSrcContent += lines[i] + "\n";

                            if (lines[i].Contains(sTagStart))
                            {
                                nFindFlag = FindOp.End;

                                sSrcContent += sNewContent;

                                bFindStart = true;
                            }
                        }
                        break;

                    case FindOp.End:
                        {
                            if (lines[i].Contains(sTagEnd))
                            {
                                nFindFlag = FindOp.None;

                                sSrcContent += lines[i] + "\n";
                                bFindEnd = true;
                            }
                        }
                        break;

                    default:
                        sSrcContent += lines[i] + "\n";
                        break;
                }
            }

            if (bFindStart && bFindEnd)
            {
                File.WriteAllText(sFilename, sSrcContent);
                return true;
            }

            return false;
        }

        public static bool AppendFileByTag(string sFilename, string sTag, string sNewContent)
        {
            if(string.IsNullOrEmpty(sNewContent)) {
                return false;
            }

            bool bFindTag = false;

            string[] lines = File.ReadAllLines(sFilename);

            string sSrcContent = "";

            FindOp nFindFlag = FindOp.Start;

            for (uint i = 0; i < lines.Length; ++i)
            {
                switch (nFindFlag)
                {
                    case FindOp.Start:
                        {
                            if (lines[i].Contains(sTag))
                            {
                                nFindFlag = FindOp.None;

                                sSrcContent += "\n";
                                sSrcContent += sNewContent;
                                
                                bFindTag = true;
                            }

                            sSrcContent += lines[i] + "\n";
                        }
                        break;

                    default:
                        sSrcContent += lines[i] + "\n";
                        break;
                }
            }

            if (bFindTag)
            {
                File.WriteAllText(sFilename, sSrcContent);
                return true;
            }

            return false;
        }

        public static string FirstLetterToUpper(string str) 
        {
            if(str.Length > 0)
                return char.ToUpper(str[0]) + str.Substring(1);
            
            return str.ToUpper();
        }
    }
}