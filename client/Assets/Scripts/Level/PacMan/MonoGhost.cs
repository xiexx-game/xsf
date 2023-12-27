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

    void Awake()
    {
        transform.localPosition = BornPos;
    }

    public void SetViewType(GhostViewType nType)
    {
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
    }
}