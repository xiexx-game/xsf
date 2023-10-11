//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Scripts/Level/Level.cs
// 作者：Xoen
// 时间：2023/08/25
// 描述：关卡
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;

public enum SceneObjID
{
    Character = 0,
    Scene,
    Max,
}

public class Level : Singleton<Level>
{
    public ScpLevel LevelConfig { get; private set; }

    public LevelDetail Detail { get; private set; }

    public MonoLevel LevelData { get; set; }

    public MonoCharacter Character { get; set; }

    private GameObject[] m_Objs;

    public GameObject GetGameObject(SceneObjID id)
    {
        return m_Objs[(int)id];
    }

    public void SetGameObject(SceneObjID id, GameObject obj)
    {
        m_Objs[(int)id] = obj;
    }

    public void Init()
    {
        m_Objs = new GameObject[(int)SceneObjID.Max];
    }

    public bool Start(uint nLevel)
    {
        LevelConfig = XSFSchema.Instance.Get<SchemaLevel>((int)SchemaID.Level).Get(nLevel) as ScpLevel;
        if(LevelConfig == null)
        {
            Debug.LogError("Level SetLevel LevelConfig == null, level=" + nLevel );
            return false;
        }

        if(nLevel == 0)
        {
            Detail = new LevelDetailLobby();
        }
        else
        {
            Detail = new LevelDetailNormal();
        }

        Detail.Start();

        return true;
    }

    public void OnUpdate()
    {

    }
}