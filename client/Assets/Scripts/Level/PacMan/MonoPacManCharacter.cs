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

public class MonoPacManCharacter : MonoBehaviour
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

    public bool Left;
    public bool Right;
    public bool Up;
    public bool Down;
    public bool Stop;

    public void Move(PacManMoveDir nDir)
    {
        int nIndex = (int)nDir;
        Eye.transform.localPosition = EyePos[nIndex];
        Body.transform.localRotation = Quaternion.Euler(0, 0, Rota[nIndex]);

        if(m_nStatus == MouthStatus.Idle)
            m_nStatus = MouthStatus.Open;
    }

    public void Idle()
    {
        Mouth[0].transform.localRotation = Quaternion.Euler(0, 0, -IdleAngel);
        Mouth[1].transform.localRotation = Quaternion.Euler(0, 0, IdleAngel);
        m_nStatus = MouthStatus.Idle;
    }

    void Update()
    {
        if(Up)
        {
            Up = false;
            Move(PacManMoveDir.Up);
        }

        if(Right)
        {
            Right = false;
            Move(PacManMoveDir.Right);
        }

        if(Down)
        {
            Down = false;
            Move(PacManMoveDir.Down);
        }

        if(Left)
        {
            Left = false;
            Move(PacManMoveDir.Left);
        }

        if(Stop)
        {
            Stop = false;
            Idle();
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
}