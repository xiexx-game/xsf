//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\XSF\Edtor\XSFEditorUtil.cs
// 作者：Xoen Xie
// 时间：2023/06/16
// 描述：工具通用函数集
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using System;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Diagnostics;

public static class XSFEditorUtil
{
    public static bool StartProcess(string sFilename, string sArgs, string sWorkDir)
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
            UnityEngine.Debug.LogError(e.Message);
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