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
using System;

public class Level : Singleton<Level>, IEventSink
{
    private LevelGame[] m_Games;
    public LevelGame Current { get; private set;}

    public bool Pause;

    public uint MaxLife 
    {
        get
        {
            return XSFSchema.Instance.Get<SchemaGlobal>((int)SchemaID.Global).GetUint(GlobalID.MaxLifeCount);
        }
    }
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

        Pause = false;
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
        Pause = false;
        Current.Enter();
    }

    public void Exit()
    {
        Current.Exit();
    }

    public void OnUpdate()
    {
        if(Pause)
            return;

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

    public uint CurrentLifeCount
    {
        get {
            return (uint)PlayerPrefs.GetInt(G.PR_LIFE, (int)MaxLife);
        }

        set {
            DateTime currentTime = DateTime.Now;
            string customFormat = currentTime.ToString("yyyy-MM-dd HH:mm:ss");
            PlayerPrefs.SetString(G.PR_TIME, customFormat);

            PlayerPrefs.SetInt(G.PR_LIFE, (int)value);
        }   
        
    }

    public DateTime LifeTime 
    {
        get
        {
            string dateString =  PlayerPrefs.GetString(G.PR_TIME);
            return DateTime.Parse(dateString);
        }
    }

    public uint GetHighScore(int nGameType)
    {
        string str = $"{G.PR_SCORE}_{nGameType}";
        return (uint)PlayerPrefs.GetInt(str, 0);
    }

    public void SetHighScore(int nGameType, uint nScore)
    {
        string str = $"{G.PR_SCORE}_{nGameType}";
        PlayerPrefs.SetInt(str, (int)nScore);
    }

}

