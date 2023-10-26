//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/Game/Gate/GateHandler.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：服务器相关定义
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using GateA;
using XSF;

public class GateHandler : IGateHandler
{
    public void OnClientClose(uint nClientID)
    {
        Serilog.Log.Information("Client close, client id=" + nClientID);
        ActorManager.Instance.OnActorLogout(nClientID);
    }
}
