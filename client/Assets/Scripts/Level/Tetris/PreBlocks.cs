//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Scripts/Level/Tetris/PreBlocks.cs
// 作者：Xoen
// 时间：2023/08/26
// 描述：预生成方块
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;

public class TetrisData
{
    public ScpTetris scp;
    public int row;
    public int col;
    public int color;

    public int ChangeType;
    public int MoveCount;
}

public class PreBlocks
{
    private GameObject m_Root;
    private Transform m_RootT;

    public const int ROW_COUNT = 4;
    public const int COL_COUNT = 4;

    private SingleBlock[] m_Blocks;

    private TetrisData m_Current;

    public void Create(GameObject root, GameObject blockObj)
    {
        m_Root = root;
        m_RootT = m_Root.transform;

        m_Blocks = TetrisDef.CreateBlocks(ROW_COUNT, COL_COUNT, m_RootT, blockObj);

        for(int i = 0; i < m_Blocks.Length; i ++)
        {
            m_Blocks[i].mono.SetPreColor();
        }
    }

    public void CreateNextTetris()
    {
        var level = Level.Instance.Current as LevelGameTetris;
        var count = level.ScpLevels.arTetris.Length;
        int index = UnityEngine.Random.Range(0, count);
        var tetrisType = level.ScpLevels.arTetris[index];
        ScpTetris scp = XSFSchema.Instance.Get<SchemaTetris>((int)SchemaID.Tetris).Get(tetrisType);
        if(scp == null)
        {
            Debug.LogError($"CreateNextTetris scp == null, type={tetrisType}");
            return;
        }

        m_Current = new TetrisData();
        var typeCount = scp.arChangeData.Length;
        m_Current.ChangeType = UnityEngine.Random.Range(0, typeCount);

        m_Current.scp = scp;
        m_Current.row = scp.arRows[m_Current.ChangeType];
        m_Current.col = scp.arCols[m_Current.ChangeType];
        m_Current.color = UnityEngine.Random.Range(0, (int)BlockColor.Max);

        ArrayData d = scp.arChangeData[m_Current.ChangeType];

        for(int i = 0; i < m_Blocks.Length; i ++)
        {
            if(d.data[i] == 0)
            {
                m_Blocks[i].mono.Hide();
            }
            else
            {
                m_Blocks[i].mono.ShowWithColor(m_Current.color);
            }
        }
    }

    public TetrisData GetNextTetris()
    {
        if(m_Current == null)
            CreateNextTetris();

        var result = m_Current;
        CreateNextTetris();

        return result;
    }


    public void Release()
    {
        m_Root = null;
        m_RootT = null;
    }
}