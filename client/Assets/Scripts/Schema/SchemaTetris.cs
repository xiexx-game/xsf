//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\Scripts\Schema\SchemaTetris.cs
// 作者：Xoen Xie
// 时间：8/26/2023
// 描述：俄罗斯方块配置
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

public class SchemaTetris : ISchema
{
    private ScpTetris [] m_Datas;

    public string GetSchemaName(string name)
    {
        return name;
    }

    public bool OnSchemaLoad(ISchemaReader reader)
    {
        CSVReader csv = reader as CSVReader;
        m_Datas = new ScpTetris[csv.mRowCount];

        for (int i = 0; i < csv.mRowCount; ++i)
        {
            ScpTetris scp = new ScpTetris();

//_CSV_LIST_BEGIN_
			scp.uId = (csv.GetData((int)CSVDataType.Uint, i, (int)CSVIndex.ScpTetris_id) as CSVData_Uint).uValue;	// 编号
			scp.arChangeData = (csv.GetData((int)CSVDataType.D2Ar, i, (int)CSVIndex.ScpTetris_ChangeData) as CSVData_D2Ar).arValue;	// 变体数据
			scp.arRows = (csv.GetData((int)CSVDataType.IArray, i, (int)CSVIndex.ScpTetris_rows) as CSVData_IArray).arValue;	// 出生的行
			scp.arCols = (csv.GetData((int)CSVDataType.IArray, i, (int)CSVIndex.ScpTetris_cols) as CSVData_IArray).arValue;	// 出生的列
//_CSV_LIST_END_

            for(int j = 0; j < scp.arChangeData.Length; j ++)
            {
                if(scp.arChangeData[j].data.Length != 16)
                {
                    UnityEngine.Debug.LogError($"SchemaTetris OnSchemaLoad tetris data error, id={scp.uId}, index={j}");
                    return false;
                }
            }

            if(scp.arRows.Length != scp.arChangeData.Length)
            {
                UnityEngine.Debug.LogError($"SchemaTetris OnSchemaLoad tetris data error, id={scp.uId}, scp.arRows.Length:{scp.arRows.Length} != scp.arChangeData.Length:{scp.arChangeData.Length}");
                return false;
            }

            //* 使用数组
            m_Datas[scp.uId] = scp;
            //*/
        }

        return true;
    }

    //* 使用数组 根据索引获取
    public ScpTetris Get(uint nIndex)
    {
        if (nIndex >= m_Datas.Length)
        {
            UnityEngine.Debug.LogError(string.Format("SchemaTetris.Get nIndex[{0}] >= m_Datas.Length[{1}]", nIndex, m_Datas.Length));
            return null;
        }

        return m_Datas[nIndex];
    }
    //*/
}
