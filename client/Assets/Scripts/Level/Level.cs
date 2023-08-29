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
    public LevelGame Current { get; private set;}
    public void Init()
    {
        m_Games = new LevelGame[(int)LevelGameType.Max];
        m_Games[(int)LevelGameType.Tetris] = new LevelGameTetris();
        m_Games[(int)LevelGameType.Snake] = new LevelGameTetris();
        m_Games[(int)LevelGameType.PacMan] = new LevelGameTetris();

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
            Current = m_Games[(int)value];
            Current.GameSocre = 0;
        }
    }

    public void Load()
    {
        XSFEvent.Instance.Subscribe(this, (uint)EventID.LoadingDone, 0);
        Current.Load();
    }

    public void Enter()
    {
        Current.Enter();
    }

    public void Exit()
    {
        Current.Exit();
    }

    public void OnUpdate()
    {
        Current.OnUpdate();
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

