//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\Scripts\Common\UIUtil.cs
// 作者：Xoen Xie
// 时间：2023/06/21
// 描述：UI 相关通用函数，供lua调用
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;
using XLua;

[LuaCallCSharp]
public static class UIUtil
{
    // 设置一个按钮的点击事件
    public static void SetClick(GameObject obj, OnClickCallback callback)
    {
        UIEventClick.Set(obj, callback);
    }
}