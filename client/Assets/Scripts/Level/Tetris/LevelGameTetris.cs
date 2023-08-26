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

public class LevelGameTetris : LevelGame, ILoadingHandler
{   
    enum ObjID
    {
        Scene = 0,
        Block,
        Max,
    }

    private ScpLevelGame m_GameScp;

    private GameObject[] m_SceneObj;

    public override void Init()
    {
        m_SceneObj = new GameObject[(int)ObjID.Max];
    }

    public override void Load()
    {
        m_GameScp = XSFSchema.Instance.Get<SchemaLevelGame>((int)SchemaID.LevelGame).Get((uint)LevelGameType.Tetris);
        if(m_GameScp == null)
        {
            Debug.LogError("LevelGameTetris Load m_GameScp is null");
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

        LoadingManager.Instance.Start();
    }

    public override void Enter()
    {

    }

    public override void Exit()
    {

    }

    public override void OnUpdate()
    {

    }

    public void OnGoLoadingDone(int id, GameObject go)
    {

    }
}
