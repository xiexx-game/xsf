//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Scripts/Level/PacMan/AI/AI_Blinky.cs
// 作者：Xoen Xie
// 时间：2023/06/21
// 描述：鬼AI - 小红
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;

public class AI_Blinky : AI_Ghost
{
    public override BlockType SetType { get { return BlockType.GhostBlinky; } }
    
    public override void OnBorn()
    {
        m_Ghost.MoveDir = PacManMoveDir.Left;
        Debug.Log("AI_Blinky OnBorn");
        DoMove(true);   
        Debug.Log("AI_Blinky OnBorn state=" + m_nState);
    }

    public override PacManMapBlock GetTarget()
    {
        Debug.Log("LevelGamePackMan.Instance.Character.Current" + LevelGamePackMan.Instance.Character.Current);
        return LevelGamePackMan.Instance.Character.Current;
    }
}