//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\Scripts\Schema\SchemaLevelGame.cs
// 作者：Xoen Xie
// 时间：8/26/2023
// 描述：关卡游戏
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

public class SchemaLevelGame : ISchema
{
    private ScpLevelGame [] m_Datas;

    public string GetSchemaName(string name)
    {
        return name;
    }

    public bool OnSchemaLoad(ISchemaReader reader)
    {
        CSVReader csv = reader as CSVReader;
        m_Datas = new ScpLevelGame[csv.mRowCount + 1];

        for (int i = 0; i < csv.mRowCount; ++i)
        {
            ScpLevelGame scp = new ScpLevelGame();

//_CSV_LIST_BEGIN_
			scp.uId = (csv.GetData((int)CSVDataType.Uint, i, (int)CSVIndex.ScpLevelGame_id) as CSVData_Uint).uValue;	// 编号
			scp.sName = (csv.GetData((int)CSVDataType.String, i, (int)CSVIndex.ScpLevelGame_Name) as CSVData_String).sValue;	// 名称
			scp.arShowUIs = (csv.GetData((int)CSVDataType.Array, i, (int)CSVIndex.ScpLevelGame_ShowUIs) as CSVData_Array).arValue;	// 显示UI
			scp.sarSceneObjects = (csv.GetData((int)CSVDataType.SArray, i, (int)CSVIndex.ScpLevelGame_SceneObjects) as CSVData_SArray).sarValue;	// 场景资源
//_CSV_LIST_END_

            //* 使用数组
            m_Datas[scp.uId] = scp;
            //*/
        }

        return true;
    }

    //* 使用数组 根据索引获取
    public ScpLevelGame Get(uint nIndex)
    {
        if (nIndex >= m_Datas.Length)
        {
            UnityEngine.Debug.LogError(string.Format("SchemaLevelGame.Get nIndex[{0}] >= m_Datas.Length[{1}]", nIndex, m_Datas.Length));
            return null;
        }

        return m_Datas[nIndex];
    }
    //*/
}
