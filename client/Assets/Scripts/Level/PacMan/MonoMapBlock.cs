//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Scripts/Level/PacMan/MonoMapBlock.cs
// 作者：Xoen Xie
// 时间：2023/06/21
// 描述：吃豆人地图块
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;
public class MonoMapBlock : MonoBehaviour
{
    public int ID;      // 地图编号

    public int row;
    public int col;

    public int [] ConnectID;    // 连通编号

}