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
using System.Collections.Generic;

public sealed class UILevel : UIBase
{
// UI_PROP_START
	public RectTransform LevelObj { get; private set; }	// 
	public GameObject Block { get; private set; }	// 
	public GameObject Dot { get; private set; }	// 
	public GameObject Kid { get; private set; }	// 
	public GameObject Left { get; private set; }	// 
	public GameObject Right { get; private set; }	// 
// UI_PROP_END

    public override string Name { get { return "UILevel"; } }

	private SingleBlock[] m_Blocks;

	private List<GameObject> m_Dots;

    public override void OnInit()
    {
        // UI_INIT_START
		// 
		LevelObj = RootT.Find("Level").GetComponent<RectTransform>();
		// 
		Block = RootT.Find("Level/Block").gameObject;
		// 
		Dot = RootT.Find("Level/Dot").gameObject;
		// 
		Kid = RootT.Find("Level/Kid").gameObject;
		// 
		Left = RootT.Find("Left").gameObject;
		UIEventClick.Set(Left, OnLeftClick);
		// 
		Right = RootT.Find("Right").gameObject;
		UIEventClick.Set(Right, OnRightClick);
        // UI_INIT_END

		m_Dots = new List<GameObject>();
    }

	private uint m_nCurLevel;
	private ScpLevel m_ScpLevel;

	public override void OnShow()
	{
		m_nCurLevel = (uint)PlayerPrefs.GetInt("CurLevel", 1);

		m_ScpLevel = XSFSchema.Instance.Get<SchemaLevel>((int)SchemaID.Level).Get(m_nCurLevel);
		Level.Instance.LevelConfig = m_ScpLevel;
		Refresh((uint)UIRefreshID.LevelFresh, null);
		XSFUI.Instance.Get((int)UIID.UIMain).Refresh((uint)UIRefreshID.LevelFresh, null);
	}

	public override void OnClose()
	{
		ClearBlock();

		for(int i = 0; i < m_Dots.Count; i ++)
		{
			GameObject.Destroy(m_Dots[i]);
		}
		m_Dots.Clear();
	}

	private void ClearBlock()
	{
		if(m_Blocks != null)
		{
			for(int i = 0; i < m_Blocks.Length; i ++)
			{
				m_Blocks[i].Clear();
			}
		}
	}

	public override void OnRefresh(uint nFreshID,  object data) 
	{
		switch(nFreshID)
		{
		case (uint)UIRefreshID.LevelFresh:
			//Title.text = $"关卡{m_nCurLevel}";
			OnClose();
			LevelObj.sizeDelta = new Vector2(m_ScpLevel.uColCount*80, m_ScpLevel.uRowCount * 80);
			m_Blocks = LevelDef.CreateBlocks((int)m_ScpLevel.uRowCount, (int)m_ScpLevel.uColCount, LevelObj.transform, Block, 80.0f, true);
			for(int i = 0; i < m_ScpLevel.sarData.Length; i ++)
			{
				if(m_ScpLevel.sarData[i] == "#") 
				{
					m_Blocks[i].SetColor(BlockColor.Wall);
				}
				else if(m_ScpLevel.sarData[i] == "-")
				{
					m_Blocks[i].SetColor(BlockColor.UIRoad);
				}
				else if(m_ScpLevel.sarData[i] == "@")
				{
					m_Blocks[i].SetColor(BlockColor.UIRoad);
					Kid.transform.position = m_Blocks[i].go.transform.position;
					Kid.SetActive(true);
					Kid.transform.SetAsLastSibling();
				}
				else if(m_ScpLevel.sarData[i] == ".")
				{
					m_Blocks[i].SetColor(BlockColor.UIRoad);
					var dot = GameObject.Instantiate(Dot);
					dot.transform.SetParent(LevelObj.transform);
					dot.transform.position = m_Blocks[i].go.transform.position;
					dot.transform.localScale = Dot.transform.localScale;
					dot.SetActive(true);
					m_Dots.Add(dot);
				}
				else if(m_ScpLevel.sarData[i] == "$")
				{
					m_Blocks[i].SetColor(BlockColor.Box);
				}
				else if(m_ScpLevel.sarData[i] == "*")
				{
					m_Blocks[i].SetColor(BlockColor.Box);
					

					var dot = GameObject.Instantiate(Dot);
					dot.transform.SetParent(LevelObj.transform);
					dot.transform.position = m_Blocks[i].go.transform.position;
					dot.transform.localScale = Dot.transform.localScale;

					var cur = dot.GetComponent<Image>().color;
					cur.a = 0.8f;
					dot.GetComponent<Image>().color = cur;
					
					dot.SetActive(true);
					m_Dots.Add(dot);
				}
			}
			break;
		}
	}

	// 
	private void OnLeftClick(GameObject go)
	{
		AudioMgr.Instance.PlayFX(AudioID.Click);
		uint level = m_nCurLevel - 1;
		var scp = XSFSchema.Instance.Get<SchemaLevel>((int)SchemaID.Level).Get(level);
		if(level == 0 || scp == null)
		{

		}
		else 
		{
			m_nCurLevel --;
			PlayerPrefs.SetInt("CurLevel", (int)m_nCurLevel);
			m_ScpLevel = scp;
			Level.Instance.LevelConfig = scp;
			Refresh((uint)UIRefreshID.LevelFresh, null);
			XSFUI.Instance.Get((int)UIID.UIMain).Refresh((uint)UIRefreshID.LevelFresh, null);
			
		}
	}

	// 
	private void OnRightClick(GameObject go)
	{
		AudioMgr.Instance.PlayFX(AudioID.Click);
		uint level = m_nCurLevel + 1;
		var scp = XSFSchema.Instance.Get<SchemaLevel>((int)SchemaID.Level).Get(level);
		if(scp == null)
		{

		}
		else 
		{
			m_nCurLevel ++;
			PlayerPrefs.SetInt("CurLevel", (int)m_nCurLevel);
			m_ScpLevel = scp;
			Level.Instance.LevelConfig = scp;

			Refresh((uint)UIRefreshID.LevelFresh, null);
			XSFUI.Instance.Get((int)UIID.UIMain).Refresh((uint)UIRefreshID.LevelFresh, null);
		}
	}

	// 
	private void OnOKClick(GameObject go)
	{
		Close();
	}

    // UI_FUNC_APPEND
}
