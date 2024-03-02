//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Scripts/Level/PacMan/AI/AI_Pinky.cs
// 作者：Xoen Xie
// 时间：2023/06/21
// 描述：鬼AI - 小粉
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;

public class AI_Pinky : AI_Ghost
{
    public override BlockType SetType { get { return BlockType.GhostPinky; } }

    public override void OnBorn()
    {
        m_nState = AIState.GoOut2End;
        m_Ghost.ShowPath = true;
    }

    public override PacManMapBlock GetTarget()
    {
        if(LevelGamePackMan.Instance.Character.Speed.HasEnergy)
        {
            Debug.LogError("Energy time");
            return LevelGamePackMan.Instance.Map.FleeTargets[(int)GhostType.Pinky];
        }
        else
        {
            var current = LevelGamePackMan.Instance.Character.RoadCurrent;
            var dir = LevelGamePackMan.Instance.Character.MoveDir;

            int nCount = 0;
            while(nCount < 4)
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
                            dir = (PacManMoveDir)nextDir;
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
}