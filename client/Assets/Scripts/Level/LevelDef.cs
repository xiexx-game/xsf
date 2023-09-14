//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Scripts/Level/LevelDef.cs
// 作者：Xoen
// 时间：2023/08/26
// 描述：
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;

public enum BlockStatus
{
    None = 0,
    Block,
    Food,
}

public class SingleBlock
{
    public int row;
    public int col;
    public MonoBlock block;

    public MonoSnakeFood food;

    public GameObject go;
    public BlockStatus Status;

    public SnakeMoveDir dir;
}


public class LevelDef
{
    public const float SINGLE_BLOCK_SIZE = 0.8f;
    public const float BLOCK_Z = -3.0f;

    public static SingleBlock[] CreateBlocks(LevelGameType nType, int row, int col, Transform rootT, GameObject go)
    {
        var blocks = new SingleBlock[row * col];

        int index = 0;
        float XStart = (SINGLE_BLOCK_SIZE * col)/-2 + SINGLE_BLOCK_SIZE/2;
        float YStart = (SINGLE_BLOCK_SIZE * row)/2 - SINGLE_BLOCK_SIZE/2;
        for(int r = 0; r < row; r ++)
        {
            float y = YStart - r * SINGLE_BLOCK_SIZE;

            for(int c = 0; c < col; c ++)
            {
                var sb = new SingleBlock();
                sb.row = r;
                sb.col = c;
                sb.go = GameObject.Instantiate(go);

                switch(nType)
                {
                case LevelGameType.Tetris:
                    sb.block = sb.go.GetComponent<MonoBlock>();
                    break;

                case LevelGameType.Snake:
                    {
                        sb.block = sb.go.transform.Find("base").GetComponent<MonoBlock>();
                        sb.food = sb.go.transform.Find("food").GetComponent<MonoSnakeFood>();
                    }
                    break;
                }
                

                float x = XStart + c * SINGLE_BLOCK_SIZE;

                sb.go.transform.SetParent(rootT, false);
                sb.go.transform.localPosition = new Vector3(x, y, BLOCK_Z);

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