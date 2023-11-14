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
using XSF;
using XsfUI;

public sealed class StartupStepShowFirstUI : StartupStep
{
    public override void Start()
    {
        Debug.LogWarning("StartupStepShowFirstUI Start");

        Subscribe((EventID)XSFCore.UI_SHOW_EVENT_ID, (uint)UIObjectID.UITestShow);

        XSFUI.Instance.Get((int)UIID.Test).Show();
    }
}