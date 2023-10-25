//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/Gate/Connector/ConnectorManager.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：消息处理
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8600, CS8602, CS8618, CS8603
using XSF;

public class ConnectorManager : IModule
{
    Dictionary<uint, ServerConnector> m_Connectors;

    List<ServerConnector> m_Temps;

    private uint m_ConnectTime;

    public ConnectorManager()
    {
        m_Connectors = new Dictionary<uint, ServerConnector>();
        m_Temps = new List<ServerConnector>();
    }

    public override void DoRegist()
    {

    }

    public void CreateConnector(uint nID, string ip, int port)
    {
        ServerConnector connector = null;
        if(m_Connectors.TryGetValue(nID, out connector))
        {
            Serilog.Log.Information("ConnectorManager CreateConnector connector exist, id=" + nID);
            return;
        }

        connector = new ServerConnector(this);
        connector.SetID(nID);
        connector.Connect(ip, port);

        m_Connectors.Add(nID, connector);
    }

    public void DeleteConnector(uint nID)
    {
        m_Connectors.Remove(nID);
    }

    public ServerConnector GetConnector(uint nID)
    {
        if(nID > 0)
        {
            ServerConnector connector = null;
            m_Connectors.TryGetValue(nID, out connector);
            return connector;
        }
        else
        {
            
        }
    }

    private static ConnectorManager m_Instance;
    public static ConnectorManager Instance { get { return m_Instance; } }

    public static void CreateModule()
    {
        m_Instance = new ConnectorManager();

        ModuleInit init = new ModuleInit();
        init.ID = (int)ModuleID.ConnectorManager;
        init.Name = "ConnectorManager";

        XSFUtil.Server.AddModule(m_Instance, init);
    }
}