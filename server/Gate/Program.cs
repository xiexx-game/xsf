// See https://aka.ms/new-console-template for more information
namespace Gate // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static void Main(string[] args)
        {
            XSF.XSFUtil.Server.Init(XSF.EP.Gate, args);
            XsfMsg.MessageModule.CreateModule();
            CC.ICenterConnector.CreateModule((int)ModuleID.CenterConnector, ServerInfoHandler.Instance);


            XSF.XSFUtil.Server.Run();
        }
    }
}





