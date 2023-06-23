//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\Scripts\Common\AASUtil.cs
// 作者：Xoen Xie
// 时间：2023/06/21
// 描述：AAS通用函数，供lua调用
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using XLua;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[CSharpCallLua]
public delegate bool OnUILoadCall(GameObject go);

[LuaCallCSharp]
public static class AASUtil
{
    public static void LoadUI(string name, OnUILoadCall callback)
    {
        AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(name);
        handle.Completed += (op) =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject instance = op.Result;
                if(callback(op.Result))
                {
                    XSF.ReleaseAASGo(op.Result);
                }
            }
            else
            {
                XSF.LogError("AASUtil.LoadUI error, name=" + name);
            }
        };
    }

    public static void ReleaseInstance(GameObject go)
    {
        Addressables.ReleaseInstance(go);
    }
}