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
            for(int i = 0; i < args.Length; i ++)
            {
                Console.WriteLine(args[i]);
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





