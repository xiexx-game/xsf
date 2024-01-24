// See https://aka.ms/new-console-template for more information
using System;
using System.Threading;
using System.Diagnostics;

namespace XSFTools // Note: actual namespace depends on the project name.
{
    public class Logger : IToolLogger
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }

        public void LogError(string message)
        {
            Console.Error.WriteLine(message);
        }
    }

    internal class Program
    {
        public static void Main(string[] args)
        {
            XSFTools.Platform p;
            string platform = "";
            if(GetArg(args, "-p", out platform))
            {
                if(platform == "win")
                {
                    p = XSFTools.Platform.Windows;
                }
                else if(platform == "mac")
                {
                    p = XSFTools.Platform.Mac;
                }
                else
                {
                    p = XSFTools.Platform.Linux;
                }

                Console.WriteLine($"指定平台 {p}");
            }
            else
            {
                Console.Error.WriteLine("未指定参数-p, 请指定操作系统平台：win, mac, linux");
                return;
            }

            string rootPath = "";
            if(GetArg(args, "-root", out rootPath))
            {
                Console.WriteLine($"工程根目录： {rootPath}");
            }
            else
            {
                Console.Error.WriteLine("未指定参数-root, 请指定工程root目录");
                return;
            }

            string c = "";
            bool IsConfig = GetArg(args, "-c", out c);

            XSFTools.Helper.Instance.Init(p, new Logger(), rootPath, IsConfig);
            if(IsConfig)
            {
                Console.WriteLine("开始导出配置....");
                XSFTools.SchemaTools.DoExport();
                Console.WriteLine("配置导出完毕....");
            }
            else
            {
                Console.WriteLine("开始导出proto....");
                XSFTools.ProtoTool.ProtoExport();
                XSFTools.ProtoTool.CodeGen();
                Console.WriteLine("proto导出完毕....");
            }
        }

        public static bool GetArg(string[] args, string tag, out string data)
        {
            data = "";
            for(int i = 0; i < args.Length; i ++)
            {
                if(args[i].Equals(tag))
                {
                    if(i+1>= args.Length)
                    {
                        return true;
                    }
                    else
                    {
                        data = args[i+1];
                        return true;
                    }
                }
            }

            return false;
        }
    }
}





