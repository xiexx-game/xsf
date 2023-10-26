//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/GateAcceptor/MessageExecutor.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：消息处理
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8600, CS8602, CS8604
using XSF;

namespace GateA
{
    internal class Executor_Gt_GtA_Handshake : IMessageExecutor
    {
        public void OnExecute(object NetObj, IMessage message, ushort nMessageID, uint nRawID, byte[] rawData)
        {
            var gate = NetObj as Gate;
            var localMsg = message as XsfMsg.MSG_Gt_GtA_Handshake;
            gate.SetID(localMsg.mPB.ServerId);

            if(gate.Owner.Add(gate))
            {
                var respMsg = XSFUtil.GetMessage((ushort)XsfPb.SMSGID.GtAGtHandshake) as XsfMsg.MSG_GtA_Gt_Handshake;
                respMsg.mPB.ServerId = XSFUtil.Server.ID;
                gate.SendMessage(respMsg);
            }
        }
    }

    internal class Executor_Gt_GtA_Heartbeat : IMessageExecutor
    {
        public void OnExecute(object NetObj, IMessage message, ushort nMessageID, uint nRawID, byte[] rawData)
        {
            var gate = NetObj as Gate;
            gate.UpdateHTTime();
        }
    }

    internal class Executor_Gt_GtA_ClientClose : IMessageExecutor
    {
        public void OnExecute(object NetObj, IMessage message, ushort nMessageID, uint nRawID, byte[] rawData)
        {
            var localMsg = message as XsfMsg.MSG_Gt_GtA_ClientClose;
            IGateAcceptor.m_Instance.m_Handler.OnClientClose(localMsg.mPB.ClientId);
        }
    }
}
