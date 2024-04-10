// See https://aka.ms/new-console-template for more information
using System;
using Serilog;
using System.Threading;
using System.Diagnostics;

namespace DB // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static void Main(string[] args)
        {
            XSF.XSFCore.Server.Init(XSF.EP.Hub, args);
            XsfMsg.MessageModule.CreateModule();
            NodeManager.CreateModule();
            CC.ICenterConnector.CreateModule((int)ModuleID.CenterConnector, null);

            XSF.XSFCore.Server.Run();
        }
    }
}





