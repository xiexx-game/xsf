//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Scripts/Loading/LoadingLevelGame.cs
// 作者：Xoen
// 时间：2023/08/25
// 描述：加载 游戏相关初始化
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;

public class LoadingLevelGame : LoadingBase
{
    public override bool IsDone 
    { 
        get 
        { 
            return true;
        } 
    }

    public int UI;

    public override void Start()
    {
        Level.Instance.Current.PreCreate();
    }

    public override void End()
    {

    }
}