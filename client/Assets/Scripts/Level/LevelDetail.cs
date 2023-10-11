//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Scripts/Level/LevelDetail.cs
// 作者：Xoen
// 时间：2023/08/25
// 描述：关卡详情
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;

using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public abstract class LevelDetail
{
    

    protected enum RunStatus
    {
        None = 0,
        LoadCharacter,
        LoadScene,
        ShowUI,
        Wait,
        Change,
    }

    protected RunStatus m_nStatus;

    public virtual void Start()
    {
        
    }

    public virtual void End()
    {

    }

    private void LoadAsset(SceneObjID id, string name)
    {
        var handle = Addressables.InstantiateAsync(name);
        handle.Completed += (op) =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject obj = op.Result;
                
                var t = obj.transform;
                Level.Instance.SetGameObject(id, obj);

                switch(id)
                {
                case SceneObjID.Character:
                    {
                        var mono = t.GetComponent<MonoCharacter>();
                        //mono.Init(this);
                        mono.Born();
                    }
                    
                    break;

                case SceneObjID.Scene:
                    {
                        var mono = t.GetComponent<MonoLevel>();
                        //mono.Init(this);
                        mono.PlayAnim();
                    }
                    break;
                }
            }
            else
            {
                Debug.LogError("LoadingGameObject.Start error, name=" + name);
            }
        };
    }

    public virtual void OnUpdate()
    {

    }
}

public class LevelDetailLobby : LevelDetail
{
    public override void Start()
    {
        base.Start();


    }


}

public class LevelDetailNormal : LevelDetail
{
    
}
