//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\XSF\Base\MonoSingleton.cs
// 作者：Xoen Xie
// 时间：2023/06/15
// 描述：Mono脚本单例
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T s_Instance;
    public static T Instance => s_Instance;

    protected virtual void Awake()
    {
        s_Instance = (T)this;
    }
}
