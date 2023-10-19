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
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;

public enum SceneObjID
{
    Character = 0,
    Lobby,
    Playground,
    Select,
    Max,
}

public class Level : Singleton<Level>, ICharacterEvent, XSFAnimHandler
{
    protected enum RunStatus
    {
        None = 0,
        LoadWait,
        ChangeWait,
        PlayWait,
    }

    private RunStatus m_nStatus;

    private int m_CharacterIndex;

    public ScpLevel LevelConfig { get; set; }

    public ScpLevel LobbyConfig { get; private set; }

    public MonoLevel LobbyData { get; set; }
    public MonoLevel PlayData { get; set; }

    public MonoCharacter Character { get; set; }

    private GameObject[] m_Objs;

    public GameObject GetGameObject(SceneObjID id)
    {
        return m_Objs[(int)id];
    }

    public void SetGameObject(SceneObjID id, GameObject obj)
    {
        if(m_Objs[(int)id] != null)
            Addressables.ReleaseInstance(m_Objs[(int)id]);

        m_Objs[(int)id] = obj;
    }

    public void Init()
    {
        m_Objs = new GameObject[(int)SceneObjID.Max];
        m_Selects = new Dictionary<int, GameObject>();
    }

    public bool Start()
    {
        LobbyConfig = XSFSchema.Instance.Get<SchemaLevel>((int)SchemaID.Level).Get(0) as ScpLevel;
        LevelConfig = XSFSchema.Instance.Get<SchemaLevel>((int)SchemaID.Level).Get(1) as ScpLevel;
        if(LobbyConfig == null)
        {
            Debug.LogError("Level SetLevel LobbyConfig == null, level=" + 0 );
            return false;
        }

        m_CharacterIndex = 2;
        LoadCharacter();
        m_nStatus = RunStatus.LoadWait;
        LoadAsset(SceneObjID.Select, "select");

        return true;
    }

    public void OnUpdate()
    {
        
    }

