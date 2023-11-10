//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/Gate/Client/MessageExecutor.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：消息处理
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8600, CS8602

using XSF;
using GateClient;
using Google.Protobuf.Collections;

public class Executor_Clt_Gt_Handshake : IMessageExecutor
{
    public void OnExecute(object NetObj, IMessage message, ushort nMessageID, uint nRawID, byte[] rawData)
    {
        var client = NetObj as Client;
        client.OnHandshake();

        var respMsg = XSFCore.GetMessage((ushort)XsfPbid.CMSGID.GtCltHandshake);
        client.SendMessage(respMsg);
    }
}

public class Executor_Clt_Gt_Heartbeat : IMessageExecutor
{
    public void OnExecute(object NetObj, IMessage message, ushort nMessageID, uint nRawID, byte[] rawData)
    {
        var client = NetObj as Client;
        client.UpdateHTTime();

        var localMsg = message as XsfMsg.MSG_Clt_Gt_Heartbeat;

        var respMsg = XSFCore.GetMessage((ushort)XsfPbid.CMSGID.GtCltHeartbeat) as XsfMsg.MSG_Gt_Clt_Heartbeat;
        respMsg.mPB.ClientTime = localMsg.mPB.Time;
        respMsg.mPB.ServerTime = XSFCore.CurrentMS;
        client.SendMessage(respMsg);
    }
}

