//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/Hub/Node/MessageExecutor.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：消息处理
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8600, CS8602

using XSF;
using Google.Protobuf.Collections;

public sealed class Executor_Hc_H_Handshake : IMessageExecutor
{
    public void OnExecute(object NetObj, IMessage message, ushort nMessageID, uint nRawID, byte[] rawData)
    {
        var localMsg = message as XsfMsg.MSG_Hc_H_Handshake;
        NetPoint node = NetObj as NetPoint;

        node.SetID(localMsg.mPB.ServerId);

        if(NodeManager.Instance.Add(node))
        {
            var respMsg = XSFCore.GetMessage((ushort)XsfPbid.SMSGID.HHcHandshake) as XsfMsg.MSG_H_Hc_Handshake;
            respMsg.mPB.ServerId = XSFCore.Server.ID;
            node.SendMessage(respMsg);
        }
    }
}

public sealed class Executor_Hc_H_Heartbeat : IMessageExecutor
{
    public void OnExecute(object NetObj, IMessage message, ushort nMessageID, uint nRawID, byte[] rawData)
    {
        NetPoint node = NetObj as NetPoint;
        node.UpdateHTTime();
    }
}
