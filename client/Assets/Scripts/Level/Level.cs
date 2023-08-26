//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Scripts/Level/Level.cs
// 作者：Xoen
// 时间：2023/08/25
// 描述：关卡 总管
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;

public class Level : Singleton<Level>, IEventSink
{
    private LevelGame[] m_Games;
    private LevelGame m_Current;
    public void Init()
    {
        m_Games = new LevelGame[(int)LevelGameType.Max];
        m_Games[(int)LevelGameType.Tetris] = new LevelGameTetris();

        for(int i = 0; i < (int)LevelGameType.Max; i++)
        {
            if(m_Games[i] != null)
                m_Games[i].Init();
        }
    }

    public LevelGameType GameType
    {
        set
        {
            m_Current = m_Games[(int)value];
        }
    }

    public void Load()
    {
        XSFEvent.Instance.Subscribe(this, (uint)EventID.LoadingDone, 0);
        m_Current.Load();
    }

    public void Enter()
    {
        m_Current.Enter();
    }

    public void Exit()
    {
        m_Current.Exit();
    }

    public void OnUpdate()
    {
        m_Current.OnUpdate();
    }

    public bool OnEvent(uint nEventID, uint nObjectID, object context)
    {
        if(nEventID == (uint)EventID.LoadingDone)
        {
            XSFGSManager.Instance.mNextStateID = XSFGSID.Play;
            return false;
        }

        return true;
    }

}

