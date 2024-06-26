//////////////////////////////////////////////////////////////////////////
//
// 文件：XSF/Base/Singleton.cs
// 作者：Xoen Xie
// 时间：2023/06/15
// 描述：单例模板
// 说明：
//
//////////////////////////////////////////////////////////////////////////

namespace XSF
{
    public class Singleton<T> where T : Singleton<T>, new()
    {
        private static T? s_Instance;
        public static T Instance => s_Instance ??= new T();
    }
}
