//////////////////////////////////////////////////////////////////////////
//
// 文件：server/DBConnector/DBConnector.cs
// 作者：Xoen Xie
// 时间：2023/06/15
// 描述：数据服连接器
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8602, CS8600, CS8618

using XSF;
using Google.Protobuf.Collections;

namespace DBC
{
    internal class DBRequest 
    {
        public ulong nSerialID;

        public IDBHandler handler;
        public uint nKeyID;
        public uint nQueueID;

        public DBUtil.DBSqlBase sql;
    }

    internal class DBConnector : IDBConnector
    {
        Dictionary<ulong, DBRequest> m_Requests;

        private uint m_nIndex;

        public DBConnector()
        {
            m_Requests = new Dictionary<ulong, DBRequest>();
            m_nIndex = 0;
        }

        public override void DoRegist()
        {
            XSFCore.SetMessageExecutor((ushort)XsfPbid.SMSGID.DbDbcHandshake, new Executor_Db_Dbc_Handshake());
            XSFCore.SetMessageExecutor((ushort)XsfPbid.SMSGID.DbDbcResponse, new Executor_Db_Dbc_Response());
        }

        public override ModuleRunCode OnStartCheck()
        {
            if(IsHandshake)
            {
                return ModuleRunCode.OK;
            }

            return ModuleRunCode.Wait;
        }

        public override void SendHandshake()
        {
            Serilog.Log.Information("DBConnector SendHandshake");
            var message = XSFCore.GetMessage((ushort)XsfPbid.SMSGID.DbcDbHandshake) as XsfMsg.MSG_Dbc_Db_Handshake;
            message.mPB.ServerId = XSFCore.Server.ID;

            SendMessage(message);
        }

        public override void SendHeartbeat()
        {
            var message = XSFCore.GetMessage((ushort)XsfPbid.SMSGID.DbcDbHeartbeat);
            SendMessage(message);
        }

        public override void SendRequest(IDBHandler handler, DBUtil.DBSqlBase sql, uint nKeyID, uint nQueueID)
        {
            DBRequest request = new DBRequest();
            request.handler = handler;
            request.nSerialID = XSFCore.UINT64_ID(XSFCore.CurrentS, m_nIndex++);
            request.sql = sql;
            request.nKeyID = nKeyID;
            request.nQueueID = nQueueID;
            
            m_Requests.Add(request.nSerialID, request);

            DoRequest(request);
        }

        private void DoRequest(DBRequest request)
        {
            var message = XSFCore.GetMessage((ushort)XsfPbid.SMSGID.DbcDbRequest) as XsfMsg.MSG_Dbc_Db_Request;
            message.mPB.SerialId = request.nSerialID;
            message.mPB.DbRequestId = (uint)request.sql.ID;
            message.mPB.QueueId = request.nQueueID;
            request.sql.Export();
            var datas = request.sql.Buffer;
            message.mPB.Datas = Google.Protobuf.ByteString.CopyFrom(request.sql.Buffer, 0, (int)request.sql.Size);

            SendMessage(message);
        }

        public override void OnHandshakeOk()
        {
            foreach( var kv in m_Requests)
            {
                DoRequest(kv.Value);
            }
        }

        public void OnResponse(ulong nSerialID, uint nCode, uint nTotal, byte[] datas)
        {
            if(m_Requests.TryGetValue(nSerialID, out DBRequest request))
            {
                request.handler.OnMysqlResult(request.sql, nCode, request.nKeyID, nTotal, datas);
            }
        }
    }
}