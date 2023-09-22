//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets/Scripts/Startup/Steps/StartupStepStart.cs
// 作者：Xoen Xie
// 时间：2023/06/29
// 描述：开始
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using UnityEngine;

public sealed class StartupStepStart : StartupStep
{
    public override void Start()
    {
        Debug.LogWarning("StartupStepStart Start");

        XSFGSManager.Instance.mNextStateID = XSFGSID.Main;
        IsDone = true;
    }
}