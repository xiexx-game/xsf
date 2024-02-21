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
    WaitReady,
    Born,
    MoveInPath,
    MoveForward,
    Think,
    IdleRight,
    IdleLeft,
    GoOut2Start,
    GoOut2End,
    GoOutDone,
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

    protected readonly Vector3 OutStart = new Vector3(0.0199999996f,0.379999995f,-0.400000006f);
    protected readonly Vector3 OutEnd = new Vector3(0.0199999996f,1.49000001f,-0.400000006f);

    protected readonly Vector3 RoomRight = new Vector3(0.75999999f,0.379999995f,-0.400000006f);
    protected readonly Vector3 RoomLeft = new Vector3(-0.74000001f,0.379999995f,-0.400000006f);

    public void Init(MonoGhost ghost)
    {
        m_nState = AIState.WaitReady;
        m_Ghost = ghost;
        m_path = new List<PacManMapBlock>();
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
                Debug.Log($"DoMove m_Ghost.MoveDir={m_Ghost.MoveDir}, DirReverse={DirReverse}");
                for(int i = 0; i < m_Current.ConnectIndex.Length; i++)
                {
                    if((int)DirReverse != i && m_Current.ConnectIndex[i] > 0)
                    {
                        m_Ghost.MoveDir = (PacManMoveDir)i;
                        m_Target = LevelGamePackMan.Instance.Map.GetBlockByIndex(m_Current.ConnectIndex[i]);
                        Debug.Log($"DoMove m_Target={m_Target}, m_Ghost.MoveDir={m_Ghost.MoveDir}");
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

    private void Move2Pos(Vector3 target, AIState endState)
    {
        var start = m_Ghost.transform.localPosition; start.z = 0;
        var end = target; end.z = 0;
        var dir = (end - start).normalized;
        float dis = Vector3.Distance(start, end);
        float disMove = m_Ghost.MoveSpeed * Time.deltaTime;

        if(dis > disMove)
        {
            Vector3 pos = start + disMove * dir;
            pos.z = m_Ghost.PosZ;
            m_Ghost.transform.localPosition = pos;
        }
        else
        {
            end.z = m_Ghost.PosZ;
            m_Ghost.transform.localPosition = end;
            m_nState = endState;
        }
    }

    public void OnUpdate()
    {
        //Debug.Log("======== AI_Ghost m_nState=" + m_nState + ", m_Ghost GType=" + m_Ghost.GType);
        switch(m_nState)
        {
        case AIState.WaitReady:
            {
                if(LevelGamePackMan.Instance.Map.IsReady)
                {
                    m_nState = AIState.Born;
                }
            }
            break;

        case AIState.Born:
            {
                m_Current = LevelGamePackMan.Instance.Map.Pos2Block(m_Ghost.transform.localPosition);
                Debug.Log("AIState.Born =" + m_Current.Index);
                m_Ghost.transform.localPosition = new Vector3(m_Current.pos.x, m_Current.pos.y, m_Ghost.PosZ);
                OnBorn();
            }
            break;

        case AIState.IdleRight:  
            {
                m_Ghost.MoveDir = PacManMoveDir.Right;
                Move2Pos(RoomRight, AIState.IdleLeft);
            }
            break;

        case AIState.IdleLeft:
            {
                m_Ghost.MoveDir = PacManMoveDir.Left;
                Move2Pos(RoomLeft, AIState.IdleRight);
            }
            break;

        case AIState.GoOut2Start:
            {

            }
            break;

        case AIState.GoOut2End:
            {
                m_Ghost.MoveDir = PacManMoveDir.Up;
                Move2Pos(OutEnd, AIState.GoOutDone);
            }
            break;

        case AIState.GoOutDone:
            {
                m_Current = LevelGamePackMan.Instance.Map.Pos2Block(m_Ghost.transform.localPosition);
                m_Ghost.MoveDir = PacManMoveDir.Left;
                DoMove(true); 
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
                            Debug.Log("MoveInPath end ...");
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
                    Debug.Log($"Move Forward end ..., m_Current={m_Current}");
                    DoMove(true);
                }
            }
            break;

        case AIState.Think:
            {
                var target = GetTarget();
                m_path.Clear();

                var map = LevelGamePackMan.Instance.Map;
                var dir = map.DirReverse(m_Ghost.MoveDir);
                var path = LevelGamePackMan.Instance.Map.FindPath(dir, m_Current, target, m_Ghost.ShowPath);
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
