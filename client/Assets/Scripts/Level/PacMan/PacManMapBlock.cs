//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Scripts/Level/PacMan/PacManMapBlock.cs
// 作者：Xoen Xie
// 时间：2023/06/21
// 描述：吃豆人 地图块
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections.Generic;

public class PacManMapBlock
{
    public int Index;
    public GameObject go;
    public ScpPacManMap scp;

    public int[] ConnectIndex = new int[4];
}

public enum BlockType
{
    None = 0,
    Bean = 0b1,             // 普通豆
    EnergyBean = 0b10,      // 能量豆
    PacMan = 0b100,
    Ghost = 0b1000,     // ghost
    RedArea = 0b10000,  // 红色区域，ghost不能往上走的区域
    Tunnel = 0b100000,   // 隧道区域，ghost要减速
    GhostStart = 0b1000000,   // 鬼屋
    Max,
}

public class PacManMap
{
    public List<PacManMapBlock> m_Blocks;
    public const float SINGLE_BLOCK_SIZE = 0.38f;
    public const int MAX_COL = 28;
    public const int MAX_ROW = 31;

    public const float BLOCK_Z = -3.0f;

    public GameObject[] m_Beans;

    public PacManMap()
    {
        m_Blocks = new List<PacManMapBlock>();
        m_Beans = new GameObject[MAX_COL * MAX_ROW];
    }

