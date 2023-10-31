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

public sealed class UIMain : UIBase
{
// UI_PROP_START
	public GameObject Play { get; private set; }	// 
	public GameObject Change { get; private set; }	// 
	public GameObject BtnLevel { get; private set; }	// 
// UI_PROP_END

    public override string Name { get { return "UIMain"; } }

    public override void OnInit()
    {
        // UI_INIT_START
		// 
		Play = RootT.Find("frame/Play").gameObject;
		UIEventClick.Set(Play, OnPlayClick);
		// 
		Change = RootT.Find("frame/Change").gameObject;
		UIEventClick.Set(Change, OnChangeClick);
		// 
		BtnLevel = RootT.Find("frame/Level").gameObject;
		UIEventClick.Set(BtnLevel, OnBtnLevelClick);
        // UI_INIT_END
    }



	// 
	private void OnPlayClick(GameObject go)
	{
		Close();
		Level.Instance.Play();
	}

	// 
	private void OnChangeClick(GameObject go)
	{
		Level.Instance.ChangeCharacter();
	}


	// 
	private void OnLevelClick(GameObject go)
	{
		
	}


	// 
	private void OnBtnLevelClick(GameObject go)
	{
		XSFUI.Instance.ShowUI((int)UIID.UILevel);
	}

    // UI_FUNC_APPEND
}
