// See https://aka.ms/new-console-template for more information
namespace Gate // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static void Main(string[] args)
        {
            XSF.XSFCore.Server.Init(XSF.EP.Game, args);
            XsfScp.SchemaModule.CreateModule();
            XsfMsg.MessageModule.CreateModule();
            
            CC.ICenterConnector.CreateModule((int)ModuleID.CenterConnector, ServerInfoHandler.Instance);
            GateA.IGateAcceptor.CreateModule((int)ModuleID.GateAcceptor, new GateHandler());
            ActorManager.CreateModule();
            DBC.IDBConnector.CreateModule((int)ModuleID.DBConnector);
            HubC.IHubConnector.CreateModule((int)ModuleID.HubConnector);

            XSF.XSFCore.Server.Run();
        }
    }
}





