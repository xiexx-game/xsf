//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Scripts/Level/Snake/LevelGameSnake.cs
// 作者：Xoen
// 时间：2023/09/13
// 描述：关卡 贪吃蛇游戏
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

public enum SnakeMoveDir 
{
    None = 0,
    Up,
    Down,
    Left,
    Right,
}

public class SnakeNode 
{
    public SingleBlock block;

    public SnakeMoveDir moveDir;

    public SnakeNode next;
}


public class LevelGameSnake : LevelGame, ILoadingHandler
{   
    enum ObjID
    {
        Scene = 0,
        Block,
        Max,
    }

    enum GameStatus
    {
        None = 0,
        CreateSnake,
        CreateFood,
        Play,
        End,
    }

    enum MoveResult
    {
        None = 0,
        GameEnd,
        EatFood,
    }

    public const int ROW_COUNT = 15;
    public const int COL_COUNT = 12;

    private ScpLevelGame m_GameScp;

    private GameObject[] m_SceneObj;

    private SingleBlock[] m_Blocks;

    private float m_fMoveTime;


    private uint m_nCurHighScore;

    private GameStatus m_nStatus;

    public ScpSnakeLevels ScpLevels {get; private set;}

    private const int SNAKE_DEFULAT_SIZE = 4;

    private SnakeMoveDir m_CurrentDir;

    private SnakeNode m_SnakeHead;
    private SnakeNode m_SnakeTail;

    private SnakeNode m_PreNode; 

    public override void Init()
    {
        m_SceneObj = new GameObject[(int)ObjID.Max];
        m_PreNode = new SnakeNode();
    }

    public override void Load()
    {
        m_GameScp = XSFSchema.Instance.Get<SchemaLevelGame>((int)SchemaID.LevelGame).Get((uint)LevelGameType.Snake);
        if(m_GameScp == null)
        {
            Debug.LogError("LevelGameSnake Load m_GameScp is null");
            return;
        }

        ScpLevels = XSFSchema.Instance.Get<SchemaSnakeLevels>((int)SchemaID.SnakeLevels).Get(CurrentLevel);
        if(ScpLevels == null)
        {
            Debug.LogError("LevelGameSnake Load ScpLevels is null");
            return;
        }

        var load = new LoadingGameObject();
        load.Handler = this;
        load.LoadingID = (int)ObjID.Scene;
        load.AASName = m_GameScp.sarSceneObjects[load.LoadingID];
        LoadingManager.Instance.AddLoading(load);

        load = new LoadingGameObject();
        load.Handler = this;
        load.LoadingID = (int)ObjID.Block;
        load.AASName = m_GameScp.sarSceneObjects[load.LoadingID];
        LoadingManager.Instance.AddLoading(load);

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

        m_Blocks = LevelDef.CreateBlocks(LevelGameType.Snake, ROW_COUNT, COL_COUNT, mono.Playground.transform, m_SceneObj[(int)ObjID.Block]);
    }

    public override void Enter()
    {
        GameSocre = 0;
        m_nStatus = GameStatus.CreateSnake;
        m_fMoveTime = 0;
        m_nCurHighScore = Level.Instance.GetHighScore((int)LevelGameType.Snake);

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

        for(int i = 0; i < m_GameScp.arShowUIs.Length; i ++)
        {
            XSFUI.Instance.CloseUI((int)m_GameScp.arShowUIs[i]);
        }
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
        m_SnakeHead = null;
        m_SnakeTail = null;

        for(int i = 0; i < m_Blocks.Length; i ++)
        {
            m_Blocks[i].Hide();
            m_Blocks[i].Status = BlockStatus.None;
        }

        Enter();
    }

