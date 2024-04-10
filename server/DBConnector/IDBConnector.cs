//////////////////////////////////////////////////////////////////////////
//
// 文件：server/DBConnector/IDBConnector.cs
// 作者：Xoen Xie
// 时间：2023/06/15
// 描述：数据服连接器
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8603
using XSF;
using DBUtil;

namespace DBC
{
    public interface IDBHandler
    {
        void OnMysqlResult(DBSqlBase sql, uint nCode, uint nKeyID, uint nTotal, byte[] data);
    }

    public abstract class IDBConnector : NetConnector
    {
        public static void CreateModule(int nID)
        {
            var connector = new DBConnector();

            NetConnectorInit init = new NetConnectorInit();
            init.ID = nID;
            init.Name = "DBConnector";
            init.NeedReconnect = true;

            XSFCore.Server.AddModule(connector, init);
        }

        public static IDBConnector Get(int nID)
        {
            return XSFCore.Server.GetModule(nID) as IDBConnector;
        }

        public abstract void SendRequest(IDBHandler handler, DBUtil.DBSqlBase sql, uint nKeyID, uint QueueID);
    }
}