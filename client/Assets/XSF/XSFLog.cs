//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\XSF\XSFLog.cs
// 作者：Xoen Xie
// 时间：2023/06/15
// 描述：一个简单的日志
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using System;
using UnityEngine;
using System.IO;
using System.Threading;
using System.Globalization;

public sealed class XSFLog : Singleton<XSFLog>
{
    struct LogData
    {
        public LogType type;         // 日志类型
        public string message;          // 数据
        public string date;             // 时间
    }

    private string m_sLogFilePath;
    private bool m_bRun;
    private object m_Lock;
    private StreamWriter m_Writer;
    private Thread m_Thread;
    private XSFQueue<LogData> m_LogQueue;
    private ManualResetEvent m_ResetEvent;

    public ulong InfoCount {get; private set;}
    public ulong ErrorCount {get; private set;}
    public ulong WarnCount {get; private set;}

    public XSFLog()
    {
        m_Lock = new object();
    }

    public void Init()
    {
        m_LogQueue = new XSFQueue<LogData>();
        m_ResetEvent = new ManualResetEvent(true);

#if UNITY_EDITOR
        string sLogDir = Application.dataPath + "/../Log/";
        if (!Directory.Exists(sLogDir))
        {
            Directory.CreateDirectory(sLogDir);
        }

        DateTime now = DateTime.Now;
        m_sLogFilePath = string.Format("{0}/{1:D2}{2:D2}_{3:D2}{4:D2}{5:D2}.log", sLogDir, now.Month, now.Day, now.Hour, now.Minute, now.Second);
#else
        m_sLogFilePath = Application.persistentDataPath + "/running.log";
#endif

        UnityEngine.Debug.Log(m_sLogFilePath);

        m_bRun = true;
        m_Thread = new Thread(new ThreadStart(Run));
        m_Thread.IsBackground = true;
        m_Thread.Start();
    }

    public string [] GetLogContent()
    {
        if(File.Exists(m_sLogFilePath))
        {
            string name = m_sLogFilePath + ".bk";
            File.Copy(m_sLogFilePath, name);
            string [] lines = File.ReadAllLines(name);
            File.Delete(name);
            return lines;
        }
        else
        {
            return null;
        }
    }

    public void Release()
    {
#if UNITY_EDITOR
        UnityEngine.Debug.LogWarning("Log Release ...");
#endif
        m_bRun = false;
        m_ResetEvent.Set();
    }

    public void Push(LogType type, string message)
    {
        if (!m_bRun)
            return;

        LogData data;
        data.type = type;
        data.message = message;
        data.date = DateTime.Now.ToString("u", DateTimeFormatInfo.InvariantInfo);

        lock (m_Lock)
        {
            m_LogQueue.Push(data);
        }

        m_ResetEvent.Set();
    }

    private void Run()
    {
        if (File.Exists(m_sLogFilePath))
            File.Delete(m_sLogFilePath);

        FileStream file = new FileStream(m_sLogFilePath, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite);
        StreamWriter writer = new StreamWriter(file);

        do
        {
            m_ResetEvent.WaitOne();

            LogData node;
            while (m_LogQueue.Pop(out node))
            {
                switch (node.type)
                {
                    case LogType.Warning:
                        writer.Write($"[WARN] {node.date} {node.message}\n");
                        WarnCount ++;
                        break;

                    case LogType.Error:
                        ErrorCount ++;
                        writer.Write($"[ERROR] {node.date} {node.message}\n");
                        break;

                    default:
                        InfoCount ++;
                        writer.Write($"[INFO] {node.date} {node.message}\n");
                        break;
                }
            }

            writer.Flush();
            m_ResetEvent.Reset();

            if (!m_bRun)
            {
                break;
            }

        } while (true);


        // 退出线程之前做收尾工作

        //关闭流   
        writer.Close();
        writer.Dispose();

        file.Close();
        file.Dispose();

#if UNITY_EDITOR
        UnityEngine.Debug.Log("File Log Quit");
#endif
    }
}