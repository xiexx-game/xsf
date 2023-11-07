//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\Scripts\UI\UIPlay.cs
// 作者：Xoen Xie
// 时间：10/20/2023
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
	public GameObject Redo { get; private set; }	// 
	public GameObject BtnUp { get; private set; }	// 
	public GameObject BtnRight { get; private set; }	// 
	public GameObject BtnDown { get; private set; }	// 
	public GameObject BtnLeft { get; private set; }	// 
	public TextMeshProUGUI Best { get; private set; }	// 
	public TextMeshProUGUI CurrentValue { get; private set; }	// 
	public GameObject Return { get; private set; }	// 
// UI_PROP_END

    public override string Name { get { return "UIPlay"; } }

    public override void OnInit()
    {
        // UI_INIT_START
		// 
		Redo = RootT.Find("Redo").gameObject;
		UIEventClick.Set(Redo, OnRedoClick);
		// 
		BtnUp = RootT.Find("op/background/up").gameObject;
		UIEventClick.Set(BtnUp, OnBtnUpClick);
		// 
		BtnRight = RootT.Find("op/background/right").gameObject;
		UIEventClick.Set(BtnRight, OnBtnRightClick);
		// 
		BtnDown = RootT.Find("op/background/down").gameObject;
		UIEventClick.Set(BtnDown, OnBtnDownClick);
		// 
		BtnLeft = RootT.Find("op/background/left").gameObject;
		UIEventClick.Set(BtnLeft, OnBtnLeftClick);
		// 
		Best = RootT.Find("Title/Best").GetComponent<TextMeshProUGUI>();
		// 
		CurrentValue = RootT.Find("Title/CurrentValue").GetComponent<TextMeshProUGUI>();
		// 
		Return = RootT.Find("Return").gameObject;
		UIEventClick.Set(Return, OnReturnClick);
        // UI_INIT_END
    }



	// 
	private void OnRedoClick(GameObject go)
	{
		AudioMgr.Instance.PlayFX(AudioID.Click);
		Level.Instance.Redo();
	}

	// 
	private void OnBtnUpClick(GameObject go)
	{
		AudioMgr.Instance.PlayFX(AudioID.Click);
		Level.Instance.MoveRight();
	}

	// 
	private void OnBtnRightClick(GameObject go)
	{
		AudioMgr.Instance.PlayFX(AudioID.Click);
		Level.Instance.MoveDown();
	}

	// 
	private void OnBtnDownClick(GameObject go)
	{
		AudioMgr.Instance.PlayFX(AudioID.Click);
		Level.Instance.MoveLeft();
	}

	// 
	private void OnBtnLeftClick(GameObject go)
	{
		AudioMgr.Instance.PlayFX(AudioID.Click);
		Level.Instance.MoveUp();
	}


	// 
	private void OnReturnClick(GameObject go)
	{
		AudioMgr.Instance.PlayFX(AudioID.Click);
		Level.Instance.GoHome();
	}

    // UI_FUNC_APPEND
}
