//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\Scripts\Schema\SchemaPacManLevels.cs
// 作者：Xoen Xie
// 时间：11/30/2023
// 描述：吃豆人游戏关卡配置
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

public class SchemaPacManLevels : ISchema
{
    private ScpPacManLevels [] m_Datas;

    public uint MaxLevel { get; private set;}

    public string GetSchemaName(string name)
    {
        return name;
    }

    public bool OnSchemaLoad(ISchemaReader reader)
    {
        CSVReader csv = reader as CSVReader;
        m_Datas = new ScpPacManLevels[csv.mRowCount + 1];

        for (int i = 0; i < csv.mRowCount; ++i)
        {
            ScpPacManLevels scp = new ScpPacManLevels();

//_CSV_LIST_BEGIN_
			scp.uId = (csv.GetData((int)CSVDataType.Uint, i, (int)CSVIndex.ScpPacManLevels_id) as CSVData_Uint).uValue;	// 编号
			scp.fMoveInterval = (csv.GetData((int)CSVDataType.Float, i, (int)CSVIndex.ScpPacManLevels_MoveInterval) as CSVData_Float).fValue;	// 移动时间间隔
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
    public ScpPacManLevels Get(uint nIndex)
    {
        if (nIndex >= m_Datas.Length)
        {
            UnityEngine.Debug.LogError(string.Format("SchemaPacManLevels.Get nIndex[{0}] >= m_Datas.Length[{1}]", nIndex, m_Datas.Length));
            return null;
        }

        return m_Datas[nIndex];
    }
    //*/
}
