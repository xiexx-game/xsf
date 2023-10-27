//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/CenterConnector/MessageExecutor.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：消息处理
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8600, CS8602, CS8604
using XSF;

namespace CC
{
    internal class Executor_C_Cc_Handshake : IMessageExecutor
    {
        public void OnExecute(object NetObj, IMessage message, ushort nMessageID, uint nRawID, byte[] rawData)
        {
            var connector = NetObj as CenterConnector;
            var localMsg = message as XsfMsg.MSG_C_Cc_Handshake;
            connector.SetID(localMsg.mPB.ServerId);

            var Server = XSFCore.Server;
            Server.SetID(localMsg.mPB.NewId);
            Serilog.Log.Information("收到中心服握手，更新本服ID, Server ID={0} - [{1}-{2}-{3}]", Server.ID, Server.SID.ID, Server.SID.Index, Server.SID.Type);

            for(int i = 0; i < localMsg.mPB.Ports.Count; i ++)
            {
                Server.SetPort((byte)i, localMsg.mPB.Ports[i]);
            }

            connector.OnHandshake();
            Server.DoStart();

            if(Server.IsRunning)
            {
                connector.OnOK();
            }

        }
    }

    internal class Executor_C_Cc_ServerInfo : IMessageExecutor
    {
        public void OnExecute(object NetObj, IMessage message, ushort nMessageID, uint nRawID, byte[] rawData)
        {
            var connector = NetObj as CenterConnector;
            var localMsg = message as XsfMsg.MSG_C_Cc_ServerInfo;
            connector.AddInfo(localMsg);
        }
    }

    internal class Executor_C_Cc_ServerLost : IMessageExecutor
    {
        public void OnExecute(object NetObj, IMessage message, ushort nMessageID, uint nRawID, byte[] rawData)
        {
            var connector = NetObj as CenterConnector;
            var localMsg = message as XsfMsg.MSG_C_Cc_ServerLost;
            connector.OnNodeLost(localMsg.mPB.ServerId);
        }
    }

    internal class Executor_C_Cc_ServerOk : IMessageExecutor
    {
        public void OnExecute(object NetObj, IMessage message, ushort nMessageID, uint nRawID, byte[] rawData)
        {
            var connector = NetObj as CenterConnector;
            var localMsg = message as XsfMsg.MSG_C_Cc_ServerOk;
            connector.OnNodeOk(localMsg.mPB.ServerId);
        }
    }

    internal class Executor_C_Cc_Stop : IMessageExecutor
    {
        public void OnExecute(object NetObj, IMessage message, ushort nMessageID, uint nRawID, byte[] rawData)
        {
            XSFCore.Server.Stop();
        }
    }
}
