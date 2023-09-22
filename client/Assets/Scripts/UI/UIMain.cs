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
// UI_PROP_END

    public override string Name { get { return "UIMain"; } }

    public override void OnInit()
    {
        // UI_INIT_START
		// 
		Play = RootT.Find("Play").gameObject;
		UIEventClick.Set(Play, OnPlayClick);
		// 
		Change = RootT.Find("Change").gameObject;
		UIEventClick.Set(Change, OnChangeClick);
        // UI_INIT_END
    }



	// 
	private void OnPlayClick(GameObject go)
	{

	}

	// 
	private void OnChangeClick(GameObject go)
	{
		var state = XSFGSManager.Instance.CurState as XSFGameStateMain;
		state.ChangeCharacter();
	}

    // UI_FUNC_APPEND
}
