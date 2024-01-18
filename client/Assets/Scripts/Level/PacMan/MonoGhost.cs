//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Scripts/Level/PacMan/MonoGhost.cs
// 作者：Xoen Xie
// 时间：2023/06/21
// 描述：鬼 脚本
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;


public enum GhostViewType
{
    Up = 0,
    Right,
    Down,
    Left,
    Max,
}

public enum GhostType 
{
    Blinky,
    Pinky,
    Inky,
    Clyde,
    Max,
}

public class MonoGhost : MonoBehaviour
{
    public GameObject [] LeftEye;
    public GameObject [] RightEye;

    public Vector3[] LeftEyePos;
    public Vector3[] RightEyePos;
    public Vector3[] EyeBluePos;

    public float EyeSpeed;

    private GhostViewType m_nCurType = GhostViewType.Max;

    private Vector3[] EyeStart = new Vector3[3];

    private Vector3 [] EyeTarget = new Vector3[3];

    private float t;

    public bool Left;
    public bool Right;
    public bool Up;
    public bool Down;

    public GhostType GType;

    public Vector3 BornPos;

    public float PosZ;

    private PacManMoveDir m_MoveDir;
    public PacManMoveDir MoveDir {
        set {
            m_MoveDir = value;

            switch(m_MoveDir)
            {
            case PacManMoveDir.Up:  SetViewType(GhostViewType.Up); break;
            case PacManMoveDir.Right:  SetViewType(GhostViewType.Right); break;
            case PacManMoveDir.Down:  SetViewType(GhostViewType.Down); break;
            case PacManMoveDir.Left:  SetViewType(GhostViewType.Left); break;
            }
        }

        get
        {
            return m_MoveDir;
        }
    }
    public float MoveSpeed;

    private AI_Ghost m_AI;

    void Awake()
    {
        LevelGamePackMan.Instance.Ghosts[(int)GType] = this;

        transform.localPosition = BornPos;
        InitAI();
    }

    void InitAI()
    {
        switch(GType)
        {
        case GhostType.Blinky: m_AI = new AI_Blinky();  break;
        case GhostType.Pinky: m_AI = new AI_Pinky();    break;
        case GhostType.Inky: m_AI = new AI_Inky();      break;
        case GhostType.Clyde: m_AI = new AI_Clyde();    break;

        default:
            break;
        }
    }

    public void SetViewType(GhostViewType nType)
    {
        if(m_nCurType == nType)
            return;
            
        m_nCurType = nType;

        EyeStart[0] = LeftEye[0].transform.localPosition;
        EyeStart[1] = RightEye[0].transform.localPosition;
        EyeStart[2] = LeftEye[1].transform.localPosition;

        EyeTarget[0] = LeftEyePos[(int)m_nCurType];
        EyeTarget[1]  = RightEyePos[(int)m_nCurType];
        EyeTarget[2]  = EyeBluePos[(int)m_nCurType];

        float Dis = Vector3.Distance(LeftEye[0].transform.localPosition, EyeTarget[0]);
        if(Dis >= 0.001)
        {
            
        } else {
            m_nCurType = GhostViewType.Max;
        }
    }

    void Update()
    {
        if(Up)
        {
            Up = false;
            SetViewType(GhostViewType.Up);
        }

        if(Right)
        {
            Right = false;
            SetViewType(GhostViewType.Right);
        }

        if(Down)
        {
            Down = false;
            SetViewType(GhostViewType.Down);
        }

        if(Left)
        {
            Left = false;
            SetViewType(GhostViewType.Left);
        }

        if(m_nCurType != GhostViewType.Max)
        {
            t += Time.deltaTime * EyeSpeed;
            LeftEye[0].transform.localPosition = Vector3.Lerp(EyeStart[0], EyeTarget[0], t);
            RightEye[0].transform.localPosition = Vector3.Lerp(EyeStart[1], EyeTarget[1], t);

            Vector3 pos = Vector3.Lerp(EyeStart[2], EyeTarget[2], t);
            LeftEye[1].transform.localPosition = pos;
            RightEye[1].transform.localPosition = pos;

            if(t >= 1.0f)
            {
                t = 0;
                m_nCurType = GhostViewType.Max;
            }
        }

        m_AI.OnUpdate();
    }
}