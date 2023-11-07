//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\Scripts\UI\UIEnd.cs
// 作者：Xoen Xie
// 时间：10/24/2023
// 描述：
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;

public sealed class UIEnd : UIBase
{
// UI_PROP_START
	public RectTransform Image { get; private set; }	// 
	public GameObject BtnHome { get; private set; }	// 
// UI_PROP_END

    public override string Name { get { return "UIEnd"; } }

    public override void OnInit()
    {
        // UI_INIT_START
		// 
		Image = RootT.Find("Main/Shine/Image").GetComponent<RectTransform>();
		// 
		BtnHome = RootT.Find("Main/BtnHome").gameObject;
		UIEventClick.Set(BtnHome, OnBtnHomeClick);
        // UI_INIT_END
    }

	public override void OnShow()
	{
		AudioMgr.Instance.PlayFX(AudioID.Finish);
	}

	public override void OnUpdate()
	{
		float a = -10 * Time.deltaTime;
		Image.Rotate(Vector3.forward, a);
	}

	// 
	private void OnBtnHomeClick(GameObject go)
	{
		AudioMgr.Instance.PlayFX(AudioID.Click);
		Close();
		Level.Instance.GoHome();
	}

    // UI_FUNC_APPEND
}
