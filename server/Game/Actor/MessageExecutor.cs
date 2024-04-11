//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/Game/Actor/MessageExecutor.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：消息处理
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8600, CS8602

using XSF;
using Google.Protobuf.Collections;

public class Executor_Clt_G_Login : IMessageExecutor
{
    public void OnExecute(object NetObj, IMessage message, ushort nMessageID, uint nRawID, byte[] rawData)
    {
        Actor a = ActorManager.Instance.DoLogin(nRawID);
        a.OnLoginOk();
    }
}

public class Executor_G_G_HubTest : IMessageExecutor
{
    public void OnExecute(object NetObj, IMessage message, ushort nMessageID, uint nRawID, byte[] rawData)
    {
        var localMsg = message as XsfMsg.MSG_G_G_HubTest;
        switch(localMsg.mPB.Code)
        {
        case 1:
            {
                Serilog.Log.Information($"Executor_G_G_HubTest code is 1, 来源服务器={nRawID}, 本地={XSFCore.Server.ID}");
                localMsg.mPB.Code = 2;
                HubC.IHubConnector.Get((int)ModuleID.HubConnector).Send2Server(nRawID, localMsg);
                Serilog.Log.Information("收到1，往对方发送2");
            }
            break;

        case 2:
            {
                Serilog.Log.Information($"Executor_G_G_HubTest code is 2, 来源服务器={nRawID}, 本地={XSFCore.Server.ID}");
                localMsg.mPB.Code = 3;
                HubC.IHubConnector.Get((int)ModuleID.HubConnector).Send2Server(0, localMsg);
                Serilog.Log.Information("收2，广播3");
            }
            break;

        case 3:
            {
                Serilog.Log.Information($"Executor_G_G_HubTest code is 3, 来源服务器={nRawID}, 本地={XSFCore.Server.ID}");
            }
            break;
        }
        
    }
}

