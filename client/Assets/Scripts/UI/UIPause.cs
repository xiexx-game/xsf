//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\Scripts\UI\UIPause.cs
// 作者：Xoen Xie
// 时间：8/30/2023
// 描述：
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public sealed class UIPause : UIBase
{
// UI_PROP_START
	public TextMeshProUGUI info { get; private set; }	// 
	public GameObject left { get; private set; }	// 
	public GameObject right { get; private set; }	// 
// UI_PROP_END

    public override string Name { get { return "UIPause"; } }

	private UIRefreshID m_RefreshID;

    public override void OnShow()
    {
        AudioManager.Instance.PlayUIAudio(ClipID.Popup);
    }

    public override void OnInit()
    {
        // UI_INIT_START
		// 
		info = RootT.Find("info").GetComponent<TextMeshProUGUI>();
		// 
		left = RootT.Find("left").gameObject;
		UIEventClick.Set(left, OnleftClick);
		// 
		right = RootT.Find("right").gameObject;
		UIEventClick.Set(right, OnrightClick);
        // UI_INIT_END
    }



	// 
	private void OnleftClick(GameObject go)
	{
		AudioManager.Instance.PlayUIAudio(ClipID.SngleSelect);
		switch((UIRefreshID)m_RefreshID)
		{
		case UIRefreshID.SetPause:
			XSFGSManager.Instance.mNextStateID = XSFGSID.Home;
			break;

		case UIRefreshID.SetEnd:
			{
				var current = Level.Instance.CurrentLifeCount;
				if(current <= 0)
				{
					XSFGSManager.Instance.mNextStateID = XSFGSID.Home;
				}
				else
				{
					current --;
        			Level.Instance.CurrentLifeCount = current;
					Level.Instance.Current.Restart();
				}
			}
			break;
		}

		Close();
	}

	// 
	private void OnrightClick(GameObject go)
	{
		AudioManager.Instance.PlayUIAudio(ClipID.SngleSelect);
		switch((UIRefreshID)m_RefreshID)
		{
		case UIRefreshID.SetPause:
			Level.Instance.Pause = false;
			break;

		case UIRefreshID.SetEnd:
			XSFGSManager.Instance.mNextStateID = XSFGSID.Home;
			break;
		}

		Close();
	}

	public override void OnRefresh(uint nFreshID,  object data) 
	{
		m_RefreshID = (UIRefreshID)nFreshID;
		switch((UIRefreshID)nFreshID)
		{
		case UIRefreshID.SetPause:
			info.text = "是否退出游戏？";
			break;

		case UIRefreshID.SetEnd:
			info.text = "游戏结束，再来一局？";
			break;
		}
	}

    // UI_FUNC_APPEND
}
