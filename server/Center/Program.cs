// See https://aka.ms/new-console-template for more information
using System;
using Serilog;
using System.Threading;
using System.Diagnostics;

namespace Center // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static void Main(string[] args)
        {
            XSF.XSFUtil.Server.Init(XSF.EP.Center, args);

            XsfScp.SchemaModule.CreateModule();

            XSF.XSFUtil.Server.Run();

            Console.WriteLine("Program main end");
        }
    }
}





