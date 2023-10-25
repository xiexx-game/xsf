//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Scripts/Level/LevelDef.cs
// 作者：Xoen
// 时间：2023/08/25
// 描述：关卡相关定义
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;


public enum BlockStatus
{
    None = 0,           // 什么都没有
    Wall = 0b0001,         // 可以行走的路
    Road = 0b0010,
    Box = 0b0100,        // 箱子
    Point = 0b1000,     // 目标点位
}

public enum BlockColor
{
    Wall = 0,
    Road,
    Box,
}

public class SingleBlock
{
    public int row;
    public int col;

    public GameObject go;
    public int Status;

    public MonoSelect select;

    public MonoBox box;

    private Color32[] m_Colors = new Color32[] {
        new Color32( 82, 82, 82, 255),
        new Color32( 255, 255, 255, 255),
        new Color32( 6, 135, 245, 255),
    };

    public void Hide()
    {
        go.SetActive(false);
    }

    public void SetColor(BlockColor color)
    {
        var image = go.GetComponent<Image>();
        if(image != null)
            image.color = m_Colors[(int)color];
        else
        {
            var sp = go.GetComponent<SpriteRenderer>();
            sp.color = m_Colors[(int)color];
        }
        go.SetActive(true);
    }

    public void Clear()
    {
        if(select != null)
            GameObject.Destroy(select.gameObject);

        if(box != null)
        {
            GameObject.Destroy(box.select.gameObject);
        }
            
        GameObject.Destroy(go);
    }
}


public class LevelDef
{
    public const float BLOCK_Z = 0f;

    public static SingleBlock[] CreateBlocks(int row, int col, Transform rootT, GameObject go, float blockSize, bool isUI)
    {
        var blocks = new SingleBlock[row * col];

        int index = 0;
        float XStart = (blockSize * col)/-2 + blockSize/2;
        float YStart = (blockSize * row)/2 - blockSize/2;
        for(int r = 0; r < row; r ++)
        {
            float y = YStart - r * blockSize;

            for(int c = 0; c < col; c ++)
            {
                var sb = new SingleBlock();
                sb.row = r;
                sb.col = c;
                sb.go = GameObject.Instantiate(go);
                sb.go.name = $"{r}-{c}";

                float x = XStart + c * blockSize;

                sb.go.transform.SetParent(rootT, false);

                if(isUI)
                {
                    sb.go.transform.localPosition = new Vector3(x, y, BLOCK_Z);
                }
                else
                {
                    sb.go.transform.localPosition = new Vector3(x, BLOCK_Z, y);
                }
                

                blocks[index++] = sb;

            } 
        }

        return blocks;
    }

    public static int GetBlockIndex(int row, int col, int nColMax)
    {
        return row * nColMax + col;
    }
}