//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\Scripts\Schema\SchemaSnakeLevels.cs
// 作者：Xoen Xie
// 时间：9/13/2023
// 描述：贪吃蛇游戏关卡配置
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

public class SchemaSnakeLevels : ISchema
{
    private ScpSnakeLevels [] m_Datas;

    public string GetSchemaName(string name)
    {
        return name;
    }

    public bool OnSchemaLoad(ISchemaReader reader)
    {
        CSVReader csv = reader as CSVReader;
        m_Datas = new ScpSnakeLevels[csv.mRowCount + 1];

        for (int i = 0; i < csv.mRowCount; ++i)
        {
            ScpSnakeLevels scp = new ScpSnakeLevels();

//_CSV_LIST_BEGIN_
			scp.uId = (csv.GetData((int)CSVDataType.Uint, i, (int)CSVIndex.ScpSnakeLevels_id) as CSVData_Uint).uValue;	// 编号
			scp.fMoveInterval = (csv.GetData((int)CSVDataType.Float, i, (int)CSVIndex.ScpSnakeLevels_MoveInterval) as CSVData_Float).fValue;	// 移动时间间隔
			scp.uLevelScore = (csv.GetData((int)CSVDataType.Uint, i, (int)CSVIndex.ScpSnakeLevels_LevelScore) as CSVData_Uint).uValue;	// 等级分数
			scp.uFoodScore = (csv.GetData((int)CSVDataType.Uint, i, (int)CSVIndex.ScpSnakeLevels_FoodScore) as CSVData_Uint).uValue;	// 食物分数
//_CSV_LIST_END_

            //* 使用数组
            m_Datas[scp.uId] = scp;
            //*/
        }

        return true;
    }

    //* 使用数组 根据索引获取
    public ScpSnakeLevels Get(uint nIndex)
    {
        if (nIndex >= m_Datas.Length)
        {
            UnityEngine.Debug.LogError(string.Format("SchemaSnakeLevels.Get nIndex[{0}] >= m_Datas.Length[{1}]", nIndex, m_Datas.Length));
            return null;
        }

        return m_Datas[nIndex];
    }
    //*/
}
