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
using System;

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
        Play,
        Moving,
        HomeWait,
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
        if(m_nStatus != RunStatus.Play)
            return;

        for(int i = 0; i < m_StatusCheck.Length; i ++)
        {
            if(m_StatusCheck[i] != m_Blocks[i].Status)
            {
                if((m_Blocks[i].Status & (int)BlockStatus.Point) == (int)BlockStatus.Point)
                {
                    if((m_Blocks[i].Status & (int)BlockStatus.Box) == (int)BlockStatus.Box)
                    {
                        m_Blocks[i].select.SetOK();
                        m_Blocks[i].box.select.Hide();
                    }
                    else
                    {
                        m_Blocks[i].select.ShowSelect(1);
                    }
                }
                else
                {
                    if((m_Blocks[i].Status & (int)BlockStatus.Box) == (int)BlockStatus.Box)
                    {
                        if(!m_Blocks[i].box.select.gameObject.activeSelf)
                        {
                            m_Blocks[i].box.select.ShowSelect(0);
                        }
                    }
                }

                m_StatusCheck[i] = m_Blocks[i].Status;
            }
        }

        bool IsWin = true;
        for(int i = 0; i < m_Blocks.Length; i ++)
        {
            if((m_Blocks[i].Status & (int)BlockStatus.Point) == (int)BlockStatus.Point)
            {
                if((m_Blocks[i].Status & (int)BlockStatus.Box) == (int)BlockStatus.Box)
                {

                }
                else
                {
                    IsWin = false;
                }
            }
        }

        if(IsWin)
        {
            m_nStatus = RunStatus.None;
            XSFUI.Instance.ShowUI((int)UIID.UIEnd);
        }
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
        else if(m_nStatus == RunStatus.PlayWait)
        {
            CreateLevelBlock();
            XSFUI.Instance.ShowUI((int)UIID.UIPlay);
            m_nStatus = RunStatus.Play;
        }
        else if(m_nStatus == RunStatus.Moving)
        {
            //Debug.LogError("2222222222222");
            m_nStatus = RunStatus.Play;
        }
        else if(m_nStatus == RunStatus.HomeWait)
        {
            XSFUI.Instance.ShowUI((int)UIID.UIMain);
            m_nStatus = RunStatus.None;
        }
        else
        {
            LoadAsset(SceneObjID.Lobby, LobbyConfig.sSceneObj);
        }
    }

    public void Redo()
    {
        if(m_nStatus != RunStatus.Play)
            return;

        int nObjIndex = 0;
        for(int i = 0; i < LevelConfig.sarData.Length; i ++)
        {
            m_Blocks[i].box = null;
            if(LevelConfig.sarData[i] == "#") 
            {
                m_Blocks[i].Status = (int)BlockStatus.Wall;
            }
            else if(LevelConfig.sarData[i] == "-")
            {
                m_Blocks[i].Status = (int)BlockStatus.Road;
            }
            else if(LevelConfig.sarData[i] == "@")
            {
                m_Blocks[i].Status = (int)BlockStatus.Road;
                Character.transform.position = m_Blocks[i].go.transform.position;
                Character.Row = m_Blocks[i].row;
                Character.Col = m_Blocks[i].col;

                //Debug.Log($"pos={Character.transform.position}, row={Character.Row}, col={Character.Col}");
            }
            else if(LevelConfig.sarData[i] == ".")
            {
                m_Blocks[i].Status = (int)BlockStatus.Road | (int)BlockStatus.Point;
                m_Blocks[i].select.ShowSelect(1);
            }
            else if(LevelConfig.sarData[i] == "$")
            {
                m_Blocks[i].Status = (int)BlockStatus.Road | (int)BlockStatus.Box; 
                m_Blocks[i].box = PlayData.Objs[nObjIndex].GetComponent<MonoBox>();
                m_Blocks[i].box.select.ShowSelect(0);
                PlayData.Objs[nObjIndex].transform.localPosition = m_Blocks[i].go.transform.localPosition;

                nObjIndex ++;
            }
            else if(LevelConfig.sarData[i] == "*")
            {
                m_Blocks[i].Status = (int)BlockStatus.Road | (int)BlockStatus.Box | (int)BlockStatus.Point; 
                PlayData.Objs[nObjIndex].transform.localPosition = m_Blocks[i].go.transform.localPosition;
                m_Blocks[i].box = PlayData.Objs[nObjIndex].GetComponent<MonoBox>();
                m_Blocks[i].box.select.Hide();

                m_Blocks[i].select.ShowSelect(1);
                m_Blocks[i].select.SetOK();

                nObjIndex ++;
            }

            m_StatusCheck[i] = m_Blocks[i].Status;
        }
    }

    private void MoveTo(int nRow, int nCol, int nRowNext, int nColNext)
    {
        //Debug.Log($"Character pos={Character.transform.position}, next row={nRow}, col={nCol}");

        int nIndex = LevelDef.GetBlockIndex(nRow, nCol, (int)LevelConfig.uColCount);
        var block = m_Blocks[nIndex];
        //Debug.Log("old block.Status=" + Convert.ToString(block.Status, 2).PadLeft(4, '0'));
        if((block.Status & (int)BlockStatus.Road) == (int)BlockStatus.Road)
        {
            // 如果这个地方有箱子，则推箱子动
            if((block.Status & (int)BlockStatus.Box) == (int)BlockStatus.Box)
            {
                int nNextIndex = LevelDef.GetBlockIndex(nRowNext, nColNext, (int)LevelConfig.uColCount);
                var blockNext = m_Blocks[nNextIndex];
                // 箱子的下一格只能是空白格或者是点位格，才能移动
                // 也就是说下一格有箱子，或者没路的，都不能移动
                //Debug.Log("old blockNext.Status=" + Convert.ToString(blockNext.Status, 2).PadLeft(4, '0'));
                if((blockNext.Status & (int)BlockStatus.Box) == (int)BlockStatus.Box || ((blockNext.Status & (int)BlockStatus.Wall) == (int)BlockStatus.Wall))
                {
                    //Debug.LogError("Cant Move ....");
                    return;
                }

                // 移动箱子
                Vector3 target = blockNext.go.transform.localPosition;
                target.y = block.box.transform.localPosition.y;
                block.box.Move(target);

                int nBoxFlag = ~(int)BlockStatus.Box;
                //Debug.Log("nBoxFlag=" + Convert.ToString(nBoxFlag, 2).PadLeft(4, '0'));
                block.Status = block.Status & nBoxFlag;
                //Debug.Log("new block.Status=" + Convert.ToString(block.Status, 2).PadLeft(4, '0'));
                blockNext.box = block.box;
                blockNext.Status = blockNext.Status | (int)BlockStatus.Box;
                //Debug.Log("new blockNext.Status=" + Convert.ToString(blockNext.Status, 2).PadLeft(4, '0'));
                block.box = null;
            }

            Character.Row = nRow;
            Character.Col = nCol;
            Debug.Log("Move to " + block.go.transform.position);
            Character.Run(block.go.transform.position);

            m_nStatus = RunStatus.Moving;
        }
        else
        {

        }
    }

    public void MoveUp()
    {
        if(m_nStatus != RunStatus.Play)
            return;

        int nRow = Character.Row - 1;
        int nNextRow = nRow - 1;

        MoveTo(nRow, Character.Col, nNextRow, Character.Col);
    }

    public void MoveRight()
    {
        if(m_nStatus != RunStatus.Play)
            return;

        int nCol = Character.Col + 1;
        int nNextCol = nCol + 1;

        MoveTo(Character.Row, nCol, Character.Row, nNextCol);
    }

    public void MoveDown()
    {
        if(m_nStatus != RunStatus.Play)
            return;

        int nRow = Character.Row + 1;
        int nNextRow = nRow + 1;

        MoveTo(nRow, Character.Col, nNextRow, Character.Col);
    }

    public void MoveLeft()
    {
        if(m_nStatus != RunStatus.Play)
            return;

        int nCol = Character.Col - 1;
        int nNextCol = nCol - 1;

        MoveTo(Character.Row, nCol, Character.Row, nNextCol);
    }

    public void GoHome()
    {
        if(m_nStatus == RunStatus.None || m_nStatus == RunStatus.Play)
        {
            for(int i = 0; i < m_Blocks.Length; i ++)
            {
                m_Blocks[i].Clear();
            }

            XSFUI.Instance.HideUI((int)UIID.UIPlay);

            Character.Exit(PlayData.ExitPos);
            m_nStatus = RunStatus.HomeWait;
        }
    }

    private SingleBlock[] m_Blocks;
    private int [] m_StatusCheck;
    private Dictionary<int, GameObject> m_Selects;
    private void CreateLevelBlock()
    {
        int nObjIndex = 0;
        m_Blocks = LevelDef.CreateBlocks((int)LevelConfig.uRowCount, (int)LevelConfig.uColCount, PlayData.Map.transform, PlayData.Block, 0.8f, false);
        m_StatusCheck = new int[m_Blocks.Length];
        for(int i = 0; i < LevelConfig.sarData.Length; i ++)
        {
            if(LevelConfig.sarData[i] == "#") 
            {
                //m_Blocks[i].SetColor(BlockColor.Wall);
                m_Blocks[i].Status = (int)BlockStatus.Wall;
            }
            else if(LevelConfig.sarData[i] == "-")
            {
                m_Blocks[i].SetColor(BlockColor.Road);
                m_Blocks[i].Status = (int)BlockStatus.Road;
            }
            else if(LevelConfig.sarData[i] == "@")
            {
                m_Blocks[i].SetColor(BlockColor.Road);
                m_Blocks[i].Status = (int)BlockStatus.Road;
                Character.transform.position = m_Blocks[i].go.transform.position;
                Character.Row = m_Blocks[i].row;
                Character.Col = m_Blocks[i].col;

                //Debug.Log($"pos={Character.transform.position}, row={Character.Row}, col={Character.Col}");
            }
            else if(LevelConfig.sarData[i] == ".")
            {
                m_Blocks[i].SetColor(BlockColor.Road);
                m_Blocks[i].Status = (int)BlockStatus.Road | (int)BlockStatus.Point;
                GameObject select = GameObject.Instantiate(m_Objs[(int)SceneObjID.Select]);
                m_Blocks[i].select = select.GetComponent<MonoSelect>();
                m_Blocks[i].select.ShowSelect(1);
                select.transform.position = m_Blocks[i].go.transform.position;
            }
            else if(LevelConfig.sarData[i] == "$")
            {
                m_Blocks[i].SetColor(BlockColor.Road);
                m_Blocks[i].Status = (int)BlockStatus.Road | (int)BlockStatus.Box;
                m_Blocks[i].box = PlayData.Objs[nObjIndex].GetComponent<MonoBox>();

                GameObject select = GameObject.Instantiate(m_Objs[(int)SceneObjID.Select]);
                select.name = "fx";
                m_Blocks[i].box.select = select.GetComponent<MonoSelect>();
                m_Blocks[i].box.select.ShowSelect(0); 

                select.transform.SetParent(PlayData.Objs[nObjIndex].transform);
                float scale = PlayData.ObjFXScale[nObjIndex];
                select.transform.localScale = new Vector3(scale, scale, scale);
                select.transform.localPosition = PlayData.ObjFXPos[nObjIndex++];
            }
            else if(LevelConfig.sarData[i] == "*")
            {
                m_Blocks[i].SetColor(BlockColor.Road);
                m_Blocks[i].Status = (int)BlockStatus.Road | (int)BlockStatus.Box | (int)BlockStatus.Point; 
                m_Blocks[i].box = PlayData.Objs[nObjIndex].GetComponent<MonoBox>();

                GameObject select = GameObject.Instantiate(m_Objs[(int)SceneObjID.Select]);
                select.transform.position = m_Blocks[i].go.transform.position;
                m_Blocks[i].select = select.GetComponent<MonoSelect>();
                m_Blocks[i].select.ShowSelect(1);
                m_Blocks[i].select.SetOK();

                select = GameObject.Instantiate(m_Objs[(int)SceneObjID.Select]);
                select.name = "fx";
                m_Blocks[i].box.select = select.GetComponent<MonoSelect>();
                m_Blocks[i].box.select.Hide();
                
                select.transform.SetParent(PlayData.Objs[nObjIndex].transform);
                float scale = PlayData.ObjFXScale[nObjIndex];
                select.transform.localScale = new Vector3(scale, scale, scale);
                select.transform.localPosition = PlayData.ObjFXPos[nObjIndex++];
            }

            m_StatusCheck[i] = m_Blocks[i].Status;
        }
    }

    public void OnExitDone()
    {
        Character.gameObject.SetActive(false);
        if(m_nStatus == RunStatus.ChangeWait)
        {
            LoadCharacter();
        }
        else if(m_nStatus == RunStatus.PlayWait)
        {
            LobbyData.PlayReverse();
        }
        else if(m_nStatus == RunStatus.HomeWait)
        {
            PlayData.PlayReverse();
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
            else if(m_nStatus == RunStatus.HomeWait)
            {
                Character.Born(LobbyData.BornPos, LobbyData.EnterPos);
                Addressables.ReleaseInstance(m_Objs[(int)SceneObjID.Playground]);
            }
        }
        else if(param == "LobbyHide")
        {
            if(m_nStatus == RunStatus.PlayWait)
            {
                LobbyData.gameObject.SetActive(false);

                PlayData.gameObject.SetActive(true);
                PlayData.PlayAnim();
            }
        }
        else if(param == "LevelShow")
        {
            if(m_nStatus == RunStatus.PlayWait)
            {
                Character.Born(PlayData.BornPos, PlayData.EnterPos);
            }
        }
        else if(param == "LevelHide")
        {
            if(m_nStatus == RunStatus.HomeWait)
            {
                LobbyData.gameObject.SetActive(true);
                LobbyData.PlayAnim();
            }
        }
        else if(param == "LevelCamera")
        {
            if(m_nStatus == RunStatus.PlayWait)
            {
                XSFMain.Instance.MainCamera.GetComponent<CameraMove>().MoveTo(PlayData.CamaraT);
            }
        }
        else if(param == "LobbyCamera")
        {
            if(m_nStatus == RunStatus.HomeWait)
            {
                XSFMain.Instance.MainCamera.GetComponent<CameraMove>().MoveTo(LobbyData.CamaraT);
            }
        }
    }
}