//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\XSF\GameState\XSFGameStatePlay.cs
// 作者：Xoen
// 时间：2022/04/08
// 描述：游戏状态机
// 说明：
//
//////////////////////////////////////////////////////////////////////////

public class XSFGameStatePlay : XSFGameState
{
    public override XSFGSID mID { get { return XSFGSID.Play; } }

    public override bool Enter()
    {
        Level.Instance.Enter();
        return true;
    }

    public override void Exit()
    {
        Level.Instance.Exit();
    }

    public override void OnUpdate()
    {
        Level.Instance.OnUpdate();
    }
}
