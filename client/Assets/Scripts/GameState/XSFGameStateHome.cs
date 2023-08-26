//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\XSF\GameState\XSFGameStateHome.cs
// 作者：Xoen
// 时间：2023/03/25
// 描述：主页状态机
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using System;
using UnityEngine.AddressableAssets;

using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;

public class XSFGameStateHome : XSFGameState
{
    public override XSFGSID mID { get { return XSFGSID.Home; } }

    public override bool Enter()
    {
        XSFUI.Instance.ShowUI((int)UIID.UISelect);
        return true;
    }

    public override void Exit()
    {
        XSFUI.Instance.CloseUI((int)UIID.UISelect);
    }

    public override void OnUpdate()
    {
        
    }
}
