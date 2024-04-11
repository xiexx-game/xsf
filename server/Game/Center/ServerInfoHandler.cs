//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/Gate/Center/ServerInfoHandler.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：服务器相关定义
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using CC;
using XSF;

public class ServerInfoHandler : Singleton<ServerInfoHandler>, IServerInfoHandler
{
    public static uint TargetServerID;

    // 有一个服务器连入到集群
    public void OnServerNew(ServerInfo info)
    {

    }

    // 有一个服务器断开
    public void OnServerLost(uint nID)
    {

    }

    public void OnServerOk(ServerInfo info)
    {
        var sid = ServerID.GetSID(info.ID);
        switch((EP)sid.Type)
        {
        case EP.DB:
            {
                DBC.IDBConnector.Get((int)ModuleID.DBConnector).Connect(info.IP, (int)info.Ports[(int)EP.DB]);
            }
            break;

        case EP.Game:
            {
                TargetServerID = info.ID;
                Serilog.Log.Information("Target Game ID=" + TargetServerID);
            }
            break;

        case EP.Hub:
            {
                HubC.IHubConnector.Get((int)ModuleID.HubConnector).Connect(info.IP, (int)info.Ports[(int)EP.Hub]);
            }
            break;
        }
    }
}
