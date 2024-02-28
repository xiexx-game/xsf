//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Scripts/Level/LevelGame.cs
// 作者：Xoen
// 时间：2023/08/25
// 描述：关卡 游戏
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;

public enum LevelGameType
{
    None = 0,
    Tetris = 1,
    Snake,
    PacMan,
    Max,
}

public abstract class LevelGame
{
    public uint CurrentLevel { get; set;}
    public uint GameSocre { get; set; }

    public abstract void Init();
    public abstract void Load();

    public abstract void PreCreate();

    public abstract void Enter();

    public abstract void Exit();

    public abstract void OnUpdate();

    public abstract void DoLeft();

    public abstract void DoRight();

    public abstract void DoDown();

    public abstract void DoUp();

    public abstract void DoUltra();

    public abstract void Restart();

    public abstract uint MaxLevel { get;}
}

