//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\Scripts\UI\UIAtlasTest.cs
// 作者：Xoen Xie
// 时间：2023/6/29
// 描述：
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;
using XSF;

namespace XsfUI
{
public sealed class UIAtlasTest : UIBase
{
	// UI_PROP_START
	public UIImageSwitcher Icon { get; private set; }   // Icon
	public GameObject Button1 { get; private set; } // Button1
	public GameObject Button2 { get; private set; } // Button2
	public GameObject BtnClose { get; private set; }    // Close
														// UI_PROP_END

	public override string Name { get { return "UIAtlasTest"; } }

	public override void OnInit()
	{
		// UI_INIT_START
		// Icon
		Icon = RootT.Find("Icon").GetComponent<UIImageSwitcher>();
		// Button1
		Button1 = RootT.Find("Button1").gameObject;
		UIEventClick.Set(Button1, OnButton1Click);
		// Button2
		Button2 = RootT.Find("Button2").gameObject;
		UIEventClick.Set(Button2, OnButton2Click);
		// Close
		BtnClose = RootT.Find("Close").gameObject;
		UIEventClick.Set(BtnClose, OnBtnCloseClick);
		// UI_INIT_END
	}

	public override void OnRefresh(uint nFreshID, object data)
	{
		switch ((UIRefreshID)nFreshID)
		{
			case UIRefreshID.UIAtlasTest_SetImage:
				Icon.SetImage("Fruits", "Banana");
				break;
		}
	}


	// Button1



	// Button1
	private void OnButton1Click(GameObject go)
	{
		Icon.SetImage("Fruits", "Carrot");
	}

	// Button2
	private void OnButton2Click(GameObject go)
	{
		Icon.SetImage("Egg", "Egg");
	}

	// Close
	private void OnBtnCloseClick(GameObject go)
	{
		Close();
	}
	// UI_FUNC_APPEND
}
}
