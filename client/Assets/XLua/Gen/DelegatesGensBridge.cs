#if USE_UNI_LUA
using LuaAPI = UniLua.Lua;
using RealStatePtr = UniLua.ILuaState;
using LuaCSFunction = UniLua.CSharpFunctionDelegate;
#else
using LuaAPI = XLua.LuaDLL.Lua;
using RealStatePtr = System.IntPtr;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;
#endif

using System;


namespace XLua
{
    public partial class DelegateBridge_Wrap : DelegateBridge
    {
		public DelegateBridge_Wrap(int reference, LuaEnv luaenv) : base(reference, luaenv){}
		
		public void __Gen_Delegate_Imp0(UnityEngine.Camera p0)
		{
#if THREAD_SAFE || HOTFIX_ENABLE
            lock (luaEnv.luaEnvLock)
            {
#endif
                RealStatePtr L = luaEnv.L;
                int errFunc = LuaAPI.pcall_prepare(L, errorFuncRef, luaReference);
                ObjectTranslator translator = luaEnv.translator;
                translator.Push(L, p0);
                
                PCall(L, 1, 0, errFunc);
                
                
                
                LuaAPI.lua_settop(L, errFunc - 1);
                
#if THREAD_SAFE || HOTFIX_ENABLE
            }
#endif
		}
        
		public void __Gen_Delegate_Imp1(UnityEngine.RectTransform p0)
		{
#if THREAD_SAFE || HOTFIX_ENABLE
            lock (luaEnv.luaEnvLock)
            {
#endif
                RealStatePtr L = luaEnv.L;
                int errFunc = LuaAPI.pcall_prepare(L, errorFuncRef, luaReference);
                ObjectTranslator translator = luaEnv.translator;
                translator.Push(L, p0);
                
                PCall(L, 1, 0, errFunc);
                
                
                
                LuaAPI.lua_settop(L, errFunc - 1);
                
#if THREAD_SAFE || HOTFIX_ENABLE
            }
#endif
		}
        
		public void __Gen_Delegate_Imp2()
		{
#if THREAD_SAFE || HOTFIX_ENABLE
            lock (luaEnv.luaEnvLock)
            {
#endif
                RealStatePtr L = luaEnv.L;
                int errFunc = LuaAPI.pcall_prepare(L, errorFuncRef, luaReference);
                
                
                PCall(L, 0, 0, errFunc);
                
                
                
                LuaAPI.lua_settop(L, errFunc - 1);
                
#if THREAD_SAFE || HOTFIX_ENABLE
            }
#endif
		}
        
		public char __Gen_Delegate_Imp3(string p0, int p1, char p2)
		{
#if THREAD_SAFE || HOTFIX_ENABLE
            lock (luaEnv.luaEnvLock)
            {
#endif
                RealStatePtr L = luaEnv.L;
                int errFunc = LuaAPI.pcall_prepare(L, errorFuncRef, luaReference);
                
                LuaAPI.lua_pushstring(L, p0);
                LuaAPI.xlua_pushinteger(L, p1);
                LuaAPI.xlua_pushinteger(L, p2);
                
                PCall(L, 3, 1, errFunc);
                
                
                char __gen_ret = (char)LuaAPI.xlua_tointeger(L, errFunc + 1);
                LuaAPI.lua_settop(L, errFunc - 1);
                return  __gen_ret;
#if THREAD_SAFE || HOTFIX_ENABLE
            }
#endif
		}
        
		public TMPro.TMP_FontAsset __Gen_Delegate_Imp4(int p0, string p1)
		{
#if THREAD_SAFE || HOTFIX_ENABLE
            lock (luaEnv.luaEnvLock)
            {
#endif
                RealStatePtr L = luaEnv.L;
                int errFunc = LuaAPI.pcall_prepare(L, errorFuncRef, luaReference);
                ObjectTranslator translator = luaEnv.translator;
                LuaAPI.xlua_pushinteger(L, p0);
                LuaAPI.lua_pushstring(L, p1);
                
                PCall(L, 2, 1, errFunc);
                
                
                TMPro.TMP_FontAsset __gen_ret = (TMPro.TMP_FontAsset)translator.GetObject(L, errFunc + 1, typeof(TMPro.TMP_FontAsset));
                LuaAPI.lua_settop(L, errFunc - 1);
                return  __gen_ret;
#if THREAD_SAFE || HOTFIX_ENABLE
            }
#endif
		}
        
