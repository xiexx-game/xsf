//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\XSF\GameState\XSFGameStateMain.cs
// 作者：Xiexx
// 时间：2022/04/11
// 描述：游戏状态机
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class XSFGameStateMain : XSFGameState
{
    public override XSFGSID mID { get { return XSFGSID.Main; } }

    public override bool Enter()
    {
        Level.Instance.Start();
        return true;
    }

    public override void Exit()
    {

    }

    public override void OnUpdate()
    {
        Level.Instance.OnUpdate();
    }
}
