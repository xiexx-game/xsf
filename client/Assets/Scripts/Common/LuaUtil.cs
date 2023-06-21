//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\Scripts\Common\LuaUtil.cs
// 作者：Xoen Xie
// 时间：2023/06/21
// 描述：一些通用函数，供lua调用
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using XLua;
using UnityEngine;

[LuaCallCSharp]
public static class LuaUtil
{
    public static Camera UICamera { get { return XSFMain.Instance.UICamera; } }
    public static Camera MainCamera { get { return XSFMain.Instance.MainCamera; } }

    public static GameObject UIRoot { get { return XSFMain.Instance.UIRoot; } }

    public static void FireEvent(uint nEventID, uint nObjectID, LuaTable context, bool bFireAll)
    {
        XSFEvent.Instance.Fire(nEventID, nObjectID, context, bFireAll);
    }

    public static int SubscribeEvent(uint nEventID, uint nObjectID, OnLuaEventFunc func)
    {
        return LuaEvent.Instance.Subscribe(nEventID, nObjectID, func);
    }

    public static void UnsubscribeEvent(int nID)
    {
        LuaEvent.Instance.Unsubscribe(nID);
    }
}