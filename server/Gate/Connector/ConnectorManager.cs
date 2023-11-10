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
    List<ServerConnector>[] m_EPConnectors;

    public ConnectorManager()
    {
        m_Connectors = new Dictionary<uint, ServerConnector>();
        m_EPConnectors = new List<ServerConnector>[(int)EP.Max];
        for(int i = 0; i < m_EPConnectors.Length; i ++)
        {
            m_EPConnectors[i] = new List<ServerConnector>();
        }
    }

    public override void DoRegist()
    {
        XSFCore.SetMessageExecutor((ushort)XsfPbid.SMSGID.GtAGtHandshake, new Executor_GtA_Gt_Handshake());
        XSFCore.SetMessageExecutor((ushort)XsfPbid.SMSGID.GtAGtClientDisconnect, new Executor_GtA_Gt_ClientDisconnect());
        XSFCore.SetMessageExecutor((ushort)XsfPbid.SMSGID.GtAGtClientMessage, new Executor_GtA_Gt_ClientMessage());
        XSFCore.SetMessageExecutor((ushort)XsfPbid.SMSGID.GtAGtBroadcast, new Executor_GtA_Gt_Broadcast());
        XSFCore.SetMessageExecutor((ushort)XsfPbid.SMSGID.GtAGtSetServerId, new Executor_GtA_Gt_SetServerID());
    }

    public override void OnClose()
    {
        foreach(KeyValuePair<uint, ServerConnector> kv in m_Connectors)
        {
            kv.Value.OnClose();
        }
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

        var sid = ServerID.GetSID(nID);
        m_EPConnectors[sid.Type].Add(connector);
    }

    public void DeleteConnector(uint nID)
    {
        ServerConnector connector = null;
        if(m_Connectors.TryGetValue(nID, out connector))
        {
            m_Connectors.Remove(nID);
            var sid = ServerID.GetSID(nID);

            m_EPConnectors[sid.Type].Remove(connector);
        }
    }

    public ServerConnector GetConnector(byte nEP, uint nID)
    {
        if(nID > 0)
        {
            ServerConnector connector = null;
            m_Connectors.TryGetValue(nID, out connector);
            return connector;
        }
        else
        {
            var list = m_EPConnectors[nEP];
            int nTotal = list.Count;
            if(nTotal <= 0)
                return null;

            int nIndex = XSFCore.RandomRange(0, nTotal);
            return list[nIndex];
        }
    }

    public static ConnectorManager Instance { get; private set; }

    public static void CreateModule()
    {
        Instance = new ConnectorManager();

        ModuleInit init = new ModuleInit();
        init.ID = (int)ModuleID.ConnectorManager;
        init.Name = "ConnectorManager";

        XSFCore.Server.AddModule(Instance, init);
    }
}