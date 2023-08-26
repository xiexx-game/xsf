//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\Scripts\UI\UITest.cs
// 作者：Xoen Xie
// 时间：2023/06/29
// 描述：UI 刷新ID
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public sealed class UITest : UIBase
{
// UI_PROP_START
	public Image Image { get; private set; }	// 
	public GameObject Button1 { get; private set; }	// Button1 click
	public UILocalizationTMP Btn1Text { get; private set; }	// Btn1Text
	public GameObject Button2 { get; private set; }	// Button2 click
	public TextMeshProUGUI Button2Text { get; private set; }	// Button2Text
	public GameObject Button3 { get; private set; }	// Button3 click
// UI_PROP_END

    public override string Name { get { return "UITest"; } }

    public override uint EventObjID { get { return (uint)EventObjectID.UITestShow; } }

    public override void OnInit()
    {
        // UI_INIT_START
		// 
		Image = RootT.Find("Image").GetComponent<Image>();
		// Button1 click
		Button1 = RootT.Find("Bottom/Button1").gameObject;
		UIEventClick.Set(Button1, OnButton1Click);
		// Btn1Text
		Btn1Text = RootT.Find("Bottom/Button1/Text").GetComponent<UILocalizationTMP>();
		// Button2 click
		Button2 = RootT.Find("Bottom/Button2").gameObject;
		UIEventClick.Set(Button2, OnButton2Click);
		// Button2Text
		Button2Text = RootT.Find("Bottom/Button2/Text").GetComponent<TextMeshProUGUI>();
		// Button3 click
		Button3 = RootT.Find("Bottom/Button3").gameObject;
        // UI_INIT_END
    }


    private void OnButton1Click(GameObject go)
    {
        Btn1Text.SetKey("login");
    }


	// Button2 click
	private void OnButton2Click(GameObject go)
	{
        var ui = XSFUI.Instance.Get((int)UIID.UIAtlasTest);
        ui.Show();
        ui.Refresh((uint)UIRefreshID.UIAtlasTest_SetImage, null);
	}
    // UI_FUNC_APPEND
}