		public TMPro.TMP_SpriteAsset __Gen_Delegate_Imp5(int p0, string p1)
		{
#if THREAD_SAFE || HOTFIX_ENABLE
            lock (luaEnv.luaEnvLock)
            {
#endif
                RealStatePtr L = luaEnv.L;
                int errFunc = LuaAPI.pcall_prepare(L, errorFuncRef, luaReference);
                ObjectTranslator translator = luaEnv.translator;
                LuaAPI.xlua_pushinteger(L, p0);
                LuaAPI.lua_pushstring(L, p1);
                
                PCall(L, 2, 1, errFunc);
                
                
                TMPro.TMP_SpriteAsset __gen_ret = (TMPro.TMP_SpriteAsset)translator.GetObject(L, errFunc + 1, typeof(TMPro.TMP_SpriteAsset));
                LuaAPI.lua_settop(L, errFunc - 1);
                return  __gen_ret;
#if THREAD_SAFE || HOTFIX_ENABLE
            }
#endif
		}
        
		public void __Gen_Delegate_Imp6(TMPro.TMP_TextInfo p0)
		{
#if THREAD_SAFE || HOTFIX_ENABLE
            lock (luaEnv.luaEnvLock)
            {
#endif
                RealStatePtr L = luaEnv.L;
                int errFunc = LuaAPI.pcall_prepare(L, errorFuncRef, luaReference);
                ObjectTranslator translator = luaEnv.translator;
                translator.Push(L, p0);
                
                PCall(L, 1, 0, errFunc);
                
                
                
                LuaAPI.lua_settop(L, errFunc - 1);
                
#if THREAD_SAFE || HOTFIX_ENABLE
            }
#endif
		}
        
		public bool __Gen_Delegate_Imp7(System.Type p0)
		{
#if THREAD_SAFE || HOTFIX_ENABLE
            lock (luaEnv.luaEnvLock)
            {
#endif
                RealStatePtr L = luaEnv.L;
                int errFunc = LuaAPI.pcall_prepare(L, errorFuncRef, luaReference);
                ObjectTranslator translator = luaEnv.translator;
                translator.Push(L, p0);
                
                PCall(L, 1, 1, errFunc);
                
                
                bool __gen_ret = LuaAPI.lua_toboolean(L, errFunc + 1);
                LuaAPI.lua_settop(L, errFunc - 1);
                return  __gen_ret;
#if THREAD_SAFE || HOTFIX_ENABLE
            }
#endif
		}
        
		
		public override Delegate GetDelegateByType(Type type)
		{
		
		    if (type == typeof(UnityEngine.Camera.CameraCallback))
			{
			    return new UnityEngine.Camera.CameraCallback(__Gen_Delegate_Imp0);
			}
		
		    if (type == typeof(UnityEngine.RectTransform.ReapplyDrivenProperties))
			{
			    return new UnityEngine.RectTransform.ReapplyDrivenProperties(__Gen_Delegate_Imp1);
			}
		
		    if (type == typeof(UnityEngine.Canvas.WillRenderCanvases))
			{
			    return new UnityEngine.Canvas.WillRenderCanvases(__Gen_Delegate_Imp2);
			}
		
		    if (type == typeof(UnityEngine.Events.UnityAction))
			{
			    return new UnityEngine.Events.UnityAction(__Gen_Delegate_Imp2);
			}
		
		    if (type == typeof(TMPro.TMP_InputField.OnValidateInput))
			{
			    return new TMPro.TMP_InputField.OnValidateInput(__Gen_Delegate_Imp3);
			}
		
		    if (type == typeof(System.Func<int, string, TMPro.TMP_FontAsset>))
			{
			    return new System.Func<int, string, TMPro.TMP_FontAsset>(__Gen_Delegate_Imp4);
			}
		
		    if (type == typeof(System.Func<int, string, TMPro.TMP_SpriteAsset>))
			{
			    return new System.Func<int, string, TMPro.TMP_SpriteAsset>(__Gen_Delegate_Imp5);
			}
		
		    if (type == typeof(System.Action<TMPro.TMP_TextInfo>))
			{
			    return new System.Action<TMPro.TMP_TextInfo>(__Gen_Delegate_Imp6);
			}
		
		    if (type == typeof(System.Func<System.Type, bool>))
			{
			    return new System.Func<System.Type, bool>(__Gen_Delegate_Imp7);
			}
		
		    return null;
		}
	}
    
}