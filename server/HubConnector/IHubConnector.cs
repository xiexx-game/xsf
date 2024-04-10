//////////////////////////////////////////////////////////////////////////
//
// 文件：server/HubConnector/IHubConnector.cs
// 作者：Xoen Xie
// 时间：2023/06/15
// 描述：数据服连接器
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8603
using XSF;

namespace HubC
{
    public abstract class IHubConnector : NetConnector
    {
        public static void CreateModule(int nID)
        {
            var connector = new HubConnector();

            NetConnectorInit init = new NetConnectorInit();
            init.ID = nID;
            init.Name = "HubConnector";
            init.NeedReconnect = true;

            XSFCore.Server.AddModule(connector, init);
        }

        public static IHubConnector Get(int nID)
        {
            return XSFCore.Server.GetModule(nID) as IHubConnector;
        }

        public abstract void Send2Server(uint nServerID, IMessage message);
    }
}