//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/HubConnector/MessageExecutor.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：消息处理
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8600, CS8602, CS8604
using XSF;

namespace HubC
{
    internal class Executor_H_Hc_Handshake : IMessageExecutor
    {
        public void OnExecute(object NetObj, IMessage message, ushort nMessageID, uint nRawID, byte[] rawData)
        {
            var connector = NetObj as HubConnector;
            var localMsg = message as XsfMsg.MSG_H_Hc_Handshake;
            connector.SetID(localMsg.mPB.ServerId);
            connector.OnHandshake();
        }
    }
}