    void LoadAsset(SceneObjID id, string name)
    {
        var handle = Addressables.InstantiateAsync(name);
        handle.Completed += (op) =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject obj = op.Result;
                SetGameObject(id, obj);
                OnAssetLoadDone(id, obj);
            }
            else
            {
                Debug.LogError("LoadingGameObject.Start error, name=" + name);
            }
        };
    }

    private void OnAssetLoadDone(SceneObjID id, GameObject go) 
    {
        switch(id)
        {
        case SceneObjID.Character:
            {
                var character = go.GetComponent<MonoCharacter>();
                character.Init(this);
                character.Born(GameConfig.Instance.BornPos, GameConfig.Instance.EnterPos);

                Character = character;
            }
            break;

        case SceneObjID.Lobby:
            {
                var level = go.GetComponent<MonoLevel>();
                level.Init(this);
                level.PlayAnim();
                LobbyData = level;
            }
            break;

        case SceneObjID.Playground:
            {
                var level = go.GetComponent<MonoLevel>();
                level.Init(this);
                PlayData = level;
                go.SetActive(false);

                Character.PlayFastExit();
            }
            break;

        case SceneObjID.Select:
            {
                go.SetActive(false);
            }
            break;
        }
    }

    private void LoadCharacter()
    {
        var schema = XSFSchema.Instance.Get<SchemaGlobal>((int)SchemaID.Global) as SchemaGlobal;
        var array = schema.Get((uint)GlobalID.CharacterList).data as CSVData_SArray;
        if(m_CharacterIndex >= array.sarValue.Length)
        {
            m_CharacterIndex = 0;
        }

        LoadAsset(SceneObjID.Character, array.sarValue[m_CharacterIndex]);
        m_CharacterIndex ++;
    }

    public void ChangeCharacter() 
    {
        if(m_nStatus == RunStatus.None)
        {
            Character.Exit(GameConfig.Instance.ExitPos);
            m_nStatus = RunStatus.ChangeWait;
        }
    }

    public void Play()
    {
        if(m_nStatus == RunStatus.None)
        {
            Character.PlayExit(GameConfig.Instance.ExitPos);
            m_nStatus = RunStatus.PlayWait;
            LoadAsset(SceneObjID.Playground, LevelConfig.sSceneObj);
        }
    }

    public void OnEnterDone()
    {
        if(m_nStatus == RunStatus.ChangeWait)
        {
            m_nStatus = RunStatus.None;
        }
        if(m_nStatus == RunStatus.PlayWait)
        {
            CreateLevelBlock();
        }
        else
        {
            LoadAsset(SceneObjID.Lobby, LobbyConfig.sSceneObj);
        }
    }

    private SingleBlock[] m_Blocks;
    private Dictionary<int, GameObject> m_Selects;
    private void CreateLevelBlock()
    {
        int nObjIndex = 0;
        m_Blocks = LevelDef.CreateBlocks((int)LevelConfig.uRowCount, (int)LevelConfig.uColCount, PlayData.Map.transform, PlayData.Block, 0.8f, false);
        for(int i = 0; i < LevelConfig.sarData.Length; i ++)
			{
				if(LevelConfig.sarData[i] == "#") 
				{
					//m_Blocks[i].SetColor(BlockColor.Wall);
				}
				else if(LevelConfig.sarData[i] == "-")
				{
					m_Blocks[i].SetColor(BlockColor.Road);
                    m_Blocks[i].Status = BlockStatus.Road;
				}
				else if(LevelConfig.sarData[i] == "@")
				{
					m_Blocks[i].SetColor(BlockColor.Road);
                    m_Blocks[i].Status = BlockStatus.Road | BlockStatus.Character;
                    Character.transform.position = m_Blocks[i].go.transform.position;
					
				}
				else if(LevelConfig.sarData[i] == ".")
				{
					m_Blocks[i].SetColor(BlockColor.Road);
                    m_Blocks[i].Status = BlockStatus.Road | BlockStatus.Point;
                    GameObject select = GameObject.Instantiate(m_Objs[(int)SceneObjID.Select]);
                    select.SetActive(true);
                    select.GetComponent<MonoSelect>().ShowSelect(1); 
                    m_Blocks[i].FX = select;
                    select.transform.position = m_Blocks[i].go.transform.position;
				}
				else if(LevelConfig.sarData[i] == "$")
				{
                    m_Blocks[i].SetColor(BlockColor.Road);
                    m_Blocks[i].Status = BlockStatus.Road | BlockStatus.Box;

                    GameObject select = GameObject.Instantiate(m_Objs[(int)SceneObjID.Select]);
                    select.name = "fx";
                    select.SetActive(true);
                    select.GetComponent<MonoSelect>().ShowSelect(0); 
                    select.transform.SetParent(PlayData.Objs[nObjIndex].transform);
                    float scale = PlayData.ObjFXScale[nObjIndex];
                    select.transform.localScale = new Vector3(scale, scale, scale);
                    select.transform.localPosition = PlayData.ObjFXPos[nObjIndex++];
				}
				else if(LevelConfig.sarData[i] == "*")
				{
                    m_Blocks[i].SetColor(BlockColor.Road);
                    m_Blocks[i].Status = BlockStatus.Road | BlockStatus.Box | BlockStatus.Point; 

                    GameObject select = GameObject.Instantiate(m_Objs[(int)SceneObjID.Select]);
                    select.transform.position = m_Blocks[i].go.transform.position;
                    select.SetActive(true);
                    select.GetComponent<MonoSelect>().ShowSelect(1); 
                    m_Blocks[i].FX = select;

                    select = GameObject.Instantiate(m_Objs[(int)SceneObjID.Select]);
                    select.name = "fx";
                    
                    select.transform.SetParent(PlayData.Objs[nObjIndex].transform);
                    float scale = PlayData.ObjFXScale[nObjIndex];
                    select.transform.localScale = new Vector3(scale, scale, scale);
                    select.transform.localPosition = PlayData.ObjFXPos[nObjIndex++];
				}
			}
    }

    public void OnExitDone()
    {
        Debug.LogError("OnExitDone 111111111");
        if(m_nStatus == RunStatus.ChangeWait)
        {
            Debug.LogError("OnExitDone 333");
            LoadCharacter();
        }
        else if(m_nStatus == RunStatus.PlayWait)
        {
            Debug.LogError("OnExitDone 22222");
            LobbyData.PlayReverse();
            XSFMain.Instance.MainCamera.GetComponent<CameraMove>().MoveTo(PlayData.CamaraT);
        }
    }

    public void OnAnimFinish(GameObject obj, string param)
    {
        if(param == "LobbyShow")
        {
            if(m_nStatus == RunStatus.LoadWait)
            {
                XSFUI.Instance.ShowUI(LobbyConfig.iUIID);
                m_nStatus = RunStatus.None;
            }
        }
        else if(param == "LobbyHide")
        {
            if(m_nStatus == RunStatus.PlayWait)
            {
                PlayData.gameObject.SetActive(true);
                PlayData.PlayAnim();
            }
        }
        else if(param == "LevelShow")
        {
            Character.Born(PlayData.BornPos, PlayData.EnterPos);
        }
    }
}