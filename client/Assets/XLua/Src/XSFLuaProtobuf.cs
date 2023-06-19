//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\XLua\Src\XSFLuaProtobuf.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：Lua protobuf
// 说明：
//
//////////////////////////////////////////////////////////////////////////
namespace XLua.LuaDLL
{
    using System.Runtime.InteropServices;

    public partial class Lua
    {

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_pb(System.IntPtr L);

        [MonoPInvokeCallback(typeof(LuaDLL.lua_CSFunction))]
        public static int LoadLuaProfobuf(System.IntPtr L)
        {
            return luaopen_pb(L);
        }
    }
}