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

public class Executor_GtA_Gt_Handshake : IMessageExecutor
{
    public void OnExecute(object NetObj, IMessage message, ushort nMessageID, uint nRawID, byte[] rawData)
    {
        var connector = NetObj as ServerConnector;
        connector.OnHandshake();
    }
}

public class Executor_GtA_Gt_ClientDisconnect : IMessageExecutor
{
    public void OnExecute(object NetObj, IMessage message, ushort nMessageID, uint nRawID, byte[] rawData)
    {
        var localMsg = message as XsfMsg.MSG_GtA_Gt_ClientDisconnect;

        var client = ClientManager.Instance.GetClient(localMsg.mPB.ClientId);
        if(client != null)
        {
            client.Disconnect((int)localMsg.mPB.Reason);
        }
    }
}

public class Executor_GtA_Gt_ClientMessage : IMessageExecutor
{
    public void OnExecute(object NetObj, IMessage message, ushort nMessageID, uint nRawID, byte[] rawData)
    {
        var localMsg = message as XsfMsg.MSG_GtA_Gt_ClientMessage;
        var MsgData = localMsg.mPB.ClientMessage.ToByteArray();

        for(int i = 0; i < localMsg.mPB.ClientId.Count; i ++)
        {
            var client = ClientManager.Instance.GetClient(localMsg.mPB.ClientId[i]);
            if(client != null)
            {
                client.SendData(MsgData);
            }
        }
    }
}

public class Executor_GtA_Gt_Broadcast : IMessageExecutor
{
    public void OnExecute(object NetObj, IMessage message, ushort nMessageID, uint nRawID, byte[] rawData)
    {
        var localMsg = message as XsfMsg.MSG_GtA_Gt_Broadcast;
        var MsgData = localMsg.mPB.ClientMessage.ToByteArray();
        ClientManager.Instance.Broadcast(MsgData);
    }
}

public class Executor_GtA_Gt_SetServerID : IMessageExecutor
{
    public void OnExecute(object NetObj, IMessage message, ushort nMessageID, uint nRawID, byte[] rawData)
    {
        var localMsg = message as XsfMsg.MSG_GtA_Gt_SetServerId;
        var client = ClientManager.Instance.GetClient(localMsg.mPB.ClientId);
        if(client != null)
        {
            client.SetServerID((byte)localMsg.mPB.Ep, localMsg.mPB.ServerId);
        }
    }
}



