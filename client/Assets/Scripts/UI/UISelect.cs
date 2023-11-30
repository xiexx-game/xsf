//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\Scripts\UI\UISelect.cs
// 作者：Xoen Xie
// 时间：8/25/2023
// 描述：
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public sealed class UISelect : UIBase
{
// UI_PROP_START
	public Image Heart1 { get; private set; }	// 
	public Image Heart2 { get; private set; }	// 
	public Image Heart3 { get; private set; }	// 
	public Image Heart4 { get; private set; }	// 
	public Image Heart5 { get; private set; }	// 
	public GameObject Play { get; private set; }	// 
	public TextMeshProUGUI name { get; private set; }	// 
	public GameObject Blocks { get; private set; }	// 
	public GameObject pacman { get; private set; }	// 
	public GameObject snake { get; private set; }	// 
	public TextMeshProUGUI HighScore { get; private set; }	// 最高分
	public GameObject Right { get; private set; }	// 
	public GameObject Left { get; private set; }	// 
	public TextMeshProUGUI LevelValue { get; private set; }	// 
	public GameObject LevelRight { get; private set; }	// 
	public GameObject LevelLeft { get; private set; }	// 
	public GameObject darkBg { get; private set; }	// 
	public GameObject DarkBg { get; private set; }	// 
// UI_PROP_END

    public override string Name { get { return "UISelect"; } }

    public override uint EventObjID { get { return (uint)EventObjectID.UISelectShow; } }

    private GameObject[] Contents;

    private LevelGameType m_Current;
    private ScpLevelGame m_GameScp;

    private uint m_nCurrentLevel;

    private Image[] m_Hearts;

    private Color32 LifeColor = new Color32(255, 96, 96, 255);
    private Color32 WhiteColor = new Color32(255, 255, 255, 255);

    public override void OnInit()
    {
        // UI_INIT_START
		// 
		Heart1 = RootT.Find("Life/1").GetComponent<Image>();
		// 
		Heart2 = RootT.Find("Life/2").GetComponent<Image>();
		// 
		Heart3 = RootT.Find("Life/3").GetComponent<Image>();
		// 
		Heart4 = RootT.Find("Life/4").GetComponent<Image>();
		// 
		Heart5 = RootT.Find("Life/5").GetComponent<Image>();
		// 
		Play = RootT.Find("play").gameObject;
		UIEventClick.Set(Play, OnPlayClick);
		// 
		name = RootT.Find("item/titel/name").GetComponent<TextMeshProUGUI>();
		// 
		Blocks = RootT.Find("item/content/blocks").gameObject;
		// 
		pacman = RootT.Find("item/content/pacman").gameObject;
		// 
		snake = RootT.Find("item/content/snake").gameObject;
		// 最高分
		HighScore = RootT.Find("item/HighScore").GetComponent<TextMeshProUGUI>();
		// 
		Right = RootT.Find("right").gameObject;
		UIEventClick.Set(Right, OnRightClick);
		// 
		Left = RootT.Find("left").gameObject;
		UIEventClick.Set(Left, OnLeftClick);
		// 
		LevelValue = RootT.Find("LevelValue").GetComponent<TextMeshProUGUI>();
		// 
		LevelRight = RootT.Find("LevelRight").gameObject;
		UIEventClick.Set(LevelRight, OnLevelRightClick);
		// 
		LevelLeft = RootT.Find("LevelLeft").gameObject;
		UIEventClick.Set(LevelLeft, OnLevelLeftClick);
		// 
		darkBg = RootT.Find("dark-bg").gameObject;
		// 
		DarkBg = RootT.Find("dark-bg").gameObject;
		UIEventClick.Set(DarkBg, OnDarkBgClick);
        // UI_INIT_END

        Contents = new GameObject[(int)LevelGameType.Max];
        Contents[(int)LevelGameType.Tetris] = Blocks;
        Contents[(int)LevelGameType.Snake] = snake;
        Contents[(int)LevelGameType.PacMan] = pacman;

        m_Hearts = new Image[Level.Instance.MaxLife];
        m_Hearts[0] = Heart1;
        m_Hearts[1] = Heart2;
        m_Hearts[2] = Heart3;
        m_Hearts[3] = Heart4;
        m_Hearts[4] = Heart5;
    }

    public override void OnClose()
    {
        Play = null;
        name = null;
        Blocks = null;
        snake = null;
        pacman = null;
        Left = null;
        Right = null;
        Contents = null;
    }

    public override void OnShow()
    {
        //Debug.LogError("OnShow = " + m_Current);
        m_Current = LevelGameType.Tetris;
        Level.Instance.GameType = m_Current;
        UpdateSelect();

        m_nCurrentLevel = 1;
        LevelValue.text = m_nCurrentLevel.ToString();

        UpdateLife();

        AudioManager.Instance.PlayBGM(BGMID.Main);
    }

    private void UpdateLife()
    {
        uint nMax = Level.Instance.MaxLife;
        uint nCurrent = Level.Instance.CurrentLifeCount;

        if(nCurrent == 0)
        {
            DateTime time = Level.Instance.LifeTime;
            DateTime now = DateTime.Now;
            TimeSpan span = now - time;
            if(span.Hours >= 12)
            {
                Level.Instance.CurrentLifeCount = Level.Instance.MaxLife;
                nCurrent = Level.Instance.MaxLife;
            }
            else
            {
                darkBg.SetActive(true);
            }
        }

        for(uint i = nMax; i > 0; i --)
        {
            if(i > nCurrent)
            {
                m_Hearts[i-1].color = WhiteColor;
            }
            else
            {
                m_Hearts[i-1].color = LifeColor;
            }
        }
    }

    private void UpdateSelect()
    {
        //Debug.LogError("UpdateSelect = " + m_Current);
        m_GameScp = XSFSchema.Instance.Get<SchemaLevelGame>((int)SchemaID.LevelGame).Get((uint)m_Current);
        name.text = m_GameScp.sName;
        for(int i = 0; i < Contents.Length; i ++)
        {
            if(Contents[i] != null)
            {
                if(i == (int)m_Current)
                {
                    Contents[(int)m_Current].SetActive(true);
                }
                else
                {
                    Contents[i].SetActive(false);
                }
            }
        }

        uint nScore = Level.Instance.GetHighScore((int)m_Current);
        if(nScore > 0)
        {
            HighScore.text = $"最高记录：{nScore}";
        }
        else
        {
            HighScore.text = "暂无记录";
        }
    }

	// 
	private void OnPlayClick(GameObject go)
	{
        AudioManager.Instance.PlayUIAudio(ClipID.SngleSelect);

        var ui = XSFUI.Instance.Get((int)UIID.UIExam);
        ui.Show();
        ui.Refresh((uint)UIRefreshID.SetLevel, m_nCurrentLevel);
        XSFUI.Instance.ShowUI((int)UIID.UIExam);
	}

	// 
	private void OnLeftClick(GameObject go)
	{
        AudioManager.Instance.PlayUIAudio(ClipID.SngleSelect);
        if(m_Current <= LevelGameType.Tetris) {
            return;
        }

        m_Current --;
        UpdateSelect();
        Level.Instance.GameType = m_Current;

        m_nCurrentLevel = 1;
        LevelValue.text = m_nCurrentLevel.ToString();
	}

	// 
	private void OnRightClick(GameObject go)
	{
        AudioManager.Instance.PlayUIAudio(ClipID.SngleSelect);
        if(m_Current >= LevelGameType.Max-1) {
            return;
        }

        m_Current ++;
        UpdateSelect();
        Level.Instance.GameType = m_Current;

        m_nCurrentLevel = 1;
        LevelValue.text = m_nCurrentLevel.ToString();
	}


	// 
	private void OnLevelRightClick(GameObject go)
	{
        AudioManager.Instance.PlayUIAudio(ClipID.SngleSelect);
        uint Max = Level.Instance.Current.MaxLevel;
        if(m_nCurrentLevel >= Max)
        {
            return;
        }

        m_nCurrentLevel ++;
        LevelValue.text = m_nCurrentLevel.ToString();
	}

	// 
	private void OnLevelLeftClick(GameObject go)
	{
        AudioManager.Instance.PlayUIAudio(ClipID.SngleSelect);
        if(m_nCurrentLevel <= 1)
        {
            return;
        }

        m_nCurrentLevel --;
        LevelValue.text = m_nCurrentLevel.ToString();
	}


	// 
	private void OnDarkBgClick(GameObject go)
	{
        AudioManager.Instance.PlayFXAudio(ClipID.Rest);
	}

    // UI_FUNC_APPEND
}
