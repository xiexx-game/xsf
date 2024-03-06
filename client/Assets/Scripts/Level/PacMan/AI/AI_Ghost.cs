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
    Move,
    DieMove,
    Think,
    IdleRight,
    IdleLeft,
    GoOut2Start,
    GoOut2End,
    GoOutDone,
    GoIn,
    ReBorn,
    Max,
}

public abstract class AI_Ghost
{
    protected AIState m_nState;

    protected MonoGhost m_Ghost;

    public PacManMapBlock m_Current;
    
    protected List<PacManPathResult> m_path;
    protected int m_nPathIndex;

    protected readonly Vector3 OutStart = new Vector3(0.0199999996f,0.379999995f,-0.400000006f);
    protected readonly Vector3 OutEnd = new Vector3(0.0199999996f,1.49000001f,-0.400000006f);

    protected readonly Vector3 RoomRight = new Vector3(0.75999999f,0.379999995f,-0.400000006f);
    protected readonly Vector3 RoomLeft = new Vector3(-0.74000001f,0.379999995f,-0.400000006f);

    public abstract BlockType SetType { get; }

    public void Init(MonoGhost ghost)
    {
        m_nState = AIState.Born;
        m_Ghost = ghost;
        m_path = new List<PacManPathResult>();
    }

    public abstract void OnBorn();
    public abstract PacManMapBlock GetTarget();

    public virtual void OnAIUpdate() {}

    private bool IsDie;

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
                var map = LevelGamePackMan.Instance.Map;
                var DirReverse = map.DirReverse(m_Ghost.MoveDir);

                m_path.Clear();
                m_nPathIndex = 0;
                
                for(int i = 0; i < m_Current.ConnectIndex.Length; i++)
                {
                    if((int)DirReverse != i && m_Current.ConnectIndex[i] > 0)
                    {
                        m_Ghost.MoveDir = (PacManMoveDir)i;

                        var nextBlock = map.GetBlockByIndex(m_Current.ConnectIndex[i]);
                        if(m_Current.Index == map.TunnelLeftIndex && nextBlock.Index == map.TunnelRightIndex)
                        {
                            PacManPathResult teleStart;
                            teleStart.pos = m_Current.pos;
                            teleStart.pos.x = map.TunnelLeft;
                            teleStart.Teleport = true;
                            teleStart.block = m_Current;
                            m_path.Add(teleStart);

                            PacManPathResult teleEnd;
                            teleEnd.pos = nextBlock.pos;
                            teleEnd.pos.x = map.TunnelRight;
                            teleEnd.block = nextBlock;
                            teleEnd.Teleport = false;
                            m_path.Add(teleEnd);

                            PacManPathResult teleEnd2;
                            teleEnd2.pos = nextBlock.pos;
                            teleEnd2.block = nextBlock;
                            teleEnd2.Teleport = false;
                            m_path.Add(teleEnd2);
                        }
                        else if(m_Current.Index == map.TunnelRightIndex && nextBlock.Index == map.TunnelLeftIndex)
                        {
                            PacManPathResult teleStart;
                            teleStart.pos = m_Current.pos;
                            teleStart.pos.x = map.TunnelRight;
                            teleStart.Teleport = true;
                            teleStart.block = m_Current;
                            m_path.Add(teleStart);

                            PacManPathResult teleEnd;
                            teleEnd.pos = nextBlock.pos;
                            teleEnd.pos.x = map.TunnelLeft;
                            teleEnd.block = nextBlock;
                            teleEnd.Teleport = false;
                            m_path.Add(teleEnd);

                            PacManPathResult teleEnd2;
                            teleEnd2.pos = nextBlock.pos;
                            teleEnd2.block = nextBlock;
                            teleEnd2.Teleport = false;
                            m_path.Add(teleEnd2);
                        }
                        else
                        {
                            var block = map.GetBlockByIndex(m_Current.ConnectIndex[i]);
                            PacManPathResult teleEnd;
                            teleEnd.pos = block.pos;
                            teleEnd.block = block;
                            teleEnd.Teleport = false;
                            m_path.Add(teleEnd);
                        }

                        break;
                    }
                }

                m_nState = AIState.Move;
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
        float disMove = m_Ghost.Speed.CurrentSpeed * Time.deltaTime;

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

    public bool DoThink()
    {
        switch(m_nState)
        {
        case AIState.Move:
            m_nState = AIState.Think;
            return true;
        }

        return false;
    }

