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
    void OnLobbyEnterDone();
    void OnLobbyExitDone();
}

public class MonoCharacter : MonoBehaviour
{
    public Animator Anim;

    ICharacterEvent m_Handler;
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
    }

    public void PlayRun()
    {
        Anim.Play("Run");
    }

    public void PlayVictory()
    {
        Anim.Play("Victory");
    }

    public void Born()
    {
        transform.position = Level.Instance.LevelData.CharacterEnterPos;
        LobbyRunEnter();
    }

    enum CharacterStatus
    {
        None = 0,
        LobbyRunEnter,
        LobbyWalk,
        LobbyRunExit,
        Idle,
    }

    private CharacterStatus m_nStatus;
    private Vector3 m_Target;
    private bool m_bRota;
    private Vector3 m_MoveDir;
    private int m_RotaParam;
    private void LobbyRunEnter()
    {
        m_Target = Level.Instance.LevelData.CharacterPos;
        PlayRun();
        m_nStatus = CharacterStatus.LobbyRunEnter;
        m_bRota = true;
        m_MoveDir = m_Target - transform.position;
        m_MoveDir.Normalize();

        Vector3 cross = Vector3.Cross(transform.forward, m_MoveDir);
        m_RotaParam = cross.y < 0 ? -1 : 1; 
    }

    private void LobbyMoveIdle()
    {
        m_Target = Level.Instance.LevelData.CharacterTurnPos;
        PlayWalk();
        m_nStatus = CharacterStatus.LobbyWalk;
        m_bRota = true;
        m_MoveDir = m_Target - transform.position;
        m_MoveDir.Normalize();

        Vector3 cross = Vector3.Cross(transform.forward, m_MoveDir);
        m_RotaParam = cross.y < 0 ? -1 : 1; 
    }

    public void LobbyRunExit()
    {
        m_Target = Level.Instance.LevelData.CharacterExitPos;
        PlayRun();
        m_nStatus = CharacterStatus.LobbyRunExit;
        m_bRota = true;
        m_MoveDir = m_Target - transform.position;
        m_MoveDir.Normalize();

        Vector3 cross = Vector3.Cross(transform.forward, m_MoveDir);
        m_RotaParam = cross.y < 0 ? -1 : 1; 
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
        case CharacterStatus.LobbyRunEnter:
            {
                float dis = GameConfig.Instance.RunSpeed * Time.deltaTime;
                float length = Vector3.Distance(transform.position, m_Target);
                if(dis > length)
                {
                    transform.position = m_Target;
                    LobbyMoveIdle();
                }
                else
                {
                    transform.position += m_MoveDir * dis;
                }
            }
            break;

        case CharacterStatus.LobbyRunExit:
            {
                float dis = GameConfig.Instance.RunSpeed * Time.deltaTime;
                float length = Vector3.Distance(transform.position, m_Target);
                if(dis > length)
                {
                    transform.position = m_Target;
                    m_Handler.OnLobbyExitDone();
                    m_nStatus = CharacterStatus.None;
                }
                else
                {
                    transform.position += m_MoveDir * dis;
                }
            }
            break;

        case CharacterStatus.LobbyWalk:
            {
                float dis = GameConfig.Instance.WalkSpeed * Time.deltaTime;
                float length = Vector3.Distance(transform.position, m_Target);
                if(dis > length)
                {
                    transform.position = m_Target;
                    LobbyIdle();
                    m_Handler.OnLobbyEnterDone();
                }
                else
                {
                    transform.position += m_MoveDir * dis;
                }
            }
            break;


        case CharacterStatus.Idle:
            break;
        }
    }
}