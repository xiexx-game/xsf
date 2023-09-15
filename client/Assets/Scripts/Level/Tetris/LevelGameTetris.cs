//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Scripts/Level/Tetris/LevelGameTetris.cs
// 作者：Xoen
// 时间：2023/08/25
// 描述：关卡 俄罗斯方块游戏
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

public class FullRowData
{
    public int row;
    public int colLeft;
    public int colRight;
}

public class LevelGameTetris : LevelGame, ILoadingHandler, IBlockDisappearEvent
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
        CreateTetris,
        Play,
        FullCheck,
        Wait,
        End,
    }

    public const int ROW_COUNT = 15;
    public const int COL_COUNT = 12;

    private ScpLevelGame m_GameScp;

    private GameObject[] m_SceneObj;

    private PreBlocks m_PreBlocks;

    private SingleBlock[] m_Blocks;

    private GameStatus m_nStatus;
    private float m_fDownTime;

    public ScpTetrisLevels ScpLevels { get; private set;}

    private TetrisData m_CurrentTetris;

    private List<SingleBlock> m_TetrisBlock;
    private List<int> m_FullRowData;

    private int DisappearCount;

    private uint m_nRowScore;
    private uint m_nScoreAddtion;

    private uint m_nCurHighScore;

    public override void Init()
    {
        m_SceneObj = new GameObject[(int)ObjID.Max];
        m_TetrisBlock = new List<SingleBlock>();
        m_FullRowData = new List<int>();
    }

    public override void Load()
    {
        m_GameScp = XSFSchema.Instance.Get<SchemaLevelGame>((int)SchemaID.LevelGame).Get((uint)LevelGameType.Tetris);
        if(m_GameScp == null)
        {
            Debug.LogError("LevelGameTetris Load m_GameScp is null");
            return;
        }

        ScpLevels = XSFSchema.Instance.Get<SchemaTetrisLevels>((int)SchemaID.TetrisLevels).Get(CurrentLevel);
        if(ScpLevels == null)
        {
            Debug.LogError("LevelGameTetris Load ScpLevels is null");
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
        MonoTetrisObjects mono = m_SceneObj[(int)ObjID.Scene].GetComponent<MonoTetrisObjects>();
        m_PreBlocks = new PreBlocks();
        m_PreBlocks.Create(mono.PreBlock, m_SceneObj[(int)ObjID.Block]);

        m_Blocks = LevelDef.CreateBlocks(LevelGameType.Tetris, ROW_COUNT, COL_COUNT, mono.Playground.transform, m_SceneObj[(int)ObjID.Block]);
    }

    public override void Enter()
    {
        GameSocre = 0;
        m_nStatus = GameStatus.CreateTetris;

        var schema = XSFSchema.Instance.Get<SchemaGlobal>((int)SchemaID.Global);
        var rowScoreScp = schema.Get((int)GlobalID.RowScore).data as CSVData_Uint;
        m_nRowScore = rowScoreScp.uValue;

        var scoreAdditionScp = schema.Get((int)GlobalID.ScoreAddition).data as CSVData_Uint;
        m_nScoreAddtion = scoreAdditionScp.uValue;

        m_nCurHighScore = Level.Instance.GetHighScore((int)LevelGameType.Tetris);

        AudioManager.Instance.PlayBGM(BGMID.Tetris);

        //Debug.LogError($"m_nRowScore={m_nRowScore}, m_nScoreAddtion={m_nScoreAddtion}");
    }

    public override void Exit()
    {
        GameSocre = 0;
        m_PreBlocks.Release();

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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Ultra();
        }
#endif

        switch (m_nStatus)
        {
        case GameStatus.CreateTetris:
            m_CurrentTetris = m_PreBlocks.GetNextTetris();
            m_nStatus = GameStatus.Play;
            m_TetrisBlock.Clear();
            m_fDownTime = 100;
            break;

        case GameStatus.Play:
            m_fDownTime += Time.deltaTime;
            if(m_fDownTime >= ScpLevels.fDownInterval)
            {
                if(!TetrisMoveDown())
                {
                    if(m_CurrentTetris.MoveCount <= 2)
                    {
                        ShowEndTetris();
                        m_nStatus = GameStatus.End;
                    }
                    else
                    {
                        m_nStatus = GameStatus.FullCheck;
                    }
                    
                }
                m_fDownTime = 0;
            }
            break;

        case GameStatus.FullCheck:
            
            if(FullCheck())
            {
                //Debug.LogError("FullCheck wait ...");
                m_nStatus = GameStatus.Wait;
            }
            else
            {
                m_nStatus = GameStatus.CreateTetris;
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

    private bool FullCheck()
    {
        bool NeedDisappear = false;
        m_FullRowData.Clear();
        DisappearCount = 0;
        for(int r = ROW_COUNT - 1; r >= 0; r --)
        {
            int count = 0;
            for(int c = 0; c < COL_COUNT; c ++)
            {
                var block = GetBlock(r, c);
                if(block.Status == BlockStatus.Block)
                {
                    count ++;
                }
            }

            if(count == COL_COUNT)
            {
                m_FullRowData.Add(r);
                NeedDisappear = true;
            }
            else if(count == 0)
            {
                break;
            }
        }

        if(NeedDisappear)
        {   
            uint totalRow = (uint)m_FullRowData.Count;
            uint nScore = totalRow * m_nRowScore;
            //Debug.LogError($"nScore={nScore}, totalRow={totalRow}, m_nRowScore={m_nRowScore}");

            AudioManager.Instance.PlayFXAudio(ClipID.LinkDone);

            if(totalRow > 1)
            {
                nScore += totalRow * m_nScoreAddtion;

                if(totalRow > 2)
                {
                    AudioManager.Instance.PlayFXAudio(ClipID.HighScore, 0.5f);
                }
            }

            GameSocre += nScore;

            if(GameSocre > m_nCurHighScore)
            {
                m_nCurHighScore = GameSocre;
                Level.Instance.SetHighScore((int)LevelGameType.Tetris, GameSocre);
            }

            var ui = XSFUI.Instance.Get((int)UIID.UIPlay);
            ui.Refresh((uint)UIRefreshID.PlayScore, null);

            uint level = XSFSchema.Instance.Get<SchemaTetrisLevels>((int)SchemaID.TetrisLevels).GetLevel(GameSocre);
            if(level > CurrentLevel)
            {
                CurrentLevel = level;
                ScpLevels = XSFSchema.Instance.Get<SchemaTetrisLevels>((int)SchemaID.TetrisLevels).Get(CurrentLevel);
                ui.Refresh((uint)UIRefreshID.PlayLevel, null);
                ui.Refresh((uint)UIRefreshID.ShowFireworks, null);
                
            }
            
        }
        
        int colLeft = COL_COUNT/2;
        int colRight = colLeft + 1;
        for(int i = 0; i < m_FullRowData.Count; i++)
        {
            var block = GetBlock(m_FullRowData[i], colLeft);
            block.block.DoDisappear(m_FullRowData[i], colLeft, this);

            block = GetBlock(m_FullRowData[i], colRight);
            block.block.DoDisappear(m_FullRowData[i], colRight, this);
        }

        return NeedDisappear;
    }

    public void OnBlockDisappearEvent(int row, int col)
    {
        var curBlock = GetBlock(row, col);

        int colLeft = COL_COUNT/2;

        if(col <= colLeft)
        {
            if(col > 0)
            {
                col --;
                var block = GetBlock(row, col);
                block.block.DoDisappear(row, col, this);
            }
            
        }
        else
        {
            if(col < COL_COUNT - 1)
            {
                col ++;
                var block = GetBlock(row, col);
                block.block.DoDisappear(row, col, this);
            }
        }
    }

    private bool IsEmptyRow(int r)
    {
        bool IsEmptyRow = true;
        for(int c = 0; c < COL_COUNT; c++)
        {
            var block = GetBlock(r, c);
            if(block.Status == BlockStatus.Block)
            {
                IsEmptyRow = false;
                break;
            }
        }

        return IsEmptyRow;
    }

    public void OnBlockDisappearDone(int row, int col)
    {
        var curBlock = GetBlock(row, col);
        curBlock.Status = BlockStatus.None;
        curBlock.Hide();

        DisappearCount ++;

        //Debug.LogError($"OnBlockDisappearDone DisappearCount={DisappearCount}, DisappearCheckCount={DisappearCheckCount}, row={row}, col={col}");

        if(DisappearCount == m_FullRowData.Count * COL_COUNT)
        {
            int nr = 0;
            for(int r = ROW_COUNT - 1; r >= 0; r--)
            {
                if(IsEmptyRow(r))  // 如果是空行，则往上找一行有方块的放到本行
                {
                    for(nr = r - 1; nr >= 0; nr --)
                    {
                        if(!IsEmptyRow(nr))
                        {
                            break;
                        }
                    }

                    if(nr == -1) // 上面全是空行了
                    {
                        m_nStatus = GameStatus.CreateTetris;
                        return;
                    }

                    for(int c = 0; c < COL_COUNT; c++)
                    {
                        var upBlock = GetBlock(nr, c);
                        var downBlock = GetBlock(r, c);

                        if(upBlock.Status == BlockStatus.Block)
                        {
                            upBlock.Hide();
                            upBlock.Status = BlockStatus.None;

                            downBlock.SetTetrisBlock(upBlock.block.ColorIndex);
                            downBlock.Status = BlockStatus.Block;
                        }
                    }
                }
            }

            m_nStatus = GameStatus.CreateTetris;
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

    public void ShowEndTetris()
    {
        ArrayData d = m_CurrentTetris.scp.arChangeData[m_CurrentTetris.ChangeType];
        for(int i = 0; i < d.data.Length; i ++)
        {
            if(d.data[i] == 1 )
            {
                if(m_TetrisBlock[i] == null)    
                {

                }
                else
                {
                    m_TetrisBlock[i].SetTetrisBlock(m_CurrentTetris.color);
                }  
            }
        }
    }

    public override void MoveLeft()
    {
        if(m_nStatus != GameStatus.Play)
        {
            return;
        }

        m_CurrentTetris.col --;
        if(!HorizontalMove())
        {
            m_CurrentTetris.col ++;
        }
    }

    public override void MoveRight()
    {
        if(m_nStatus != GameStatus.Play)
        {
            return;
        }
        
        m_CurrentTetris.col ++;
        if(!HorizontalMove())
        {
            m_CurrentTetris.col --;
        }
    }

    public override void MoveDown()
    {
        TetrisMoveDown();
    }

    public override void Change()
    {
        if(m_nStatus != GameStatus.Play)
        {
            return;
        }

        ChangeType();
    }

    public bool HorizontalMove()
    {
        List<SingleBlock> preList = new List<SingleBlock>();
        preList.AddRange(m_TetrisBlock);

        m_TetrisBlock.Clear();
        for(int i = m_CurrentTetris.row; i < m_CurrentTetris.row + PreBlocks.COL_COUNT; i ++)
        {
            for(int j = m_CurrentTetris.col; j < m_CurrentTetris.col + PreBlocks.ROW_COUNT; j++ )
            {
                if( j < 0 || j >= COL_COUNT || i < 0 || i >= ROW_COUNT)
                    m_TetrisBlock.Add(null);
                else
                {
                    var block = GetBlock(i, j);
                    m_TetrisBlock.Add(block);
                }
            }
        }

        ArrayData d = m_CurrentTetris.scp.arChangeData[m_CurrentTetris.ChangeType];
        for(int i = 0; i < d.data.Length; i ++)
        {
            if(d.data[i] == 1 )
            {
                if(m_TetrisBlock[i] == null)    
                {
                    // 靠边了，不能再移动了
                    m_TetrisBlock.Clear();
                    m_TetrisBlock.AddRange(preList);
                    return false;
                }
                else
                {
                    if(m_TetrisBlock[i].Status == BlockStatus.Block)  // 这个地方已经有别的方块在了
                    {
                        m_TetrisBlock.Clear();
                        m_TetrisBlock.AddRange(preList);
                        return false;
                    }
                        
                }
            }
        }

        // 隐藏之前的方块
        for(int i = 0; i < preList.Count; i ++)
        {
            if(preList[i] != null && preList[i].go.activeSelf)
            {
                if(preList[i].Status == BlockStatus.None)
                    preList[i].Hide();
            }
        }

        // 显示新位置的方块
        for(int i = 0; i < d.data.Length; i ++)
        {
            if(d.data[i] == 1 )
            {
                if(m_TetrisBlock[i] == null)    
                {

                }
                else
                {
                    m_TetrisBlock[i].SetTetrisBlock(m_CurrentTetris.color);
                }  
            }
        }

        return true;
    }

    public bool TetrisMoveDown()
    {
        List<SingleBlock> preList = new List<SingleBlock>();
        preList.AddRange(m_TetrisBlock);

        m_CurrentTetris.row ++;
        m_TetrisBlock.Clear();
        for(int i = m_CurrentTetris.row; i < m_CurrentTetris.row + PreBlocks.COL_COUNT; i ++)
        {
            for(int j = m_CurrentTetris.col; j < m_CurrentTetris.col + PreBlocks.ROW_COUNT; j++ )
            {
                if(i < 0 || i >= ROW_COUNT)
                {
                    m_TetrisBlock.Add(null);
                }
                else
                {
                    var block = GetBlock(i, j);
                    m_TetrisBlock.Add(block);
                }
                
            }
        }

        ArrayData d = m_CurrentTetris.scp.arChangeData[m_CurrentTetris.ChangeType];
        if( m_CurrentTetris.row >= 0 )
        {
            for(int i = 0; i < d.data.Length; i ++)
            {
                if(d.data[i] == 1 )
                {
                    if(m_TetrisBlock[i] == null)    
                    {
                        for(int j = 0; j < preList.Count; j ++)
                        {
                            if(preList[j] != null && preList[j].go.activeSelf && preList[j].Status == BlockStatus.None)
                            {
                                preList[j].block.DoShimmer();
                                preList[j].Status = BlockStatus.Block;
                            }
                        }

                        return false;   // 已经到底了
                    }
                    else
                    {
                        if(m_TetrisBlock[i].Status == BlockStatus.Block)  // 这个地方已经有别的方块在了
                        {
                            for(int j = 0; j < preList.Count; j ++)
                            {
                                if(preList[j] != null && preList[j].go.activeSelf && preList[j].Status == BlockStatus.None)
                                {
                                    preList[j].block.DoShimmer();
                                    preList[j].Status = BlockStatus.Block;
                                }
                            }
                            return false;
                        }
                            
                    }
                }
            }
        }

        m_CurrentTetris.MoveCount ++;

        // 隐藏之前的方块
        for(int i = 0; i < preList.Count; i ++)
        {
            if(preList[i] != null && preList[i].go.activeSelf)
            {
                if(preList[i].Status == BlockStatus.None)
                    preList[i].Hide();
            }
        }

        // 显示新位置的方块
        for(int i = 0; i < d.data.Length; i ++)
        {
            if(d.data[i] == 1 )
            {
                if(m_TetrisBlock[i] == null)    
                {

                }
                else
                {
                    m_TetrisBlock[i].SetTetrisBlock(m_CurrentTetris.color);
                }  
            }
        }

        return true;
    }

    public bool ChangeType()
    {
        List<SingleBlock> preList = new List<SingleBlock>();
        preList.AddRange(m_TetrisBlock);

        m_TetrisBlock.Clear();
        for(int i = m_CurrentTetris.row; i < m_CurrentTetris.row + PreBlocks.COL_COUNT; i ++)
        {
            for(int j = m_CurrentTetris.col; j < m_CurrentTetris.col + PreBlocks.ROW_COUNT; j++ )
            {
                if(i < 0 || i >= ROW_COUNT || j < 0 || j >= COL_COUNT)
                {
                    m_TetrisBlock.Add(null);
                }
                else
                {
                    var block = GetBlock(i, j);
                    m_TetrisBlock.Add(block);
                }
            }
        }

        int nNextType = m_CurrentTetris.ChangeType + 1;
        if(nNextType >= m_CurrentTetris.scp.arChangeData.Length)
        {
            nNextType = 0;
        }
        ArrayData d = m_CurrentTetris.scp.arChangeData[nNextType];

        for(int i = 0; i < d.data.Length; i ++)
        {
            if(d.data[i] == 1 )
            {
                if(m_TetrisBlock[i] == null)    
                {
                    return false;   // 已经到底了
                }
                else
                {
                    if(m_TetrisBlock[i].Status == BlockStatus.Block)  // 这个地方已经有别的方块在了
                    {
                        return false;
                    }  
                }
            }
        }

        m_CurrentTetris.ChangeType = nNextType;

        // 隐藏之前的方块
        for(int i = 0; i < preList.Count; i ++)
        {
            if(preList[i] != null && preList[i].go.activeSelf)
            {
                if(preList[i].Status == BlockStatus.None)
                    preList[i].Hide();
            }
        }

        // 显示新位置的方块
        for(int i = 0; i < d.data.Length; i ++)
        {
            if(d.data[i] == 1 )
            {
                if(m_TetrisBlock[i] == null)    
                {

                }
                else
                {
                    m_TetrisBlock[i].SetTetrisBlock(m_CurrentTetris.color);
                }  
            }
        }

        return true;
    }

    public override void Ultra()
    {
        if(m_nStatus != GameStatus.Play)
        {
            return;
        }

        AudioManager.Instance.PlayFXAudio(ClipID.StartCreate);
        int nCount = 0;
        while(TetrisMoveDown())
        {
            nCount ++;
        }

        if(nCount > 2)
        {
            m_nStatus = GameStatus.FullCheck;
        }
        
    }

    public override uint MaxLevel 
    { 
        get
        {
            return XSFSchema.Instance.Get<SchemaTetrisLevels>((int)SchemaID.TetrisLevels).MaxLevel;
        }
    }
}
