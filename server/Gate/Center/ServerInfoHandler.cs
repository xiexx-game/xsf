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
    // 有一个服务器连入到集群
    public void OnServerNew(ServerInfo info)
    {
        var sid = ServerID.GetSID(info.ID);
        if(sid.Type == (byte)EP.Game)
        {
            if(info.Status == NodeStatus.Ok)
            {
                ConnectorManager.Instance.CreateConnector(info.ID, info.IP, (int)info.Ports[(int)EP.Gate]);
            }
        }
    }

    // 有一个服务器断开
    public void OnServerLost(uint nID)
    {

    }

    public void OnServerOk(ServerInfo info)
    {
        var sid = ServerID.GetSID(info.ID);
        if(sid.Type == (byte)EP.Game)
        {
            ConnectorManager.Instance.CreateConnector(info.ID, info.IP, (int)info.Ports[(int)EP.Gate]);
        }
    }
}