//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Scripts/Loading/LoadingGameObject.cs
// 作者：Xoen
// 时间：2023/08/25
// 描述：加载GameObject
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public enum LoadingID
{
    None = 0,

    Max,
}

public interface ILoadingHandler
{
    void OnGoLoadingDone(int id, GameObject go);
}

public class LoadingGameObject : LoadingBase
{
    public int LoadingID;
    public string AASName;
    public ILoadingHandler Handler;

    public override bool IsDone { get { return m_isLoadDone;} }

    private bool m_isLoadDone;

    public override void Start()
    {
        var handle = Addressables.InstantiateAsync(AASName);
        handle.Completed += (op) =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                m_isLoadDone = true;
                Handler.OnGoLoadingDone(LoadingID, op.Result);
            }
            else
            {
                Debug.LogError("LoadingGameObject.Start error, name=" + AASName);
            }
        };
    }
}