//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Scripts/GameState/XSFGameState.cs
// 作者：Xoen
// 时间：2023/08/25
// 描述：游戏状态机
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using System;

public enum XSFGSID
{ 
    None = 0,

    Home,
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
