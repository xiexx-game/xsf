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

public sealed class UISelect : UIBase
{
// UI_PROP_START
	public GameObject Play { get; private set; }	// 
	public TextMeshProUGUI name { get; private set; }	// 
	public GameObject Blocks { get; private set; }	// 
	public GameObject snake { get; private set; }	// 
	public GameObject pacman { get; private set; }	// 
	public GameObject Right { get; private set; }	// 
	public GameObject Left { get; private set; }	// 
// UI_PROP_END

    public override string Name { get { return "UISelect"; } }

    public override uint EventObjID { get { return (uint)EventObjectID.UISelectShow; } }

    private GameObject[] Contents;

    private LevelGameType m_Current;
    private ScpLevelGame m_GameScp;

    public override void OnInit()
    {
        // UI_INIT_START
		// 
		Play = RootT.Find("play").gameObject;
		UIEventClick.Set(Play, OnPlayClick);
		// 
		name = RootT.Find("item/titel/name").GetComponent<TextMeshProUGUI>();
		// 
		Blocks = RootT.Find("item/content/blocks").gameObject;
		// 
		snake = RootT.Find("item/content/snake").gameObject;
		// 
		pacman = RootT.Find("item/content/pacman").gameObject;
		// 
		Right = RootT.Find("right").gameObject;
		UIEventClick.Set(Right, OnRightClick);
		// 
		Left = RootT.Find("left").gameObject;
		UIEventClick.Set(Left, OnLeftClick);
        // UI_INIT_END

        Contents = new GameObject[(int)LevelGameType.Max];
        Contents[(int)LevelGameType.Tetris] = Blocks;
        Contents[(int)LevelGameType.Snake] = snake;
        Contents[(int)LevelGameType.PacMan] = pacman;
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
        UpdateSelect();
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
    }

	// 
	private void OnPlayClick(GameObject go)
	{
        Level.Instance.GameType = m_Current;
        Level.Instance.Load();
	}

	// 
	private void OnLeftClick(GameObject go)
	{
        if(m_Current <= LevelGameType.Tetris) {
            return;
        }

        m_Current --;
        UpdateSelect();
	}

	// 
	private void OnRightClick(GameObject go)
	{
        if(m_Current >= LevelGameType.Max-1) {
            return;
        }

        m_Current ++;
        UpdateSelect();
	}

    // UI_FUNC_APPEND
}
