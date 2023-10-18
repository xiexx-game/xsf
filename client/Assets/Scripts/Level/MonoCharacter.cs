//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Scripts/Level/MonoCharacter.cs
// 作者：Xoen
// 时间：2023/08/25
// 描述：角色脚本
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;

public interface ICharacterEvent
{
    void OnEnterDone();
    void OnExitDone();
}

public class MonoCharacter : MonoBehaviour
{
    public Animator Anim;

    ICharacterEvent m_Handler;

    int m_nTargetIndex;
    Vector3[] m_Targets;

    public void Init(ICharacterEvent handler)
    {
        m_Handler = handler;
    }

    public void PlayIdle()
    {
        Anim.Play("Idle");
    }

    public void PlayWalk()
    {
        Anim.Play("Walk");
        m_fSpeed = GameConfig.Instance.WalkSpeed;
    }

    public void PlayRun()
    {
        Anim.Play("Run");
        m_fSpeed = GameConfig.Instance.RunSpeed;
    }

    public void PlayVictory()
    {
        Anim.Play("Victory");
    }

    public void Born(Vector3 pos, Vector3[] targets)
    {
        transform.position = pos;
        m_nTargetIndex = 0;
        m_Targets = targets;
        m_nStatus = CharacterStatus.StartEnter;
    }

    enum CharacterStatus
    {
        None = 0,
        StartEnter,
        Enter,
        Exit,
        PlayExit,
        PlayFastExit,
        Idle,
    }

    private CharacterStatus m_nStatus;
    private Vector3 m_Target;
    private bool m_bRota;
    private Vector3 m_MoveDir;
    private int m_RotaParam;

    private float m_fSpeed;

    private void Enter()
    {
        m_Target = m_Targets[m_nTargetIndex++];
        m_bRota = true;
        m_MoveDir = m_Target - transform.position;
        m_MoveDir.Normalize();

        Vector3 cross = Vector3.Cross(transform.forward, m_MoveDir);
        m_RotaParam = cross.y < 0 ? -1 : 1; 
    }

    public void Exit(Vector3 target)
    {
        m_Target = target;
        PlayRun();
        m_nStatus = CharacterStatus.Exit;
        m_bRota = true;
        m_MoveDir = m_Target - transform.position;
        m_MoveDir.Normalize();

        Vector3 cross = Vector3.Cross(transform.forward, m_MoveDir);
        m_RotaParam = cross.y < 0 ? -1 : 1; 
    }

    public void PlayExit(Vector3 target)
    {
        m_Target = target;
        PlayWalk();
        m_nStatus = CharacterStatus.Exit;
        m_bRota = true;
        m_MoveDir = m_Target - transform.position;
        m_MoveDir.Normalize();

        Vector3 cross = Vector3.Cross(transform.forward, m_MoveDir);
        m_RotaParam = cross.y < 0 ? -1 : 1; 
    }

    public void PlayFastExit()
    {
        PlayRun();
    }

    private void LobbyIdle()
    {
        PlayIdle();
        m_nStatus = CharacterStatus.Idle;
    }

    void Update()
    {
        if(m_bRota)
        {
            float rota = GameConfig.Instance.CharacterRotaSpeed * Time.deltaTime;
            //Debug.Log($"rota={rota}");

            var current = transform.forward;
            float a = Vector3.Angle(current, m_MoveDir);
            if(rota > a)
            {
                transform.rotation.SetLookRotation(m_MoveDir);
                m_bRota = false;
            }
            else
            {
                transform.Rotate(Vector3.up, m_RotaParam * rota);
            }
        }

        switch(m_nStatus)
        {
        case CharacterStatus.StartEnter:
            PlayRun();
            Enter();
            m_nStatus = CharacterStatus.Enter;
            break;

        case CharacterStatus.Enter:
            {
                float dis = m_fSpeed * Time.deltaTime;
                float length = Vector3.Distance(transform.position, m_Target);
                if(dis > length)
                {
                    transform.position = m_Target;
                    if(m_nTargetIndex >= m_Targets.Length)
                    {
                        m_Handler.OnEnterDone();
                        m_nStatus = CharacterStatus.Idle;
                    }
                    else
                    {
                        Enter();
                    }
                }
                else
                {
                    transform.position += m_MoveDir * dis;
                }
            }
            break;

        case CharacterStatus.Idle:
            PlayIdle();
            m_nStatus = CharacterStatus.None;
            break;

        case CharacterStatus.Exit:
            {
                float dis = m_fSpeed * Time.deltaTime;
                float length = Vector3.Distance(transform.position, m_Target);
                if(dis > length)
                {
                    transform.position = m_Target;
                    m_Handler.OnExitDone();
                    m_nStatus = CharacterStatus.Idle;
                }
                else
                {
                    transform.position += m_MoveDir * dis;
                }
            }
            break;
        }
    }
}