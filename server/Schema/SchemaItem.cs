//////////////////////////////////////////////////////////////////////////
// 
// 文件：Schema/SchemaItem.cs
// 作者：Xoen Xie
// 时间：2023/6/19
// 描述：物品配置
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8600, CS8602, CS8603, CS8618, CS8601
using XSF;

namespace XsfScp
{
    public class SchemaItem : ISchema
    {
        private Dictionary<uint, ScpItem> m_Datas;

        public string GetSchemaName(string name)
        {
            return name;
        }

        public void OnSchemaLoad(ISchemaReader reader)
        {
            m_Datas = new Dictionary<uint, ScpItem>();
            CSVReader csv = reader as CSVReader;

            for (int i = 0; i < csv.mRowCount; ++i)
            {
                ScpItem scp = new ScpItem();

    //_CSV_LIST_BEGIN_
				scp.uId = (csv.GetData((int)CSVDataType.Uint, i, (int)CSVIndex.ScpItem_id) as CSVData_Uint).uValue;	// 编号
				scp.iType = (csv.GetData((int)CSVDataType.Int, i, (int)CSVIndex.ScpItem_type) as CSVData_Int).iValue;	// 类型
				scp.fParam1 = (csv.GetData((int)CSVDataType.Float, i, (int)CSVIndex.ScpItem_param1) as CSVData_Float).fValue;	// 参数1
				scp.ulParam2 = (csv.GetData((int)CSVDataType.Ulong, i, (int)CSVIndex.ScpItem_param2) as CSVData_Ulong).ulValue;	// 参数2
				scp.arParam3 = (csv.GetData((int)CSVDataType.Array, i, (int)CSVIndex.ScpItem_param3) as CSVData_Array).arValue;	// 参数3
				scp.icParam4 = (csv.GetData((int)CSVDataType.IDAndCount, i, (int)CSVIndex.ScpItem_param4) as CSVData_IDAndCount).icValue;	// 参数4
				scp.bParam5 = (csv.GetData((int)CSVDataType.Bool, i, (int)CSVIndex.ScpItem_param5) as CSVData_Bool).bValue;	// 参数5
    //_CSV_LIST_END_

                //* 使用字典
                if (m_Datas.ContainsKey(scp.uId))
                {
                    throw new XSFSchemaLoadException("SchemaItem.OnSchemaLoad key exist, id=" + scp.uId);
                }

                m_Datas.Add(scp.uId, scp);
                //*/
            }
        }

        //* 使用字典 根据ID获取
        public ScpItem Get(uint nID)
        {
            ScpItem scp = null;
            m_Datas.TryGetValue(nID, out scp);
            return scp;
        }
        //*/
    }
}

