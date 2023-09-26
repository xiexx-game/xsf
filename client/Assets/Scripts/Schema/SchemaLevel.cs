//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\Scripts\Schema\SchemaLevel.cs
// 作者：Xoen Xie
// 时间：9/23/2023
// 描述：关卡配置
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

public class SchemaLevel : ISchema
{
    private ScpLevel [] m_Datas;

    public string GetSchemaName(string name)
    {
        return name;
    }

    public bool OnSchemaLoad(ISchemaReader reader)
    {
        CSVReader csv = reader as CSVReader;
        m_Datas = new ScpLevel[csv.mRowCount + 1];

        for (int i = 0; i < csv.mRowCount; ++i)
        {
            ScpLevel scp = new ScpLevel();

//_CSV_LIST_BEGIN_
			scp.uId = (csv.GetData((int)CSVDataType.Uint, i, (int)CSVIndex.ScpLevel_id) as CSVData_Uint).uValue;	// 编号
			scp.sarData = (csv.GetData((int)CSVDataType.SArray, i, (int)CSVIndex.ScpLevel_data) as CSVData_SArray).sarValue;	// 关卡数据
			scp.uRowCount = (csv.GetData((int)CSVDataType.Uint, i, (int)CSVIndex.ScpLevel_RowCount) as CSVData_Uint).uValue;	// 行数
			scp.uColCount = (csv.GetData((int)CSVDataType.Uint, i, (int)CSVIndex.ScpLevel_ColCount) as CSVData_Uint).uValue;	// 列数
//_CSV_LIST_END_

            //* 使用数组
            m_Datas[scp.uId] = scp;
            //*/
        }

        return true;
    }

    //* 使用数组 根据索引获取
    public ScpLevel Get(uint nIndex)
    {
        if (nIndex >= m_Datas.Length)
        {
            //UnityEngine.Debug.LogError(string.Format("SchemaLevel.Get nIndex[{0}] >= m_Datas.Length[{1}]", nIndex, m_Datas.Length));
            return null;
        }

        return m_Datas[nIndex];
    }
    //*/
}
