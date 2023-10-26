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

