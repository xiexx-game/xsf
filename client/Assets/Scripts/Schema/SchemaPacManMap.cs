//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\Scripts\Schema\SchemaPacManMap.cs
// 作者：Xoen Xie
// 时间：12/8/2023
// 描述：吃豆人游戏地图配置
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

public class SchemaPacManMap : ISchema
{
    private List<ScpPacManMap> m_Datas;

    public List<ScpPacManMap> Datas { get { return m_Datas; } }

    public string GetSchemaName(string name)
    {
        return name;
    }

    public bool OnSchemaLoad(ISchemaReader reader)
    {
        CSVReader csv = reader as CSVReader;
        m_Datas = new List<ScpPacManMap>();

        for (int i = 0; i < csv.mRowCount; ++i)
        {
            ScpPacManMap scp = new ScpPacManMap();

//_CSV_LIST_BEGIN_
			scp.iRow = (csv.GetData((int)CSVDataType.Int, i, (int)CSVIndex.ScpPacManMap_Row) as CSVData_Int).iValue;	// 行
			scp.iCol = (csv.GetData((int)CSVDataType.Int, i, (int)CSVIndex.ScpPacManMap_Col) as CSVData_Int).iValue;	// 列
			scp.sSprite = (csv.GetData((int)CSVDataType.String, i, (int)CSVIndex.ScpPacManMap_Sprite) as CSVData_String).sValue;	// 图片
			scp.fXRota = (csv.GetData((int)CSVDataType.Float, i, (int)CSVIndex.ScpPacManMap_XRota) as CSVData_Float).fValue;	// X旋转
			scp.fYRota = (csv.GetData((int)CSVDataType.Float, i, (int)CSVIndex.ScpPacManMap_YRota) as CSVData_Float).fValue;	// Y旋转
			scp.fZRota = (csv.GetData((int)CSVDataType.Float, i, (int)CSVIndex.ScpPacManMap_ZRota) as CSVData_Float).fValue;	// z旋转
			scp.uBlockType = (csv.GetData((int)CSVDataType.Uint, i, (int)CSVIndex.ScpPacManMap_BlockType) as CSVData_Uint).uValue;	// 块类型
//_CSV_LIST_END_

            m_Datas.Add(scp);
        }

        return true;
    }
}
