//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\XSF\GameState\XSFGameStateMain.cs
// 作者：Xiexx
// 时间：2022/04/11
// 描述：游戏状态机
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using System;

public class XSFGameStateMain : XSFGameState
{
    public override XSFGSID mID { get { return XSFGSID.Main; } }

    public override bool Enter()
    {
        

        return true;
    }

    public override void End()
    {

    }
}
