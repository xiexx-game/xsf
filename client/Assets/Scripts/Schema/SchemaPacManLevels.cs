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
			scp.fMoveSpeed = (csv.GetData((int)CSVDataType.Float, i, (int)CSVIndex.ScpPacManLevels_MoveSpeed) as CSVData_Float).fValue;	// 吃豆人速度
			scp.fBeanMoveSpeed = (csv.GetData((int)CSVDataType.Float, i, (int)CSVIndex.ScpPacManLevels_BeanMoveSpeed) as CSVData_Float).fValue;	// 吃豆人速度
			scp.fEnergyMoveSpeed = (csv.GetData((int)CSVDataType.Float, i, (int)CSVIndex.ScpPacManLevels_EnergyMoveSpeed) as CSVData_Float).fValue;	// 恐惧吃豆人速度
			scp.fGhostSpeed = (csv.GetData((int)CSVDataType.Float, i, (int)CSVIndex.ScpPacManLevels_GhostSpeed) as CSVData_Float).fValue;	// 鬼速度
			scp.fGhostTunnelMoveSpeed = (csv.GetData((int)CSVDataType.Float, i, (int)CSVIndex.ScpPacManLevels_GhostTunnelMoveSpeed) as CSVData_Float).fValue;	// 鬼隧道速度
			scp.fEnergyGhostSpeed = (csv.GetData((int)CSVDataType.Float, i, (int)CSVIndex.ScpPacManLevels_EnergyGhostSpeed) as CSVData_Float).fValue;	// 恐惧鬼速度
			scp.fEnergyTime = (csv.GetData((int)CSVDataType.Float, i, (int)CSVIndex.ScpPacManLevels_EnergyTime) as CSVData_Float).fValue;	// 恐惧时间
			scp.uDotsLeft1 = (csv.GetData((int)CSVDataType.Uint, i, (int)CSVIndex.ScpPacManLevels_DotsLeft1) as CSVData_Uint).uValue;	// 幽灵离开1
			scp.fGhostSpeed1 = (csv.GetData((int)CSVDataType.Float, i, (int)CSVIndex.ScpPacManLevels_GhostSpeed1) as CSVData_Float).fValue;	// 幽灵速度1
			scp.uDotsLeft2 = (csv.GetData((int)CSVDataType.Uint, i, (int)CSVIndex.ScpPacManLevels_DotsLeft2) as CSVData_Uint).uValue;	// 幽灵离开2
			scp.fGhostSpeed2 = (csv.GetData((int)CSVDataType.Float, i, (int)CSVIndex.ScpPacManLevels_GhostSpeed2) as CSVData_Float).fValue;	// 幽灵速度2
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
            return m_Datas[m_Datas.Length-1];
        }

        return m_Datas[nIndex];
    }
    //*/
}
