//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/DBConnector/MessageExecutor.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：消息处理
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8600, CS8602, CS8604
using XSF;

namespace DBC
{
    internal class Executor_Db_Dbc_Handshake : IMessageExecutor
    {
        public void OnExecute(object NetObj, IMessage message, ushort nMessageID, uint nRawID, byte[] rawData)
        {
            var connector = NetObj as DBConnector;
            var localMsg = message as XsfMsg.MSG_Db_Dbc_Handshake;
            connector.SetID(localMsg.mPB.ServerId);
            connector.OnHandshake();
        }
    }

    internal class Executor_Db_Dbc_Response : IMessageExecutor
    {
        public void OnExecute(object NetObj, IMessage message, ushort nMessageID, uint nRawID, byte[] rawData)
        {
            var connector = NetObj as DBConnector;
            var localMsg = message as XsfMsg.MSG_Db_Dbc_Response;
            connector.OnResponse(localMsg.mPB.SerialId, localMsg.mPB.Code, localMsg.mPB.Total, localMsg.mPB.Datas.ToByteArray());
        }
    }
}
