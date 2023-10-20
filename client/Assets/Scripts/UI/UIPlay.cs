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
        // UI_INIT_END
    }



	// 
	private void OnRedoClick(GameObject go)
	{
		Level.Instance.Redo();
	}

	// 
	private void OnBtnUpClick(GameObject go)
	{
		
		Level.Instance.MoveRight();
	}

	// 
	private void OnBtnRightClick(GameObject go)
	{
		
		Level.Instance.MoveDown();
	}

	// 
	private void OnBtnDownClick(GameObject go)
	{
		Level.Instance.MoveLeft();
	}

	// 
	private void OnBtnLeftClick(GameObject go)
	{
		
		Level.Instance.MoveUp();
	}

    // UI_FUNC_APPEND
}