    public override void OnUpdate()
    {
        switch(m_nStatus)
        {
        case GameStatus.CreateSnake:
            {
                CreateSnake();
                m_nStatus = GameStatus.CreateFood;
            }
            break;

        case GameStatus.CreateFood:
            {
                CreateFood();
                m_nStatus = GameStatus.Play;
            }
            break;

        case GameStatus.Play:
            {
                m_fMoveTime += Time.deltaTime;
                if(m_fMoveTime >= ScpLevels.fMoveInterval)
                {
                    m_fMoveTime = 0;
                    var result = SnakeMove();
                    switch(result)
                    {
                    case MoveResult.GameEnd:
                        m_nStatus = GameStatus.End;
                        break;

                    case MoveResult.EatFood:
                        AudioManager.Instance.PlayFXAudio(ClipID.Cycle);
                        AddNode();
                        GameSocre += ScpLevels.uFoodScore;

                        var ui = XSFUI.Instance.Get((int)UIID.UIPlaySnake);
                        ui.Refresh((uint)UIRefreshID.PlayScore, null);

                        if(GameSocre > m_nCurHighScore)
                        {
                            m_nCurHighScore = GameSocre;
                            Level.Instance.SetHighScore((int)LevelGameType.Snake, GameSocre);
                        }

                        uint level = XSFSchema.Instance.Get<SchemaSnakeLevels>((int)SchemaID.SnakeLevels).GetLevel(GameSocre);
                        if(level > CurrentLevel)
                        {
                            CurrentLevel = level;
                            ScpLevels = XSFSchema.Instance.Get<SchemaSnakeLevels>((int)SchemaID.SnakeLevels).Get(CurrentLevel);
                            ui.Refresh((uint)UIRefreshID.PlayLevel, null);
                            ui.Refresh((uint)UIRefreshID.ShowFireworks, null);
                            AudioManager.Instance.PlayFXAudio(ClipID.HighScore);
                        }

                        SnakeNode node = m_SnakeHead;
                        while(node != null)
                        {
                            node.block.block.DoShimmer();
                            node = node.next;
                        }

                        CreateFood();
                        break;
                    }
                }
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

    private void AddNode()
    {
        var color = m_SnakeHead.block.block.ColorIndex;

        SnakeNode node = new SnakeNode();
        node.block = m_PreNode.block;
        node.block.Status = BlockStatus.Block;
        node.block.SetSnakeNode(color);
        
        m_SnakeTail.next = node;
        m_SnakeTail = node;
    }

    private MoveResult SnakeMove()
    {
        SnakeNode node = m_SnakeHead;
        MoveResult result = MoveResult.None;

        m_PreNode.block = null;
        m_PreNode.moveDir = SnakeMoveDir.None;

        //Debug.Log("SnakeMove start  ...");

        while(true)
        {
            SingleBlock newBlock = null;
            if(m_PreNode.block == null)
            {
                int row = node.block.row;
                int col = node.block.col;

                //Debug.Log($"SnakeMove row={row}, col={col}");

                switch(node.moveDir)
                {
                case SnakeMoveDir.Up:
                    row --;
                    break;

                case SnakeMoveDir.Down:
                    row ++;
                    break;

                case SnakeMoveDir.Left:
                    col --;
                    break;

                case SnakeMoveDir.Right:
                    col ++;
                    break;
                }

                if(row < 0 || row >= ROW_COUNT || col < 0 || col >= COL_COUNT)  // 撞墙了
                {
                    //Debug.Log($"SnakeMove game end, row={row}, col={col}");
                    return MoveResult.GameEnd;
                }

                newBlock = GetBlock(row, col);
                if(newBlock.Status == BlockStatus.Block)
                {
                    return MoveResult.GameEnd;
                }
                else if(newBlock.Status == BlockStatus.Food)
                {
                    result = MoveResult.EatFood;
                }
            } 
            else 
            {
                newBlock = m_PreNode.block;
            }
            
            var curColor = node.block.block.ColorIndex;
            node.block.Hide();
            node.block.Status = BlockStatus.None;

            m_PreNode.block = node.block;
            m_PreNode.moveDir = node.moveDir;

            node.block = newBlock;
            node.block.SetSnakeNode(curColor);
            node.block.Status = BlockStatus.Block;
            node.moveDir = m_PreNode.moveDir;

            if(node.next == null)
                break;
             
            node = node.next;
        }

        return result;
    }

    private void CreateFood()
    {
        do
        {
            int row = UnityEngine.Random.Range(0, ROW_COUNT);
            int col = UnityEngine.Random.Range(0, COL_COUNT);

            SingleBlock block = GetBlock(row, col);
            if(block.Status == BlockStatus.Block)
            {
                continue;
            }

            block.SetSnakeFood();
            block.Status = BlockStatus.Food;
            return; 

        }while(true);
    }

    private void CreateSnake()
    {
        int current = UnityEngine.Random.Range(0, 100);
        if(current < 25)
            m_CurrentDir = SnakeMoveDir.Up;
        else if( current < 50)
            m_CurrentDir = SnakeMoveDir.Down;
        else if(current < 75)
            m_CurrentDir = SnakeMoveDir.Left;
        else
            m_CurrentDir = SnakeMoveDir.Right;

        var color = UnityEngine.Random.Range(0, (int)BlockColor.Max);

        switch(m_CurrentDir)
        {
        case SnakeMoveDir.Up:
            {
                int nRowStart = ROW_COUNT - 5;
                int nColStart = COL_COUNT/2;

                for(int i = 0; i < SNAKE_DEFULAT_SIZE; i ++)
                {
                    Debug.Log("Create Snake node ...");
                    var node = new SnakeNode();
                    node.block = GetBlock(nRowStart, nColStart);
                    node.block.SetSnakeNode(color);
                    node.block.Status = BlockStatus.Block;
                    node.moveDir = m_CurrentDir;

                    if(m_SnakeHead == null) 
                    {
                        m_SnakeHead = node;
                        m_SnakeTail = node;
                    }
                    else
                    {
                        m_SnakeTail.next = node;
                        m_SnakeTail = node;
                    }

                    nRowStart ++;
                }
            }
            break;

        case SnakeMoveDir.Down:
            {
                int nRowStart = 4;
                int nColStart = COL_COUNT/2;

                for(int i = 0; i < SNAKE_DEFULAT_SIZE; i ++)
                {
                    var node = new SnakeNode();
                    node.block = GetBlock(nRowStart, nColStart);
                    node.block.SetSnakeNode(color);
                    node.block.Status = BlockStatus.Block;
                    node.moveDir = m_CurrentDir;

                    if(m_SnakeHead == null) 
                    {
                        m_SnakeHead = node;
                        m_SnakeTail = node;
                    }
                    else
                    {
                        m_SnakeTail.next = node;
                        m_SnakeTail = node;
                    }

                    nRowStart --;
                }
            }
            break;

        case SnakeMoveDir.Left:
            {
                int nRowStart = ROW_COUNT/2;
                int nColStart = COL_COUNT-5;

                for(int i = 0; i < SNAKE_DEFULAT_SIZE; i ++)
                {
                    var node = new SnakeNode();
                    node.block = GetBlock(nRowStart, nColStart);
                    node.block.SetSnakeNode(color);
                    node.block.Status = BlockStatus.Block;
                    node.moveDir = m_CurrentDir;

                    if(m_SnakeHead == null) 
                    {
                        m_SnakeHead = node;
                        m_SnakeTail = node;
                    }
                    else
                    {
                        m_SnakeTail.next = node;
                        m_SnakeTail = node;
                    }

                    nColStart ++;
                }
            }
            break;

        case SnakeMoveDir.Right:
            {
                int nRowStart = ROW_COUNT/2;
                int nColStart = 4;

                for(int i = 0; i < SNAKE_DEFULAT_SIZE; i ++)
                {
                    var node = new SnakeNode();
                    node.block = GetBlock(nRowStart, nColStart);
                    node.block.SetSnakeNode(color);
                    node.block.Status = BlockStatus.Block;
                    node.moveDir = m_CurrentDir;

                    if(m_SnakeHead == null) 
                    {
                        m_SnakeHead = node;
                        m_SnakeTail = node;
                    }
                    else
                    {
                        m_SnakeTail.next = node;
                        m_SnakeTail = node;
                    }

                    nColStart --;
                }
            }
            break;
        }
    }

    public void OnGoLoadingDone(int id, GameObject go)
    {
        m_SceneObj[id] = go;
        if(id == (int)ObjID.Block) 
        {
            m_SceneObj[id].SetActive(false);
        }
    }

    public override void DoLeft()
    {
        if(m_nStatus != GameStatus.Play)
            return;

        if(m_SnakeHead.moveDir != SnakeMoveDir.Right && m_SnakeHead.moveDir != SnakeMoveDir.Left)
            m_SnakeHead.moveDir = SnakeMoveDir.Left;
    }

    public override void DoRight()
    {
        if(m_nStatus != GameStatus.Play)
            return;

        if(m_SnakeHead.moveDir != SnakeMoveDir.Left && m_SnakeHead.moveDir != SnakeMoveDir.Right)
            m_SnakeHead.moveDir = SnakeMoveDir.Right;
    }

    public override void DoDown()
    {
        if(m_nStatus != GameStatus.Play)
            return;

        if(m_SnakeHead.moveDir != SnakeMoveDir.Up && m_SnakeHead.moveDir != SnakeMoveDir.Down)
            m_SnakeHead.moveDir = SnakeMoveDir.Down;
    }

    public override void DoUp()
    {
        if(m_nStatus != GameStatus.Play)
            return;

        if(m_SnakeHead.moveDir != SnakeMoveDir.Down && m_SnakeHead.moveDir != SnakeMoveDir.Up)
            m_SnakeHead.moveDir = SnakeMoveDir.Up;
    }

    

    public override void DoUltra()
    {

    }

    public override uint MaxLevel 
    { 
        get
        {
            return XSFSchema.Instance.Get<SchemaSnakeLevels>((int)SchemaID.SnakeLevels).MaxLevel;
        }
    }
}
