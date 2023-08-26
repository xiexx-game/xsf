//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Scripts/Loading/LoadingUI.cs
// 作者：Xoen
// 时间：2023/08/25
// 描述：加载 UI
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;

public class LoadingUI : LoadingBase
{
    public override bool IsDone 
    { 
        get 
        { 
            return XSFUI.Instance.IsUIShow(UI);
        } 
    }

    public int UI;

    public override void Start()
    {
        XSFUI.Instance.ShowUI(UI);
    }

    public override void End()
    {

    }
}