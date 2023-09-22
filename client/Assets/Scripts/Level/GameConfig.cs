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
    public Vector3 MainCharacterPos;
    public Vector3 TitlePos;
    public Vector3 LobbyPos;

    public Vector3 CharacterEnterPos;
    public Vector3 CharacterExitPos;
    public float CharacterRotaSpeed;

    public float RunSpeed;
    public float WalkSpeed;
}