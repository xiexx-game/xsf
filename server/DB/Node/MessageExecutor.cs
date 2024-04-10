//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/DB/Node/MessageExecutor.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：消息处理
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8600, CS8602

using XSF;
using Google.Protobuf.Collections;

public sealed class Executor_Dbc_Db_Handshake : IMessageExecutor
{
    public void OnExecute(object NetObj, IMessage message, ushort nMessageID, uint nRawID, byte[] rawData)
    {
        var localMsg = message as XsfMsg.MSG_Dbc_Db_Handshake;
        NetPoint node = NetObj as NetPoint;

        node.SetID(localMsg.mPB.ServerId);

        if(NodeManager.Instance.Add(node))
        {
            var respMsg = XSFCore.GetMessage((ushort)XsfPbid.SMSGID.DbDbcHandshake) as XsfMsg.MSG_Db_Dbc_Handshake;
            respMsg.mPB.ServerId = XSFCore.Server.ID;
            node.SendMessage(respMsg);
        }
    }
}

public sealed class Executor_Dbc_Db_Heartbeat : IMessageExecutor
{
    public void OnExecute(object NetObj, IMessage message, ushort nMessageID, uint nRawID, byte[] rawData)
    {
        NetPoint node = NetObj as NetPoint;
        node.UpdateHTTime();
    }
}

public sealed class Executor_Dbc_Db_Request : IMessageExecutor
{
    public void OnExecute(object NetObj, IMessage message, ushort nMessageID, uint nRawID, byte[] rawData)
    {
        NetPoint node = NetObj as NetPoint;
        var localMsg = message as XsfMsg.MSG_Dbc_Db_Request;
        var code = MysqlManager.Instance.MysqlRequest(node.ID, localMsg.mPB.SerialId, localMsg.mPB.DbRequestId, localMsg.mPB.QueueId, localMsg.mPB.Datas.ToByteArray());
        if(code != (uint)XsfPb.OpResult.Ok)
        {
            var respMsg = XSFCore.GetMessage((ushort)XsfPbid.SMSGID.DbDbcResponse) as XsfMsg.MSG_Db_Dbc_Response;
            respMsg.mPB.SerialId = localMsg.mPB.SerialId;
            respMsg.mPB.Code = code;
            respMsg.mPB.Total = 0;
            respMsg.mPB.Datas = Google.Protobuf.ByteString.Empty;
            node.SendMessage(respMsg);
        }
    }
}