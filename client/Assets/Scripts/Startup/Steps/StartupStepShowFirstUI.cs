//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\Scripts\Startup\Steps\StartupStepShowFirstUI.cs
// 作者：Xoen Xie
// 时间：2023/06/29
// 描述：显示第一个UI
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using UnityEngine;

public sealed class StartupStepShowFirstUI : StartupStep
{
    public override void Start()
    {
        Debug.LogWarning("StartupStepShowFirstUI Start");

        Subscribe((EventID)XSF.UI_SHOW_EVENT_ID, (uint)EventID.UITestShow);

        XSFUI.Instance.Get<UITest>((int)UIID.UITest).Show();
    }
}