    public void OnUpdate()
    {
        OnAIUpdate();

        //Debug.Log("======== AI_Ghost m_nState=" + m_nState + ", m_Ghost GType=" + m_Ghost.GType);
        switch(m_nState)
        {
        case AIState.Born:
            {
                m_Ghost.OnBorn();
                m_Current = LevelGamePackMan.Instance.Map.Pos2Block(m_Ghost.transform.localPosition);
                LevelGamePackMan.Instance.Map.OnGhostEnterBlock(m_Ghost, m_Current);

                //Debug.Log("AIState.Born =" + m_Current.Index);
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
                var pos = m_Ghost.transform.localPosition;
                if(OutStart.x <= pos.x)
                {
                    m_Ghost.MoveDir = PacManMoveDir.Left;
                }
                else
                {
                    m_Ghost.MoveDir = PacManMoveDir.Right;
                }

                Move2Pos(OutStart, AIState.GoOut2End);
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

        case AIState.Move:
            {
                var start = m_Ghost.transform.localPosition; start.z = 0;
                var end = m_path[m_nPathIndex].pos; end.z = 0;
                var dir = (end - start).normalized;
                float dis = Vector3.Distance(start, end);

                float disMove = m_Ghost.Speed.CurrentSpeed * Time.deltaTime;

                if(dis > disMove)
                {
                    Vector3 pos = start + disMove * dir;
                    pos.z = m_Ghost.PosZ;
                    m_Ghost.transform.localPosition = pos;

                    var block = LevelGamePackMan.Instance.Map.Pos2Block(m_Ghost.transform.localPosition);
                    if(block != m_Current)
                    {
                        LevelGamePackMan.Instance.Map.OnGhostExitBlock(m_Ghost, m_Current);
                        m_Current = block;
                        LevelGamePackMan.Instance.Map.OnGhostEnterBlock(m_Ghost, m_Current);
                    }
                }
                else 
                {
                    if(m_path[m_nPathIndex].Teleport)
                    {
                        m_nPathIndex ++;
                        end = m_path[m_nPathIndex].pos;
                        end.z = m_Ghost.PosZ;
                        m_Ghost.transform.localPosition = end;
                        LevelGamePackMan.Instance.Map.OnGhostExitBlock(m_Ghost, m_Current);
                        m_Current = LevelGamePackMan.Instance.Map.Pos2Block(m_Ghost.transform.localPosition);
                        LevelGamePackMan.Instance.Map.OnGhostEnterBlock(m_Ghost, m_Current);

                        m_nPathIndex ++;
                        return;
                    }
                    else
                    {
                        end.z = m_Ghost.PosZ;
                        m_Ghost.transform.localPosition = end;
                    }

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
                                if(m_path[m_nPathIndex].block.Index == m_Current.ConnectIndex[i])
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

        case AIState.GoIn:
            {
                m_Ghost.MoveDir = PacManMoveDir.Down;
                Move2Pos(OutStart, AIState.ReBorn);
            }
            break;

        case AIState.ReBorn:
            {
                m_Ghost.ReBorn();
                m_nState = AIState.GoOut2End;
            }
            break;

        case AIState.DieMove:
            {
                var start = m_Ghost.transform.localPosition; start.z = 0;
                var end = m_path[m_nPathIndex].pos; end.z = 0;
                var end2 = OutEnd; end2.z = 0;
                var dir = (end - start).normalized;
                float dis = Vector3.Distance(start, end);
                float dis2 = Vector3.Distance(start, end2);

                float disMove = m_Ghost.Speed.CurrentSpeed * Time.deltaTime;
                if(disMove > dis2)
                {
                    m_nState = AIState.GoIn;
                    end2.z = m_Ghost.PosZ;
                    m_Ghost.transform.localPosition = end2;
                    return;
                }

                if(dis > disMove)
                {
                    Vector3 pos = start + disMove * dir;
                    pos.z = m_Ghost.PosZ;
                    m_Ghost.transform.localPosition = pos;
                }
                else 
                {
                    if(m_path[m_nPathIndex].Teleport)
                    {
                        m_nPathIndex ++;
                        end = m_path[m_nPathIndex].pos;
                        end.z = m_Ghost.PosZ;
                        m_Ghost.transform.localPosition = end;

                        m_nPathIndex ++;
                        return;
                    }
                    else
                    {
                        end.z = m_Ghost.PosZ;
                        m_Ghost.transform.localPosition = end;
                    }


                    m_nPathIndex ++;
                    if(m_nPathIndex >= m_path.Count)
                    {
                        m_nState = AIState.GoIn;
                        end2.z = m_Ghost.PosZ;
                        m_Ghost.transform.localPosition = end2;
                        return;
                    }
                    else
                    {
                        for(int i = 0; i < m_Current.ConnectIndex.Length; i ++)
                        {
                            if(m_path[m_nPathIndex].block.Index == m_Current.ConnectIndex[i])
                            {
                                m_Ghost.MoveDir = (PacManMoveDir)i;
                                break;
                            }
                        }
                    }
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
                    if(m_path[1].block.Index == m_Current.ConnectIndex[i])
                    {
                        m_Ghost.MoveDir = (PacManMoveDir)i;
                        break;
                    }
                }

                m_nState = AIState.Move;
            }
            break;
        }
    }

    public void OnDie()
    {
        IsDie = true;
        var map = LevelGamePackMan.Instance.Map;
        var dir = map.DirReverse(m_Ghost.MoveDir);

        var block = LevelGamePackMan.Instance.Map.Pos2Block(OutEnd);

        m_path.Clear();
        var path = LevelGamePackMan.Instance.Map.FindPath(dir, m_Current, block, m_Ghost.ShowPath);
        m_path.AddRange(path);
        m_nPathIndex = 1;

        for(int i = 0; i < m_Current.ConnectIndex.Length; i ++)
        {
            if(m_path[1].block.Index == m_Current.ConnectIndex[i])
            {
                m_Ghost.MoveDir = (PacManMoveDir)i;
                break;
            }
        }

        m_nState = AIState.DieMove;
    }
}
