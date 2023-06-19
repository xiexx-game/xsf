//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\XSF\Component\MonoCoroutine.cs
// 作者：Xoen Xie
// 时间：2022/06/16
// 描述：协程脚本
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;

public enum CoroutineID
{
    C0 = 0,
    C1,
    C2,
    Max,
}

public class MonoCoroutine : MonoBehaviour
{
    public CoroutineID ID;
}
