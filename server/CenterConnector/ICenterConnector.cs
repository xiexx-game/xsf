//////////////////////////////////////////////////////////////////////////
//
// 文件：server/CenterConnector/ICenterConnector.cs
// 作者：Xoen Xie
// 时间：2023/06/15
// 描述：中心服连接器
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using XSF;

namespace CC
{
    public interface IServerInfoHandler
    {
        void OnServerNew(ServerInfo info);       // 有一个服务器连入到集群
	    void OnServerLost(uint nID);              // 有一个服务器断开
        void OnServerOk(ServerInfo info);
    }

    public abstract class ICenterConnector : NetConnector
    {
        public static void CreateModule(int nID, IServerInfoHandler handler)
        {
            var connector = new CenterConnector();
            connector.m_Handler = handler;

            NetConnectorInit init = new NetConnectorInit();
            init.ID = nID;
            init.Name = "CenterConnector";
            init.NoWaitStart = true;
            init.NeedReconnect = true;

            XSFCore.Server.AddModule(connector, init);
        }
    }
}