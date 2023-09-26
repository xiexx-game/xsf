//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\Scripts\UI\UILevel.cs
// 作者：Xoen Xie
// 时间：9/23/2023
// 描述：
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public sealed class UILevel : UIBase
{
// UI_PROP_START
	public RectTransform Level { get; private set; }	// 
	public GameObject Block { get; private set; }	// 
	public GameObject Dot { get; private set; }	// 
	public GameObject Left { get; private set; }	// 
	public GameObject Right { get; private set; }	// 
	public TextMeshProUGUI Title { get; private set; }	// 
	public GameObject OK { get; private set; }	// 
// UI_PROP_END

    public override string Name { get { return "UILevel"; } }

    public override void OnInit()
    {
        // UI_INIT_START
		// 
		Level = RootT.Find("Level").GetComponent<RectTransform>();
		// 
		Block = RootT.Find("Level/Block").gameObject;
		// 
		Dot = RootT.Find("Level/Dot").gameObject;
		// 
		Left = RootT.Find("Left").gameObject;
		UIEventClick.Set(Left, OnLeftClick);
		// 
		Right = RootT.Find("Right").gameObject;
		UIEventClick.Set(Right, OnRightClick);
		// 
		Title = RootT.Find("Title").GetComponent<TextMeshProUGUI>();
		// 
		OK = RootT.Find("OK").gameObject;
		UIEventClick.Set(OK, OnOKClick);
        // UI_INIT_END
    }

	private uint m_nCurLevel;
	private ScpLevel m_ScpLevel;

	public override void OnShow()
	{
		m_nCurLevel = 1;

		m_ScpLevel = XSFSchema.Instance.Get<SchemaLevel>((int)SchemaID.Level).Get(m_nCurLevel);
		Refresh((uint)UIRefreshID.LevelFresh, null);
	}

	public override void OnRefresh(uint nFreshID,  object data) 
	{
		switch(nFreshID)
		{
		case (uint)UIRefreshID.LevelFresh:
			Title.text = $"关卡{m_nCurLevel}";
			break;
		}
	}

	// 
	private void OnLeftClick(GameObject go)
	{
		uint level = m_nCurLevel - 1;
		var scp = XSFSchema.Instance.Get<SchemaLevel>((int)SchemaID.Level).Get(level);
		if(scp == null)
		{

		}
		else 
		{
			m_nCurLevel --;
			m_ScpLevel = scp;
			Refresh((uint)UIRefreshID.LevelFresh, null);
		}
	}

	// 
	private void OnRightClick(GameObject go)
	{
		uint level = m_nCurLevel + 1;
		var scp = XSFSchema.Instance.Get<SchemaLevel>((int)SchemaID.Level).Get(level);
		if(scp == null)
		{

		}
		else 
		{
			m_nCurLevel ++;
			m_ScpLevel = scp;
			Refresh((uint)UIRefreshID.LevelFresh, null);
		}
	}

	// 
	private void OnOKClick(GameObject go)
	{
		Close();
	}

    // UI_FUNC_APPEND
}
