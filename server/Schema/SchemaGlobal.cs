//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\Scripts\Schema\SchemaGlobal.cs
// 作者：Xoen Xie
// 时间：2023/6/19
// 描述：全局变量配置
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8600, CS8602

using XSF;

namespace XsfScp
{
    public class SchemaGlobal : ISchema
    {
        private ScpGlobal []? m_Datas;

        public string GetSchemaName(string name)
        {
            return name;
        }

        public void OnSchemaLoad(ISchemaReader reader)
        {
            CSVReader? csv = reader as CSVReader;
            m_Datas = new ScpGlobal[csv.mRowCount + 1];

            for (int i = 0; i < csv.mRowCount; ++i)
            {
                ScpGlobal scp = new ScpGlobal();
                
                scp.uId = (csv.GetData((int)CSVDataType.Uint, i, (int)CSVIndex.ScpGlobal_id) as CSVData_Uint).uValue;
                if(scp.uId != i + 1) 
                {
                    throw new XSFSchemaLoadException($"SchemaGlobal.OnSchemaLoad index error, scp.uId:{scp.uId} != i:{i} + 1");
                }

                string typeStr = (csv.GetData((int)CSVDataType.String, i, (int)CSVIndex.ScpGlobal_type) as CSVData_String).sValue;
                scp.data = CSVData.GetDataByName(typeStr);

                string valueStr = (csv.GetData((int)CSVDataType.String, i, (int)CSVIndex.ScpGlobal_value) as CSVData_String).sValue;
                scp.data.Read(i, (int)CSVIndex.ScpGlobal_value, valueStr);

                //* 使用数组
                m_Datas[scp.uId] = scp;
                //*/
            }
        }

        //* 使用数组 根据索引获取
        public ScpGlobal? Get(uint nIndex)
        {
            if (nIndex >= m_Datas?.Length)
            {
                Serilog.Log.Error(string.Format("SchemaGlobal.Get nIndex[{0}] >= m_Datas.Length[{1}]", nIndex, m_Datas.Length));
                return null;
            }

            return m_Datas?[nIndex];
        }
        //*/
    }
}
