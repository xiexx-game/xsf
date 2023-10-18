//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Scripts/Level/GameConfig.cs
// 作者：Xoen
// 时间：2023/08/25
// 描述：游戏状态机
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;

public class GameConfig : MonoSingleton<GameConfig>
{
    public float CharacterRotaSpeed;

    public float RunSpeed;
    public float WalkSpeed;

    public Vector3 BornPos;

    public Vector3[] EnterPos;

    public Vector3 ExitPos;
}