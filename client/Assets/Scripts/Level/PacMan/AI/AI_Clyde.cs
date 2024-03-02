//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Scripts/Level/PacMan/AI/AI_Clyde.cs
// 作者：Xoen Xie
// 时间：2023/06/21
// 描述：鬼AI - 小黄
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;

public class AI_Clyde : AI_Ghost
{
    public override BlockType SetType { get { return BlockType.GhostClyde; } }

    public override void OnBorn()
    {
        m_nState = AIState.IdleRight;
    }

    public override PacManMapBlock GetTarget()
    {
        if(LevelGamePackMan.Instance.Character.Speed.HasEnergy)
        {
            return LevelGamePackMan.Instance.Map.FleeTargets[(int)GhostType.Clyde];
        }
        else
        {
            var start = LevelGamePackMan.Instance.Character.transform.localPosition; start.z = 0;
            var end = m_Ghost.transform.localPosition; end.z = 0;
            if(Vector3.Distance(start, end) > PacManMap.SINGLE_BLOCK_SIZE * 8)
            {
                return LevelGamePackMan.Instance.Character.RoadCurrent;
            } 
            else 
            {
                return LevelGamePackMan.Instance.Map.FleeTargets[(int)GhostType.Clyde];
            }
        }
    }
}