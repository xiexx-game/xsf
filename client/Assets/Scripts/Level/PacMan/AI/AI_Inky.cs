//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Scripts/Level/PacMan/AI/AI_Inky.cs
// 作者：Xoen Xie
// 时间：2023/06/21
// 描述：鬼AI - 小兰
// 说明：
//
//////////////////////////////////////////////////////////////////////////

public class AI_Inky : AI_Ghost
{
    public override void OnBorn()
    {
        m_nState = AIState.IdleLeft;
    }

    public override PacManMapBlock GetTarget()
    {
        return null;
    }
    
}