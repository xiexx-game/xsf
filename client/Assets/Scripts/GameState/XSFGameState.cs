//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Scripts/GameState/XSFGameState.cs
// 作者：Xiexx
// 时间：2022/03/05
// 描述：游戏状态机
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using System;

public enum XSFGSID
{ 
    None = 0,
    Main,
    Play,

    Max,
}

public class XSFGameState
{
    public virtual XSFGSID mID { get { return XSFGSID.None; } }

    public virtual bool Enter()
    {
        return true;
    }

    public virtual void Exit()
    { 
        
    }

    public virtual void OnUpdate()
    { 
        
    }
}
