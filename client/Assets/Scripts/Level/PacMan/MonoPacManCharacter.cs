//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Scripts/Level/PacMan/MonoPacManCharacter.cs
// 作者：Xoen Xie
// 时间：2023/06/21
// 描述：鬼 脚本
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;


public enum PacManMoveDir
{
    Up = 0,
    Right,
    Down,
    Left,
    Max,
}

public class MonoPacManCharacter : MonoBehaviour, ISMHandler
{
    public GameObject Eye;

    public Vector3[] EyePos;
    public float [] Rota;

    public GameObject Body;
    public GameObject[] Mouth;

    public float IdleAngel;
    public float OpenAngel;
    public float OpenSpeed;

    enum MouthStatus
    {
        Idle = 0,
        Open,
        Close,
    }

    MouthStatus m_nStatus;

    public Vector3 BornPos;
    public float PosZ;

    public PacManMoveDir MoveDir;

    public PacManMapBlock Current;

    private bool WaitBorn;

    private bool IsMoving;

    public Vector3 MoveTarget;

    public bool RotaSmooth;
    public float RotaSpeed;

    private bool IsRota;
    private Quaternion rotaStart;
    private Quaternion rotaEnd;
    private float rotaT;

    public SpeedManager Speed;

    void Awake()
    {
        WaitBorn = true;
        transform.localPosition = BornPos;
        Body.transform.localRotation = Quaternion.Euler(0, 0, Rota[1]);
        LevelGamePackMan.Instance.Character = this;
        Idle();
        MoveDir = PacManMoveDir.Right;

        Speed = new SpeedManager();
        Speed.Init(this);
    }

    public void Move(PacManMoveDir nDir, Vector3 target)
    {
        MoveTarget = target;
        MoveDir = nDir;
        int nIndex = (int)nDir;
        Eye.transform.localPosition = EyePos[nIndex];

        if(RotaSmooth)
        {
            IsRota = true;
            rotaStart = Body.transform.localRotation;
            rotaEnd = Quaternion.Euler(0, 0, Rota[nIndex]);
            rotaT = 0;
        }
        else
        {
            Body.transform.localRotation = Quaternion.Euler(0, 0, Rota[nIndex]);
        }

        if(m_nStatus == MouthStatus.Idle)
            m_nStatus = MouthStatus.Open;

        IsMoving = true;
    }

    public void Idle()
    {
        Mouth[0].transform.localRotation = Quaternion.Euler(0, 0, -IdleAngel);
        Mouth[1].transform.localRotation = Quaternion.Euler(0, 0, IdleAngel);
        m_nStatus = MouthStatus.Idle;

        IsMoving = false;
    }

    void Update()
    {
        Speed.Update();

        var map = LevelGamePackMan.Instance.Map;
        if(WaitBorn && map.IsReady)
        {
            WaitBorn = false;

            Current = map.Pos2Block(transform.localPosition);
            map.OnPacManEnterBlock(Current);
            Debug.LogWarning("MonoPacManCharacter Current=" + Current);
        }

        if(IsRota)
        {
            rotaT += Time.deltaTime * RotaSpeed;
            if(rotaT >= 1.0f)
            {
                rotaT = 1.0f;
                IsRota = false;
            }

            Body.transform.localRotation = Quaternion.Slerp(rotaStart, rotaEnd, rotaT);
        }

        if(IsMoving)
        {
            var start = transform.localPosition; start.z = 0;
            var end = MoveTarget; end.z = 0;
            var dir = (end - start).normalized;
            float dis = Vector3.Distance(start, end);
            float disMove = Speed.CurrentSpeed * Time.deltaTime;

            if(dis > disMove)
            {
                Vector3 pos = start + disMove * dir;
                pos.z = PosZ;
                transform.localPosition = pos;

                var block = map.Pos2Block(transform.localPosition);
                if(block != Current)
                {
                    map.OnPacManExitBlock(Current);
                    Current = block;
                    map.OnPacManEnterBlock(Current);
                }
            }
            else
            {
                end.z = PosZ;
                transform.localPosition = end;
                
                var nextIndex = Current.ConnectIndex[(int)MoveDir];
                if(nextIndex > 0)
                {
                    if(Current.Index == map.TunnelRightIndex && nextIndex == map.TunnelLeftIndex)
                    {
                        if(end.x < map.TunnelRight)
                        {
                            MoveTarget = end;
                            MoveTarget.x = map.TunnelRight;
                        }
                        else
                        {
                            var tunnelLeft = map.GetBlockByIndex(map.TunnelLeftIndex);
                            MoveTarget = tunnelLeft.pos;
                            
                            end.x = map.TunnelLeft;
                            transform.localPosition = end;

                            map.OnPacManExitBlock(Current);
                            Current = map.Pos2Block(transform.localPosition);
                            map.OnPacManEnterBlock(Current);
                        }
                    }
                    else if(Current.Index == map.TunnelLeftIndex && nextIndex == map.TunnelRightIndex)
                    {
                        if(end.x > map.TunnelLeft)
                        {
                            MoveTarget = end;
                            MoveTarget.x = map.TunnelLeft;
                        }
                        else
                        {
                            var TunnelRight = map.GetBlockByIndex(map.TunnelRightIndex);
                            MoveTarget = TunnelRight.pos;

                            end.x = map.TunnelRight;
                            transform.localPosition = end;

                            map.OnPacManExitBlock(Current);
                            Current = map.Pos2Block(transform.localPosition);
                            map.OnPacManEnterBlock(Current);
                        }
                    }
                    else
                    {
                        var target = map.GetBlockByIndex(nextIndex);
                        MoveTarget = target.pos;
                    }
                }
                else 
                {
                    Idle();
                }
            }
        }


        switch(m_nStatus)
        {
        case MouthStatus.Open:
            {
                var angel = Time.deltaTime * OpenSpeed;
                var UpAngel = Mouth[1].transform.localRotation.eulerAngles.z;
                UpAngel += angel;
                if(UpAngel >= OpenAngel) {
                    UpAngel = OpenAngel;
                    m_nStatus = MouthStatus.Close;
                }

                Mouth[0].transform.localRotation = Quaternion.Euler(0, 0, -UpAngel);
                Mouth[1].transform.localRotation = Quaternion.Euler(0, 0, UpAngel);
            }
            break;

        case MouthStatus.Close:
            {
                var angel = Time.deltaTime * OpenSpeed;
                var UpAngel = Mouth[1].transform.localRotation.eulerAngles.z;
                UpAngel -= angel;
                if(UpAngel <= 0.0f) {
                    UpAngel = 0.0f;
                    m_nStatus = MouthStatus.Open;
                }

                Mouth[0].transform.localRotation = Quaternion.Euler(0, 0, -UpAngel);
                Mouth[1].transform.localRotation = Quaternion.Euler(0, 0, UpAngel);
            }
            break;
        }
    }

    public bool IsGhost { get { return false; } }

    public void OnEnergyEnd()
    {
        
    }
}