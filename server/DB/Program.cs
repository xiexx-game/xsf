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
            XSF.XSFCore.Server.Init(XSF.EP.DB, args);
            XsfScp.SchemaModule.CreateModule();
            XsfMsg.MessageModule.CreateModule();
            NodeManager.CreateModule();
            CC.ICenterConnector.CreateModule((int)ModuleID.CenterConnector, null);

            MysqlManager.CreateModule();

            XSF.XSFCore.Server.Run();
        }
    }
}





