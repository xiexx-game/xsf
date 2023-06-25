//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\Scripts\Startup\Steps\StartupStepLua.cs
// 作者：Xoen Xie
// 时间：2023/06/24
// 描述：游戏启动 - Lua
// 说明：
//
//////////////////////////////////////////////////////////////////////////

public sealed class StartupStepLua : StartupStep
{
    public override void Start()
    {
        XSF.LogWarning("StartupStepLua Start");
        XSFLua.Instance.Init();
        XSFLua.Instance.LuaStart();
        Subscribe((EventID)XSF.LUA_EVENT_ID, 0);
    }
}