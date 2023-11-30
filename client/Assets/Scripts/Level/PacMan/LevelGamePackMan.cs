//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Scripts/Level/PacMan/LevelGamePackMan.cs
// 作者：Xoen
// 时间：2023/09/13
// 描述：关卡 吃豆人
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;


public class LevelGamePackMan : LevelGame, ILoadingHandler
{   
    enum ObjID
    {
        Scene = 0,
        Ghost,
        PacMan,
        SmallBeans,
        BigBeans,
        Max,
    }

    enum GameStatus
    {
        None = 0,

        Play,
        End,
    }

    enum MoveResult
    {
        None = 0,
        GameEnd,
        EatFood,
    }

    public const int ROW_COUNT = 36;
    public const int COL_COUNT = 28;

    private ScpLevelGame m_GameScp;

    private GameObject[] m_SceneObj;

    private SingleBlock[] m_Blocks;

    private GameStatus m_nStatus;

    public ScpPacManLevels ScpLevels { get; private set; }


    public override void Init()
    {
        m_SceneObj = new GameObject[(int)ObjID.Max];
    }

    public override void Load()
    {
        m_GameScp = XSFSchema.Instance.Get<SchemaLevelGame>((int)SchemaID.LevelGame).Get((uint)LevelGameType.PacMan);
        if(m_GameScp == null)
        {
            Debug.LogError("LevelGamePackMan Load m_GameScp is null");
            return;
        }

        ScpLevels = XSFSchema.Instance.Get<SchemaPacManLevels>((int)SchemaID.PacManLevels).Get(CurrentLevel);
        if(ScpLevels == null)
        {
            Debug.LogError("LevelGamePackMan Load ScpLevels is null");
            return;
        }

        var load = new LoadingGameObject();
        load.Handler = this;
        load.LoadingID = (int)ObjID.Scene;
        load.AASName = m_GameScp.sarSceneObjects[load.LoadingID];
        LoadingManager.Instance.AddLoading(load);

        // load = new LoadingGameObject();
        // load.Handler = this;
        // load.LoadingID = (int)ObjID.Block;
        // load.AASName = m_GameScp.sarSceneObjects[load.LoadingID];
        // LoadingManager.Instance.AddLoading(load);

        for(int i = 0; i < m_GameScp.arShowUIs.Length; i ++)
        {
            var loadUI = new LoadingUI();
            loadUI.UI = (int)m_GameScp.arShowUIs[i];
            LoadingManager.Instance.AddLoading(loadUI);
        }

        LoadingManager.Instance.AddLoading(new LoadingLevelGame());

        LoadingManager.Instance.Start();
    }

    public override void PreCreate()
    {
        MonoSnakeObjects mono = m_SceneObj[(int)ObjID.Scene].GetComponent<MonoSnakeObjects>();

        //m_Blocks = LevelDef.CreateBlocks(LevelGameType.Snake, ROW_COUNT, COL_COUNT, mono.Playground.transform, m_SceneObj[(int)ObjID.Block]);
    }

    public override void Enter()
    {
        GameSocre = 0;

        AudioManager.Instance.PlayBGM(BGMID.Snake);
    }

    public override void Exit()
    {
        GameSocre = 0;

        for(int i = 0; i < m_Blocks.Length; i ++)
        {
            GameObject.Destroy(m_Blocks[i].go);
            m_Blocks[i] = null;
        }
        
        for(int i = 0; i < m_SceneObj.Length; i ++)
        {
            if(m_SceneObj[i] != null)
                Addressables.ReleaseInstance(m_SceneObj[i]);
        }

        XSFUI.Instance.HideUI((int)UIID.UIPlay);
    }

    public SingleBlock GetBlock(int row, int col)
    {
        var index = LevelDef.GetBlockIndex(row, col, COL_COUNT);
        if(index < 0 || index >= m_Blocks.Length)
            return null;

        return m_Blocks[index];
    }

    public override void Restart()
    {
        for(int i = 0; i < m_Blocks.Length; i ++)
        {
            m_Blocks[i].Hide();
            m_Blocks[i].Status = BlockStatus.None;
        }

        Enter();
    }

    public override void OnUpdate()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Change();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveLeft();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveRight();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveDown();
        }
#endif

        switch(m_nStatus)
        {
        case GameStatus.Play:
            {
                
            }
            break;

        case GameStatus.End:
            {
                var ui = XSFUI.Instance.Get((int)UIID.UIPause);
		
                ui.Show();
                ui.Refresh((uint)UIRefreshID.SetEnd, null);
                m_nStatus = GameStatus.None;
            }
            break;
        }

    }

    public void OnGoLoadingDone(int id, GameObject go)
    {
        m_SceneObj[id] = go;
        
    }

    public override void MoveLeft()
    {
        if(m_nStatus != GameStatus.Play)
            return;
    }

    public override void MoveRight()
    {
        if(m_nStatus != GameStatus.Play)
            return;
    }

    public override void MoveDown()
    {
        if(m_nStatus != GameStatus.Play)
            return;

    }

    public override void Change()
    {
        if(m_nStatus != GameStatus.Play)
            return;
    }

    

    public override void Ultra()
    {

    }

    public override uint MaxLevel 
    { 
        get
        {
            return XSFSchema.Instance.Get<SchemaPacManLevels>((int)SchemaID.PacManLevels).MaxLevel;
        }
    }
}
