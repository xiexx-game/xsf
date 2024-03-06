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
    public override BlockType SetType { get { return BlockType.GhostInky; } }

    public override void OnBorn()
    {
        m_nState = AIState.IdleLeft;
    }

    public override void OnAIUpdate()
    {
        if(m_nState == AIState.IdleLeft || m_nState == AIState.IdleRight)
        {
            if(LevelGamePackMan.Instance.Map.BeanCount <= LevelGamePackMan.Instance.ScpLevels.uDotsLeft1)
            {
                m_nState = AIState.GoOut2Start;
            }
        }
    }

    public override PacManMapBlock GetTarget()
    {
        if(LevelGamePackMan.Instance.Character.Speed.HasEnergy)
        {
            return LevelGamePackMan.Instance.Map.FleeTargets[(int)GhostType.Inky];
        }
        else
        {
            PacManMapBlock center = GetCenterBlock();

            int colOffset = m_Current.scp.iCol - center.scp.iCol;
            int rowOffset = m_Current.scp.iRow - center.scp.iRow;

            int colTarget = center.scp.iCol - colOffset;
            int rowTarget = center.scp.iRow - rowOffset;

            PacManMapBlock target = LevelGamePackMan.Instance.Map.GetBlock(rowTarget, colTarget);
            if( target == null )
            {
                return center;
            }
            else
            {
                if(target.IsRoad)
                {
                    return target;
                }
                else
                {
                    return center;
                }
            }
        }
    }
    
    private PacManMapBlock GetCenterBlock()
    {
        var current = LevelGamePackMan.Instance.Character.RoadCurrent;
        var dir = LevelGamePackMan.Instance.Character.MoveDir;

        int nCount = 0;
        while(nCount < 2)
        {
            if(current.ConnectIndex[(int)dir] > 0)
            {
                current = LevelGamePackMan.Instance.Map.GetBlockByIndex(current.ConnectIndex[(int)dir]);
                nCount ++;
            }
            else
            {
                int nextDir = (int)dir +1;

                while(true)
                {
                    if(nextDir >= (int)PacManMoveDir.Max)
                        nextDir = (int)PacManMoveDir.Up;

                    if(nextDir == (int)dir)
                        return current;

                    if(current.ConnectIndex[(int)nextDir] > 0)
                    {
                        current = LevelGamePackMan.Instance.Map.GetBlockByIndex(current.ConnectIndex[(int)nextDir]);
                        nCount ++;
                        break;
                    }

                    nextDir ++;
                }
            }
        }

        return current;
    }
}