//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\Scripts\UI\UIPlayPacMan.cs
// 作者：Xoen Xie
// 时间：3/12/2024
// 描述：
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public sealed class UIPlayPacMan : UIBase
{
// UI_PROP_START
	public GameObject left { get; private set; }	// 
	public GameObject right { get; private set; }	// 
	public GameObject down { get; private set; }	// 
	public GameObject up { get; private set; }	// 
	public GameObject fireworks { get; private set; }	// 
	public TextMeshProUGUI GameLevel { get; private set; }	// 等级
	public TextMeshProUGUI GameScore { get; private set; }	// 分数
	public GameObject Pause { get; private set; }	// 暂停
// UI_PROP_END

    public override string Name { get { return "UIPlayPacMan"; } }

	private float m_fShowTime;
	private bool m_bShowFireworks;

    public override void OnInit()
    {
        // UI_INIT_START
		// 
		left = RootT.Find("bottom-pacman/left").gameObject;
		UIEventClick.Set(left, OnleftClick);
		// 
		right = RootT.Find("bottom-pacman/right").gameObject;
		UIEventClick.Set(right, OnrightClick);
		// 
		down = RootT.Find("bottom-pacman/down").gameObject;
		UIEventClick.Set(down, OndownClick);
		// 
		up = RootT.Find("bottom-pacman/up").gameObject;
		UIEventClick.Set(up, OnupClick);
		// 
		fireworks = RootT.Find("fireworks").gameObject;
		// 等级
		GameLevel = RootT.Find("top-pacman/level/bg-bottom/value").GetComponent<TextMeshProUGUI>();
		// 分数
		GameScore = RootT.Find("top-pacman/score/bg-bottom/value").GetComponent<TextMeshProUGUI>();
		// 暂停
		Pause = RootT.Find("top-pacman/pause").gameObject;
		UIEventClick.Set(Pause, OnPauseClick);
        // UI_INIT_END
    }

	public override void OnShow()
	{
		GameScore.text = Level.Instance.Current.GameSocre.ToString();
		GameLevel.text = Level.Instance.Current.CurrentLevel.ToString();
	}

	public override void OnRefresh(uint nFreshID,  object data)
	{
		switch((UIRefreshID)nFreshID)
		{
		case UIRefreshID.PlayLevel:
			GameLevel.text = Level.Instance.Current.CurrentLevel.ToString();
			break;

		case UIRefreshID.PlayScore:
			GameScore.text = Level.Instance.Current.GameSocre.ToString();
			break;

		case UIRefreshID.ShowFireworks:
			m_fShowTime = 3;
			fireworks.SetActive(true);
			m_bShowFireworks = true;
			break;
		
		}
	}


	public override void OnUpdate()
	{	
		if(m_bShowFireworks)
		{
			m_fShowTime -= Time.deltaTime;
			if(m_fShowTime <= 0)
			{
				m_bShowFireworks = false;
				fireworks.SetActive(false);
			}

		}
	}


	// 
	private void OnleftClick(GameObject go)
	{
		AudioManager.Instance.PlayUIAudio(ClipID.SngleSelect);
		Level.Instance.Current.DoLeft();
	}

	// 
	private void OnrightClick(GameObject go)
	{
		AudioManager.Instance.PlayUIAudio(ClipID.SngleSelect);
		Level.Instance.Current.DoRight();
	}

	// 
	private void OndownClick(GameObject go)
	{
		AudioManager.Instance.PlayUIAudio(ClipID.SngleSelect);
		Level.Instance.Current.DoDown();
	}

	// 
	private void OnupClick(GameObject go)
	{
		AudioManager.Instance.PlayUIAudio(ClipID.SngleSelect);
		Level.Instance.Current.DoUp();
	}

	// 暂停
	private void OnPauseClick(GameObject go)
	{
		AudioManager.Instance.PlayUIAudio(ClipID.SngleSelect);
		Level.Instance.Pause = true;
		var ui = XSFUI.Instance.Get((int)UIID.UIPause);
		
		ui.Show();
		ui.Refresh((uint)UIRefreshID.SetPause, null);
	}

    // UI_FUNC_APPEND
}
