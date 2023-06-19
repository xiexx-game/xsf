//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\XLua\Src\XSFStaticLuaCallbacks.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：框架自定义Lua接口
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#if USE_UNI_LUA
using LuaAPI = UniLua.Lua;
using RealStatePtr = UniLua.ILuaState;
using LuaCSFunction = UniLua.CSharpFunctionDelegate;
#else
using LuaAPI = XLua.LuaDLL.Lua;
using RealStatePtr = System.IntPtr;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;
#endif

using XLua;
using System;

public static class XSFStaticLuaCallbacks
{
    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    internal static int XSFLog(RealStatePtr L)
    {
        try
        {
            int n = LuaAPI.lua_gettop(L);
            string s = String.Empty;

            if (0 != LuaAPI.xlua_getglobal(L, "tostring"))
            {
                return LuaAPI.luaL_error(L, "can not get tostring in print:");
            }

            for (int i = 1; i <= n; i++)
            {
                LuaAPI.lua_pushvalue(L, -1);  /* function to be called */
                LuaAPI.lua_pushvalue(L, i);   /* value to print */
                if (0 != LuaAPI.lua_pcall(L, 1, 1, 0))
                {
                    return LuaAPI.lua_error(L);
                }
                s += LuaAPI.lua_tostring(L, -1);

                if (i != n) s += " ";

                LuaAPI.lua_pop(L, 1);  /* pop result */
            }
            XSF.Log("LUA: " + s);
            return 0;
        }
        catch (System.Exception e)
        {
            return LuaAPI.luaL_error(L, "c# exception in print:" + e);
        }
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    internal static int XSFLogWarn(RealStatePtr L)
    {
        try
        {
            int n = LuaAPI.lua_gettop(L);
            string s = String.Empty;

            if (0 != LuaAPI.xlua_getglobal(L, "tostring"))
            {
                return LuaAPI.luaL_error(L, "can not get tostring in print:");
            }

            for (int i = 1; i <= n; i++)
            {
                LuaAPI.lua_pushvalue(L, -1);  /* function to be called */
                LuaAPI.lua_pushvalue(L, i);   /* value to print */
                if (0 != LuaAPI.lua_pcall(L, 1, 1, 0))
                {
                    return LuaAPI.lua_error(L);
                }
                s += LuaAPI.lua_tostring(L, -1);

                if (i != n) s += " ";

                LuaAPI.lua_pop(L, 1);  /* pop result */
            }
            XSF.LogWarning("LUA: " + s);
            return 0;
        }
        catch (System.Exception e)
        {
            return LuaAPI.luaL_error(L, "c# exception in print:" + e);
        }
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    internal static int XSFLogError(RealStatePtr L)
    {
        try
        {
            int n = LuaAPI.lua_gettop(L);
            string s = String.Empty;

            if (0 != LuaAPI.xlua_getglobal(L, "tostring"))
            {
                return LuaAPI.luaL_error(L, "can not get tostring in print:");
            }

            for (int i = 1; i <= n; i++)
            {
                LuaAPI.lua_pushvalue(L, -1);  /* function to be called */
                LuaAPI.lua_pushvalue(L, i);   /* value to print */
                if (0 != LuaAPI.lua_pcall(L, 1, 1, 0))
                {
                    return LuaAPI.lua_error(L);
                }
                s += LuaAPI.lua_tostring(L, -1);

                if (i != n) s += " ";

                LuaAPI.lua_pop(L, 1);  /* pop result */
            }
            XSF.LogError("LUA: " + s);
            return 0;
        }
        catch (System.Exception e)
        {
            return LuaAPI.luaL_error(L, "c# exception in print:" + e);
        }
    }

    
}