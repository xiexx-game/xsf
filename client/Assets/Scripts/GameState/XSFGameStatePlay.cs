//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Scripts/GameState/XSFGameStatePlay.cs
// 作者：Xiexx
// 时间：2022/04/08
// 描述：游戏状态机
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using System;

public class XSFGameStatePlay : XSFGameState
{
    public override XSFGSID mID { get { return XSFGSID.Play; } }

    public override bool Enter()
    {




        return true;
    }

    public override void End()
    {
        //Playground.mInstance.End();
    }

    public override void Update()
    {

    }
}
