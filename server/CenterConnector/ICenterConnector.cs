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
    public abstract class ICenterConnector : NetConnector
    {
        public static ICenterConnector CreateModule(int nID)
        {
            var connector = new CenterConnector();

            NetConnectorInit init = new NetConnectorInit();
            init.ID = nID;
            init.Name = "CenterConnector";
            init.NoWaitStart = true;
            init.NeedReconnect = true;

            XSFUtil.Server.AddModule(connector, init);

            return connector;
        }
    }
}