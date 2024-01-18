//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Scripts/Level/PacMan/AI/AI_Blinky.cs
// 作者：Xoen Xie
// 时间：2023/06/21
// 描述：鬼AI - 小红
// 说明：
//
//////////////////////////////////////////////////////////////////////////

public class AI_Blinky : AI_Ghost
{
    public override void OnBorn()
    {
        m_Ghost.MoveDir = PacManMoveDir.Left;
        DoMove(true);
    }

    public override PacManMapBlock GetTarget()
    {
        return LevelGamePackMan.Instance.Character.Current;
    }
}