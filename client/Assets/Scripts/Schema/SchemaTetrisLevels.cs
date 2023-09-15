//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\Scripts\Schema\SchemaTetrisLevels.cs
// 作者：Xoen Xie
// 时间：8/27/2023
// 描述：俄罗斯方块游戏关卡配置
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

public class SchemaTetrisLevels : ISchema
{
    private ScpTetrisLevels [] m_Datas;

    public string GetSchemaName(string name)
    {
        return name;
    }

    public bool OnSchemaLoad(ISchemaReader reader)
    {
        CSVReader csv = reader as CSVReader;
        m_Datas = new ScpTetrisLevels[csv.mRowCount + 1];

        for (int i = 0; i < csv.mRowCount; ++i)
        {
            ScpTetrisLevels scp = new ScpTetrisLevels();

//_CSV_LIST_BEGIN_
			scp.uId = (csv.GetData((int)CSVDataType.Uint, i, (int)CSVIndex.ScpTetrisLevels_id) as CSVData_Uint).uValue;	// 编号
			scp.arTetris = (csv.GetData((int)CSVDataType.Array, i, (int)CSVIndex.ScpTetrisLevels_Tetris) as CSVData_Array).arValue;	// 俄罗斯方块
			scp.fDownInterval = (csv.GetData((int)CSVDataType.Float, i, (int)CSVIndex.ScpTetrisLevels_DownInterval) as CSVData_Float).fValue;	// 掉落时间间隔
			scp.uLevelScore = (csv.GetData((int)CSVDataType.Uint, i, (int)CSVIndex.ScpTetrisLevels_LevelScore) as CSVData_Uint).uValue;	// 等级分数
//_CSV_LIST_END_

            //* 使用数组
            m_Datas[scp.uId] = scp;
            //*/

            if(scp.uId > MaxLevel)
                MaxLevel = scp.uId;
        }

        return true;
    }

    //* 使用数组 根据索引获取
    public ScpTetrisLevels Get(uint nIndex)
    {
        if (nIndex >= m_Datas.Length)
        {
            UnityEngine.Debug.LogError(string.Format("SchemaTetrisLevels.Get nIndex[{0}] >= m_Datas.Length[{1}]", nIndex, m_Datas.Length));
            return null;
        }

        return m_Datas[nIndex];
    }

    public uint MaxLevel { get; private set;}

    public uint GetLevel(uint nScore)
    {
        uint nResult = 1;
        for(uint i = 1; i < m_Datas.Length - 1; i ++)
        {
            nResult = i;

            if(nScore >= m_Datas[i].uLevelScore)
            {
                if(nScore < m_Datas[i+1].uLevelScore)
                {
                    break;
                }
            }
            else
            {
                break;
            }
        }

        return nResult;
    }
    //*/
}
