//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Scripts/Level/PacMan/MonoPacMan.cs
// 作者：Xoen Xie
// 时间：2023/06/21
// 描述：吃豆人 Go 脚本
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;

public enum MapObjID
{
    DoubleAngle90,
    DoubleAngleSmall,
    DoubleAngle,
    DoubleLine,
    SingleAngleLine,
    SingleAngle,
    SingleLine,
    Bean,
    EnergyBean,
    Max,
}

public class MonoPacMan : MonoBehaviour
{
    public GameObject Frame;

    public GameObject []MapObjs;

    public MonoGhost []Ghosts;
    public MonoPacManCharacter PacMan; 
}