//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Scripts/Level/Tetris/TetrisDef.cs
// 作者：Xoen
// 时间：2023/08/26
// 描述：
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;

public class TetrisDef
{
    public const float SINGLE_BLOCK_SIZE = 0.8f;
    public const float BLOCK_Z = -3.0f;

    public static SingleBlock[] CreateBlocks(int row, int col, Transform rootT, GameObject go)
    {
        var blocks = new SingleBlock[row * col];

        int index = 0;
        float XStart = (TetrisDef.SINGLE_BLOCK_SIZE * col)/-2 + TetrisDef.SINGLE_BLOCK_SIZE/2;
        float YStart = (TetrisDef.SINGLE_BLOCK_SIZE * row)/2 - TetrisDef.SINGLE_BLOCK_SIZE/2;
        for(int r = 0; r < row; r ++)
        {
            float y = YStart - r * TetrisDef.SINGLE_BLOCK_SIZE;

            for(int c = 0; c < col; c ++)
            {
                var sb = new SingleBlock();
                sb.row = r;
                sb.col = c;
                sb.go = GameObject.Instantiate(go);
                sb.mono = sb.go.GetComponent<MonoBlock>();

                float x = XStart + c * TetrisDef.SINGLE_BLOCK_SIZE;

                sb.go.transform.SetParent(rootT, false);
                sb.go.transform.localPosition = new Vector3(x, y, TetrisDef.BLOCK_Z);

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

/*
0:0:0:0:
0:1:1:0:
0:1:1:0:
0:0:0:0 -2, 4

0:1:0:0:
0:1:0:0:
0:1:1:0:
0:0:0:0 -1:-2:-1:-1, 4:4:4:4

0:0:0:0:
1:1:1:0:
1:0:0:0:
0:0:0:0  -2, 4

1:1:0:0:
0:1:0:0:
0:1:0:0:
0:0:0:0  -1, 4

0:0:1:0:
1:1:1:0:
0:0:0:0:
0:0:0:0  -1, 4


0:1:0:0:
0:1:0:0:
1:1:0:0:
0:0:0:0  -1:-1:-1:-2, 4

1:0:0:0:
1:1:1:0:
0:0:0:0:
0:0:0:0  -1, 4

0:1:1:0:
0:1:0:0:
0:1:0:0:
0:0:0:0   -1, 4

0:0:0:0:
1:1:1:0:
0:0:1:0:
0:0:0:0  -2, 4



0:0:0:0:
1:1:0:0:
0:1:1:0:
0:0:0:0  -2, 4

0:0:0:0:
0:0:1:0:
0:1:1:0:
0:1:0:0   -2, 4


0:0:0:0:
0:0:1:1:
0:1:1:0:
0:0:0:0   -2, 4

0:0:0:0:
0:1:0:0:
0:1:1:0:
0:0:1:0   -2, 4



0:0:0:0:
1:1:1:1:
0:0:0:0:
0:0:0:0   -2, 4

0:1:0:0:
0:1:0:0:
0:1:0:0:
0:1:0:0   -1, 4



0:0:0:0:
0:1:0:0:
1:1:1:0:
0:0:0:0  -2, 4

0:0:0:0:
0:1:0:0:
0:1:1:0: 
0:1:0:0  -2, 4

0:0:0:0:
0:0:0:0:
1:1:1:0:
0:1:0:0    -3, 4

0:0:0:0:
0:1:0:0:
1:1:0:0:
0:1:0:0    -2, 4


0:0:0:0:
1:1:1:1:
1:0:0:1:
1:0:0:1    -2, 4

0:1:1:0:
0:1:1:0:
0:1:1:0:
0:1:1:0    -1, 4

1:1:1:1:
0:0:0:0:
1:1:1:1:
1:1:1:1    -1, 4


*/