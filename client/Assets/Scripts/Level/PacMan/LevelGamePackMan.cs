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

    public static LevelGamePackMan m_Instance;
    public static LevelGamePackMan Instance {
        get {
            if(m_Instance == null)
                m_Instance = new LevelGamePackMan();

            return m_Instance;
        }
    }

    public const int ROW_COUNT = 36;
    public const int COL_COUNT = 28;

    private ScpLevelGame m_GameScp;

    private GameObject[] m_SceneObj;

    private GameStatus m_nStatus;

    public ScpPacManLevels ScpLevels { get; private set; }

    public PacManMap Map { get; private set; }

    public MonoGhost []Ghosts;
    public MonoPacManCharacter Character;
    private uint m_nCurHighScore;

    public override void Init()
    {
        m_SceneObj = new GameObject[(int)ObjID.Max];
        Map = new PacManMap();

        Ghosts = new MonoGhost[(int)GhostType.Max];
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
        MonoPacMan mono = m_SceneObj[(int)ObjID.Scene].GetComponent<MonoPacMan>();
        Map.Create(mono);

        var end = new Vector3(0.0500000007f,-3.03999996f,-0.200000003f);
        var startBlock = Map.GetBlock(11, 12);
        var endBlock = Map.Pos2Block(end);

        //return;
        //List<PacManMapBlock> result = Map.FindPath(PacManMoveDir.Right, startBlock, endBlock);

        //for(int i = 0; i < result.Count; i ++)
        //{
        //    Debug.Log($"pos={result[i].scp.iRow}, {result[i].scp.iCol}");
        //    var block = Map.GetBlock(result[i].scp.iRow, result[i].scp.iCol);
        //    block.go.GetComponent<SpriteRenderer>().color = Color.yellow;
        //}
    }

    public override void Enter()
    {
        GameSocre = 0;
        m_nStatus = GameStatus.Play;
        AudioManager.Instance.PlayBGM(BGMID.Snake);
        m_nCurHighScore = Level.Instance.GetHighScore((int)LevelGameType.PacMan);
    }

    public override void Exit()
    {
        GameSocre = 0;

        Map.Release();
        
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

    public PacManMapBlock GetBlock(int row, int col)
    {
        return Map.GetBlock(row, col);
    }

    public override void Restart()
    {
        CurrentLevel = 1;
        ScpLevels = XSFSchema.Instance.Get<SchemaPacManLevels>((int)SchemaID.PacManLevels).Get(CurrentLevel);
        Enter();

        var ui = XSFUI.Instance.Get((int)UIID.UIPlayPacMan);
        ui.Refresh((uint)UIRefreshID.PlayScore, null);
        ui.Refresh((uint)UIRefreshID.PlayLevel, null);

        Character.Restart();

        for(int i = 0; i < Ghosts.Length; i ++)
        {
            Ghosts[i].Restart();
        }

        Map.Reset();
    }

    public bool IsMapReady;

    public bool IsPlaying
    {
        get
        {
            return IsMapReady && (m_nStatus == GameStatus.Play);
        }
    }

    public override void OnUpdate()
    {
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

    public void OnBeanEat(bool IsEnergy)
    {
        GameSocre += IsEnergy ? (uint)10 : (uint)1;

        var ui = XSFUI.Instance.Get((int)UIID.UIPlayPacMan);
        ui.Refresh((uint)UIRefreshID.PlayScore, null);

        if(GameSocre > m_nCurHighScore)
        {
            m_nCurHighScore = GameSocre;
            Level.Instance.SetHighScore((int)LevelGameType.PacMan, GameSocre);
        }

        if(LevelGamePackMan.Instance.Map.BeanCount <= 0)
        {
            CurrentLevel ++;
            ScpLevels = XSFSchema.Instance.Get<SchemaPacManLevels>((int)SchemaID.PacManLevels).Get(CurrentLevel);
            ui.Refresh((uint)UIRefreshID.PlayLevel, null);
            ui.Refresh((uint)UIRefreshID.ShowFireworks, null);
            AudioManager.Instance.PlayFXAudio(ClipID.HighScore);

            EnterNewLevel();
        }
    }

    public void EnterNewLevel()
    {
        Map.Reset();

        Character.Restart();

        for(int i = 0; i < Ghosts.Length; i ++)
        {
            Ghosts[i].Restart();
        }
    }

    public void GameOver()
    {
        m_nStatus = GameStatus.End;
    }

    public void OnGoLoadingDone(int id, GameObject go)
    {
        m_SceneObj[id] = go;
        
    }

    public override void DoLeft()
    {
        if(m_nStatus != GameStatus.Play)
            return;

        var current = Character.Current;
        var currentDir = Character.MoveDir;

        var leftIndex = current.ConnectIndex[(int)PacManMoveDir.Left];
        if(leftIndex > 0)
        {
            var block = Map.GetBlockByIndex(leftIndex);
            Character.Move(PacManMoveDir.Left, block.pos);
        }
        else if(Character.transform.localPosition.x <= current.pos.x)
        {
            if(currentDir == PacManMoveDir.Up)
            {
                var upIndex = current.ConnectIndex[(int)PacManMoveDir.Up];
                if(upIndex > 0)
                {
                    var upBlock = Map.GetBlockByIndex(upIndex);
                    var upLeftIndex = upBlock.ConnectIndex[(int)PacManMoveDir.Left];
                    if(upLeftIndex > 0)
                    {
                        var block = Map.GetBlockByIndex(upLeftIndex);
                        Character.Move(PacManMoveDir.Left, block.pos);
                    }
                }
            }
            else if(currentDir == PacManMoveDir.Down)
            {
                var downIndex = current.ConnectIndex[(int)PacManMoveDir.Down];
                if(downIndex > 0)
                {
                    var downBlock = Map.GetBlockByIndex(downIndex);
                    var downLeftIndex = downBlock.ConnectIndex[(int)PacManMoveDir.Left];
                    if(downLeftIndex > 0)
                    {
                        var block = Map.GetBlockByIndex(downLeftIndex);
                        Character.Move(PacManMoveDir.Left, block.pos);
                    }
                }
            }
        }
    }

    public override void DoRight()
    {
        if(m_nStatus != GameStatus.Play)
            return;

        var current = Character.Current;
        var currentDir = Character.MoveDir;

        var rightIndex = current.ConnectIndex[(int)PacManMoveDir.Right];
        if(rightIndex > 0)
        {
            var block = Map.GetBlockByIndex(rightIndex);
            Character.Move(PacManMoveDir.Right, block.pos);
        }
        else if(Character.transform.localPosition.x >= current.pos.x)
        {
            if(currentDir == PacManMoveDir.Up)
            {
                var upIndex = current.ConnectIndex[(int)PacManMoveDir.Up];
                if(upIndex > 0)
                {
                    var upBlock = Map.GetBlockByIndex(upIndex);
                    var upRightIndex = upBlock.ConnectIndex[(int)PacManMoveDir.Right];
                    if(upRightIndex > 0)
                    {
                        var block = Map.GetBlockByIndex(upRightIndex);
                        Character.Move(PacManMoveDir.Right, block.pos);
                    }
                }
            }
            else if(currentDir == PacManMoveDir.Down)
            {
                var downIndex = current.ConnectIndex[(int)PacManMoveDir.Down];
                if(downIndex > 0)
                {
                    var downBlock = Map.GetBlockByIndex(downIndex);
                    var downRightIndex = downBlock.ConnectIndex[(int)PacManMoveDir.Right];
                    if(downRightIndex > 0)
                    {
                        var block = Map.GetBlockByIndex(downRightIndex);
                        Character.Move(PacManMoveDir.Right, block.pos);
                    }
                }
            }
        }
    }

    public override void DoDown()
    {
        if(m_nStatus != GameStatus.Play)
            return;

        var current = Character.Current;
        var currentDir = Character.MoveDir;

        var downIndex = current.ConnectIndex[(int)PacManMoveDir.Down];
        if(downIndex > 0)
        {
            var block = Map.GetBlockByIndex(downIndex);
            Character.Move(PacManMoveDir.Down, block.pos);
        }
        else if(Character.transform.localPosition.y >= current.pos.y)
        {
            if(currentDir == PacManMoveDir.Left)
            {
                var leftIndex = current.ConnectIndex[(int)PacManMoveDir.Left];
                if(leftIndex > 0)
                {
                    var leftBlock = Map.GetBlockByIndex(leftIndex);
                    var downLeftIndex = leftBlock.ConnectIndex[(int)PacManMoveDir.Down];
                    if(downLeftIndex > 0)
                    {
                        var block = Map.GetBlockByIndex(downLeftIndex);
                        Character.Move(PacManMoveDir.Down, block.pos);
                    }
                }
                
            }
            else if(currentDir == PacManMoveDir.Right)
            {
                var RightIndex = current.ConnectIndex[(int)PacManMoveDir.Right];
                if(RightIndex > 0)
                {
                    var rightBlock = Map.GetBlockByIndex(RightIndex);
                    var downRightIndex = rightBlock.ConnectIndex[(int)PacManMoveDir.Down];
                    if(downRightIndex > 0)
                    {
                        var block = Map.GetBlockByIndex(downRightIndex);
                        Character.Move(PacManMoveDir.Down, block.pos);
                    }
                }
            }
        }
    }

    public override void DoUp()
    {
        if(m_nStatus != GameStatus.Play)
            return;

        var current = Character.Current;
        var currentDir = Character.MoveDir;

        var upIndex = current.ConnectIndex[(int)PacManMoveDir.Up];
        if(upIndex > 0)
        {
            var block = Map.GetBlockByIndex(upIndex);
            Character.Move(PacManMoveDir.Up, block.pos);
        }
        else if(Character.transform.localPosition.y <= current.pos.y)
        {
            if(currentDir == PacManMoveDir.Left)
            {
                var leftIndex = current.ConnectIndex[(int)PacManMoveDir.Left];
                if(leftIndex > 0)
                {
                    var leftBlock = Map.GetBlockByIndex(leftIndex);
                    var upLeftIndex = leftBlock.ConnectIndex[(int)PacManMoveDir.Up];
                    if(upLeftIndex > 0)
                    {
                        var block = Map.GetBlockByIndex(upLeftIndex);
                        Character.Move(PacManMoveDir.Up, block.pos);
                    }
                }
            }
            else if(currentDir == PacManMoveDir.Right)
            {
                var RightIndex = current.ConnectIndex[(int)PacManMoveDir.Right];
                if(RightIndex > 0)
                {
                    var rightBlock = Map.GetBlockByIndex(RightIndex);
                    var upRightIndex = rightBlock.ConnectIndex[(int)PacManMoveDir.Up];
                    if(upRightIndex > 0)
                    {
                        var block = Map.GetBlockByIndex(upRightIndex);
                        Character.Move(PacManMoveDir.Up, block.pos);
                    }
                }
            }
        }
    }

    public override void DoUltra()
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
