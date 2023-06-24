//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\Scripts\Startup\Steps\StartupStepProto.cs
// 作者：Xoen Xie
// 时间：2023/06/24
// 描述：游戏启动 - Lua
// 说明：
//
//////////////////////////////////////////////////////////////////////////

public sealed class StartupStepProto : StartupStep
{
    public override void Start()
    {
        XSFLua.Instance.ProtoStart();
        Subscribe((EventID)XSF.PROTO_EVENT_ID, 0);
    }
}