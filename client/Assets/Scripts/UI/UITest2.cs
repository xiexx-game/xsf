//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\Scripts\UI\UITest2.cs
// 作者：Xoen Xie
// 时间：11/13/2023
// 描述：
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using XSF;

namespace XsfUI
{

public sealed class UITest2 : UIBase
{
// UI_PROP_START
	public TextMeshProUGUI TestTxt { get; private set; }	// 测试文本
// UI_PROP_END

    public override string Name { get { return "UITest2"; } }

    public override void OnInit()
    {
        // UI_INIT_START
		// 测试文本
		TestTxt = RootT.Find("Image/Text (TMP)").GetComponent<TextMeshProUGUI>();
        // UI_INIT_END
    }


    // UI_FUNC_APPEND
}

}
