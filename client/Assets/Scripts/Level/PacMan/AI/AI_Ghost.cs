//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Scripts/Level/PacMan/AI/AI_Ghost.cs
// 作者：Xoen Xie
// 时间：2023/06/21
// 描述：鬼AI
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections.Generic;

public enum AIState
{
    None = 0,
    Born,
    MoveInPath,
    MoveForward,
    Think,
    Max,
}

public abstract class AI_Ghost
{
    protected AIState m_nState;

    protected MonoGhost m_Ghost;

    protected PacManMapBlock m_Current;
    protected PacManMapBlock m_Target;
    protected List<PacManMapBlock> m_path;
    protected int m_nPathIndex;

    public void Init(MonoGhost ghost)
    {
        m_nState = AIState.Born;
        m_Ghost = ghost;
    }

    public abstract void OnBorn();
    public abstract PacManMapBlock GetTarget();

    public void DoMove(bool isForward)
    {
        if(isForward)
        {
            if(m_Current.ConnectCount >= 3)
            {
                m_nState = AIState.Think;
            }
            else
            {
                var DirReverse = LevelGamePackMan.Instance.Map.DirReverse(m_Ghost.MoveDir);
                for(int i = 0; i < m_Current.ConnectIndex.Length; i++)
                {
                    if((int)DirReverse != i && m_Current.ConnectIndex[i] > 0)
                    {
                        m_Ghost.MoveDir = (PacManMoveDir)i;
                        m_Target = LevelGamePackMan.Instance.Map.GetBlockByIndex(m_Current.ConnectIndex[i]);
                        break;
                    }
                }

                m_nState = AIState.MoveForward;
            }
        }
        else 
        {
            m_nState = AIState.Think;
        }
    }

    public void OnUpdate()
    {
        switch(m_nState)
        {
        case AIState.Born:
            {
                m_Current = LevelGamePackMan.Instance.Map.Pos2Block(m_Ghost.transform.localPosition);
                m_Ghost.transform.localPosition = new Vector3(m_Current.pos.x, m_Current.pos.y, m_Ghost.PosZ);
                OnBorn();
            }
            break;

        case AIState.MoveInPath:
            {
                var start = m_Ghost.transform.localPosition; start.z = 0;
                var end = m_path[m_nPathIndex].pos; end.z = 0;
                var dir = (end - start).normalized;
                float dis = Vector3.Distance(start, end);

                float disMove = m_Ghost.MoveSpeed * Time.deltaTime;

                if(dis > disMove)
                {
                    Vector3 pos = start + disMove * dir;
                    pos.z = m_Ghost.PosZ;
                    m_Ghost.transform.localPosition = pos;

                    var block = LevelGamePackMan.Instance.Map.Pos2Block(m_Ghost.transform.localPosition);
                    if(block != m_Current)
                    {
                        m_Current = block;
                    }
                }
                else 
                {
                    end.z = m_Ghost.PosZ;
                    m_Ghost.transform.localPosition = end;

                    if(m_Current.ConnectCount >= 3)
                    {
                        m_nState = AIState.Think;
                    }
                    else
                    {
                        m_nPathIndex ++;
                        if(m_nPathIndex >= m_path.Count)
                        {
                            DoMove(true);
                        }
                        else
                        {
                            for(int i = 0; i < m_Current.ConnectIndex.Length; i ++)
                            {
                                if(m_path[m_nPathIndex].Index == m_Current.ConnectIndex[i])
                                {
                                    m_Ghost.MoveDir = (PacManMoveDir)i;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            break;

        case AIState.MoveForward:
            {
                var start = m_Ghost.transform.localPosition; start.z = 0;
                var end = m_Target.pos; end.z = 0;
                var dir = (end - start).normalized;
                float dis = Vector3.Distance(start, end);

                float disMove = m_Ghost.MoveSpeed * Time.deltaTime;

                if(dis > disMove)
                {
                    Vector3 pos = start + disMove * dir;
                    pos.z = m_Ghost.PosZ; 
                    m_Ghost.transform.localPosition = pos;

                    var block = LevelGamePackMan.Instance.Map.Pos2Block(m_Ghost.transform.localPosition);
                    if(block != m_Current)
                    {
                        m_Current = block;
                    }
                }
                else 
                {
                    end.z = m_Ghost.PosZ;
                    m_Ghost.transform.localPosition = end;

                    DoMove(true);
                }
            }
            break;

        case AIState.Think:
            {
                var target = GetTarget();
                m_path.Clear();

                var map = LevelGamePackMan.Instance.Map;
                var dir = map.DirReverse(LevelGamePackMan.Instance.Character.MoveDir);
                var path = LevelGamePackMan.Instance.Map.FindPath(dir, m_Current, target);
                m_path.AddRange(path);
                m_nPathIndex = 1;

                for(int i = 0; i < m_Current.ConnectIndex.Length; i ++)
                {
                    if(m_path[1].Index == m_Current.ConnectIndex[i])
                    {
                        m_Ghost.MoveDir = (PacManMoveDir)i;
                        break;
                    }
                }

                m_nState = AIState.MoveInPath;
            }
            break;
        }
    }
}
