//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\Scripts\UI\UIPlay.cs
// 作者：Xoen Xie
// 时间：8/24/2023
// 描述：
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public sealed class UIPlay : UIBase
{
// UI_PROP_START
	public GameObject left { get; private set; }	// 
	public GameObject right { get; private set; }	// 
	public GameObject down { get; private set; }	// 
	public GameObject change { get; private set; }	// 
	public GameObject ultra { get; private set; }	// 
	public TextMeshProUGUI GameLevel { get; private set; }	// 等级
	public TextMeshProUGUI GameScore { get; private set; }	// 分数
	public GameObject Pause { get; private set; }	// 暂停
// UI_PROP_END

    public override string Name { get { return "UIPlay"; } }

    public override void OnInit()
    {
        // UI_INIT_START
		// 
		left = RootT.Find("bottom/left").gameObject;
		UIEventClick.Set(left, OnleftClick);
		// 
		right = RootT.Find("bottom/right").gameObject;
		UIEventClick.Set(right, OnrightClick);
		// 
		down = RootT.Find("bottom/down").gameObject;
		UIEventClick.Set(down, OndownClick);
		// 
		change = RootT.Find("bottom/change").gameObject;
		UIEventClick.Set(change, OnchangeClick);
		// 
		ultra = RootT.Find("bottom/f").gameObject;
		UIEventClick.Set(ultra, OnultraClick);
		// 等级
		GameLevel = RootT.Find("top/level/bg-bottom/value").GetComponent<TextMeshProUGUI>();
		// 分数
		GameScore = RootT.Find("top/score/bg-bottom/value").GetComponent<TextMeshProUGUI>();
		// 暂停
		Pause = RootT.Find("top/pause").gameObject;
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
		
		}
	}

	// 向左
	private void OnleftClick(GameObject go)
	{
		AudioManager.Instance.PlayUIAudio(ClipID.SngleSelect);
		Level.Instance.Current.MoveLeft();
	}

	// 向右
	private void OnrightClick(GameObject go)
	{
		AudioManager.Instance.PlayUIAudio(ClipID.SngleSelect);
		Level.Instance.Current.MoveRight();
	}

	// 向下
	private void OndownClick(GameObject go)
	{
		AudioManager.Instance.PlayUIAudio(ClipID.SngleSelect);
		Level.Instance.Current.MoveDown();
	}

	// 变形
	private void OnchangeClick(GameObject go)
	{
		AudioManager.Instance.PlayUIAudio(ClipID.SngleSelect);
		Level.Instance.Current.Change();
	}


	// 
	private void OnultraClick(GameObject go)
	{
		Level.Instance.Current.Ultra();
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