    public void Create(MonoPacMan mono)
    {
        int index = 0;
        float XStart = (SINGLE_BLOCK_SIZE * MAX_COL) / -2 + SINGLE_BLOCK_SIZE / 2;
        float YStart = (SINGLE_BLOCK_SIZE * MAX_ROW) / 2 - SINGLE_BLOCK_SIZE / 2;

        var datas = XSFSchema.Instance.Get<SchemaPacManMap>((int)SchemaID.PacManMap).Datas;

        for (int r = 0; r < MAX_ROW; r++)
        {
            float y = YStart - r * SINGLE_BLOCK_SIZE;

            for (int c = 0; c < MAX_COL; c++)
            {
                float x = XStart + c * SINGLE_BLOCK_SIZE;
                PacManMapBlock block = new PacManMapBlock();
                block.Index = index;
                block.scp = datas[index++];

                Debug.Log($"index={index}, c={c}, row={r}, block.scp.iCol={block.scp.iCol}, block.scp.iRow={block.scp.iRow}");

                if (block.scp.iCol != c || block.scp.iRow != r)
                {
                    Debug.LogError($"PacManMap Create block.scp.iCol:{block.scp.iCol} != c:{c} || block.scp.iRow:{block.scp.iRow} != r:{r}");
                    return;
                }

                string prefix = "";

                switch (block.scp.sSprite)
                {
                    case "double-angle-90":
                        block.go = GameObject.Instantiate(mono.MapObjs[(int)MapObjID.DoubleAngle90]);
                        break;

                    case "double-angle-small":
                        block.go = GameObject.Instantiate(mono.MapObjs[(int)MapObjID.DoubleAngleSmall]);
                        break;

                    case "double-angle":
                        block.go = GameObject.Instantiate(mono.MapObjs[(int)MapObjID.DoubleAngle]);
                        break;

                    case "double-line":
                        block.go = GameObject.Instantiate(mono.MapObjs[(int)MapObjID.DoubleLine]);
                        break;

                    case "single-angle-line":
                        block.go = GameObject.Instantiate(mono.MapObjs[(int)MapObjID.SingleAngleLine]);
                        break;

                    case "single-angle":
                        block.go = GameObject.Instantiate(mono.MapObjs[(int)MapObjID.SingleAngle]);
                        break;

                    case "single-line":
                        block.go = GameObject.Instantiate(mono.MapObjs[(int)MapObjID.SingleLine]);
                        break;

                    default:
                        block.go = new GameObject();
                        prefix = "_path";
                        
                        if ((block.scp.uBlockType & (uint)BlockType.EnergyBean) == (uint)BlockType.EnergyBean)
                        {
                            m_Beans[block.Index] = GameObject.Instantiate(mono.MapObjs[(int)MapObjID.EnergyBean]);
                            m_Beans[block.Index].transform.SetParent(mono.Frame.transform);
                            m_Beans[block.Index].transform.localPosition = new Vector3(x, y, BLOCK_Z);
                            m_Beans[block.Index].SetActive(true);
                        }
                        else if((block.scp.uBlockType & (uint)BlockType.Bean) == (uint)BlockType.Bean)
                        {
                            m_Beans[block.Index] = GameObject.Instantiate(mono.MapObjs[(int)MapObjID.Bean]);
                            m_Beans[block.Index].transform.SetParent(mono.Frame.transform);
                            m_Beans[block.Index].transform.localPosition = new Vector3(x, y, BLOCK_Z);
                            m_Beans[block.Index].SetActive(true);
                        }
                        break;
                }

                if (block.go != null)
                {
                    block.go.SetActive(true);
                    block.go.name = $"{block.scp.iRow}_{block.scp.iCol}_{block.Index}{prefix}";
                    block.go.transform.SetParent(mono.Frame.transform);
                    block.go.transform.localPosition = new Vector3(x, y, BLOCK_Z);
                    block.go.transform.localRotation = Quaternion.Euler(block.scp.fXRota, block.scp.fYRota, block.scp.fZRota);
                }

                m_Blocks.Add(block);
            }
        }

        for (int i = 0; i < m_Blocks.Count; i++)
        {
            if (m_Blocks[i].go.name.Contains("path"))
            {
                int nCol = m_Blocks[i].scp.iCol;
                int nRow = m_Blocks[i].scp.iRow;

                // 上
                int upRow = nRow - 1;
                if (upRow >= 0)
                {
                    var blockUp = GetBlock(upRow, nCol);
                    if (blockUp != null)
                    {
                        if (blockUp.go.name.Contains("path"))
                        {
                            m_Blocks[i].go.name += $".u{blockUp.Index}";
                            m_Blocks[i].ConnectIndex[0] = blockUp.Index;
                        }
                    }
                }


                // right
                int rightCol = nCol + 1;
                if (rightCol < MAX_COL)
                {
                    var blockRight = GetBlock(nRow, rightCol);
                    if (blockRight != null)
                    {
                        if (blockRight.go.name.Contains("path"))
                        {
                            m_Blocks[i].go.name += $".r{blockRight.Index}";
                            m_Blocks[i].ConnectIndex[1] = blockRight.Index;
                        }
                    }
                }

                // down
                int downRow = nRow + 1;
                if (downRow < MAX_ROW)
                {
                    var blockDown = GetBlock(downRow, nCol);
                    if (blockDown != null)
                    {
                        if (blockDown.go.name.Contains("path"))
                        {
                            m_Blocks[i].go.name += $".d{blockDown.Index}";
                            m_Blocks[i].ConnectIndex[2] = blockDown.Index;
                        }
                    }
                }

                // left
                int leftCol = nCol - 1;
                if (leftCol >= 0)
                {
                    var blockLeft = GetBlock(nRow, leftCol);
                    if (blockLeft != null)
                    {
                        if (blockLeft.go.name.Contains("path"))
                        {
                            m_Blocks[i].go.name += $".l{blockLeft.Index}";
                            m_Blocks[i].ConnectIndex[3] = blockLeft.Index;
                        }
                    }
                }

                // 构建隧道
                if (i == 392)
                {
                    m_Blocks[i].go.name += $".l419";
                    m_Blocks[i].ConnectIndex[3] = 419;
                }
                else if (i == 419)
                {
                    m_Blocks[i].go.name += $".r392";
                    m_Blocks[i].ConnectIndex[1] = 392;
                }
            }
        }


        Debug.Log($"enum str={BlockType.GhostStart} value={(int)BlockType.GhostStart}");
        Debug.Log($"enum str={BlockType.EnergyBean} value={(int)BlockType.EnergyBean}");
        Debug.Log($"enum str=BlockType.Bean|BlockType.RedArea value={(int)(BlockType.Bean | BlockType.RedArea)}");
        Debug.Log($"enum str={BlockType.PacMan} value={(int)(BlockType.PacMan)}");
        Debug.Log($"enum str=BlockType.Bean|BlockType.RedArea value={(int)(BlockType.PacMan | BlockType.RedArea)}");
    }

    public void Release()
    {
        for (int i = 0; i < m_Blocks.Count; i++)
        {
            GameObject.Destroy(m_Blocks[i].go);
        }
    }

    public PacManMapBlock GetBlock(int row, int col)
    {
        var index = LevelDef.GetBlockIndex(row, col, MAX_COL);
        if (index < 0 || index >= m_Blocks.Count)
            return null;

        return m_Blocks[index];
    }
}