//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\Scripts\UI\UIMain.cs
// 作者：Xoen Xie
// 时间：9/22/2023
// 描述：
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public sealed class UIMain : UIBase
{
// UI_PROP_START
	public GameObject Play { get; private set; }	// 
	public TextMeshProUGUI Title { get; private set; }	// 
	public GameObject Change { get; private set; }	// 
// UI_PROP_END

    public override string Name { get { return "UIMain"; } }

    public override void OnInit()
    {
        // UI_INIT_START
		// 
		Play = RootT.Find("frame/Play").gameObject;
		UIEventClick.Set(Play, OnPlayClick);
		// 
		Title = RootT.Find("frame/Play/Title").GetComponent<TextMeshProUGUI>();
		// 
		Change = RootT.Find("frame/Change").gameObject;
		UIEventClick.Set(Change, OnChangeClick);
        // UI_INIT_END
    }

	public override void OnRefresh(uint nFreshID,  object data) 
	{
		switch(nFreshID)
		{
		case (uint)UIRefreshID.LevelFresh:
			Title.text = "关卡" + Level.Instance.LevelConfig.uId;
			break;
		}
	}


	// 
	private void OnPlayClick(GameObject go)
	{
		AudioMgr.Instance.PlayFX(AudioID.Click);
		Close();
		XSFUI.Instance.CloseUI((int)UIID.UILevel);
		Level.Instance.Play();
	}

	// 
	private void OnChangeClick(GameObject go)
	{
		AudioMgr.Instance.PlayFX(AudioID.Click);
		Level.Instance.ChangeCharacter();
	}

    // UI_FUNC_APPEND
}
