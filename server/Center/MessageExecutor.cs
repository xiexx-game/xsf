//////////////////////////////////////////////////////////////////////////
// 
// 文件：Center/MessageExecutor.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：消息处理
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8600, CS8602

using XSF;
using Google.Protobuf.Collections;

public sealed class Executor_Cc_C_Handshake : IMessageExecutor
{
    public void OnExecute(object NetObj, IMessage message, ushort nMessageID, uint nRawID, byte[] rawData)
    {
        var localMsg = message as XsfMsg.MSG_Cc_C_Handshake;

        uint[] ports = new uint[(int)EP.Max];
        for(int i = 0; i < localMsg.mPB.Ports.Count; i ++)
        {
            ports[i] = localMsg.mPB.Ports[i];
        }

        NetPoint node = NetObj as NetPoint;
        ServerInfo sn = NodeManager.Instance.AddNode(localMsg.mPB.ServerId, node.RemoteIP, ports);
        if(sn == null)
        {
            node.Release();
            return;
        }

        node.SetID(sn.ID);

        if(NodeManager.Instance.Add(node))
        {
            var respMsg = XSFUtil.GetMessage((ushort)XsfPb.SMSGID.CCcHandshake) as XsfMsg.MSG_C_Cc_Handshake;
            respMsg.mPB.ServerId = sn.ID;
            respMsg.mPB.Ports.Clear();
            for(int i = 0; i < sn.Ports.Length; i ++ )
            {
                respMsg.mPB.Ports.Add(sn.Ports[i]);
            }
            
            node.SendMessage(respMsg);
        }
    }
}

public sealed class Executor_Cc_C_Heartbeat : IMessageExecutor
{
    public void OnExecute(object NetObj, IMessage message, ushort nMessageID, uint nRawID, byte[] rawData)
    {
        NetPoint node = NetObj as NetPoint;
        node.UpdateHTTime();
    }
}

public sealed class Executor_Cc_C_ServerOk : IMessageExecutor
{
    public void OnExecute(object NetObj, IMessage message, ushort nMessageID, uint nRawID, byte[] rawData)
    {
        NetPoint node = NetObj as NetPoint;
        NodeManager.Instance.OnNodeOk(node.ID);
    }
}