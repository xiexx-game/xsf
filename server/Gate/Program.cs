// See https://aka.ms/new-console-template for more information
namespace Gate // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static void Main(string[] args)
        {
            XSF.XSFCore.Server.Init(XSF.EP.Gate, args);
            XsfMsg.MessageModule.CreateModule();
            CC.ICenterConnector.CreateModule((int)ModuleID.CenterConnector, ServerInfoHandler.Instance);

            GateClient.ClientManager.CreateModule();
            ConnectorManager.CreateModule();

            XSF.XSFCore.Server.Run();

        }
    }
}





