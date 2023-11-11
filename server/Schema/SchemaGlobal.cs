//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\Scripts\Schema\SchemaGlobal.cs
// 作者：Xoen Xie
// 时间：2023/6/19
// 描述：全局变量配置
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8600, CS8602, CS8618, CS8601

using XSF;

namespace XsfScp
{
    public class SchemaGlobal : ISchema
    {
        private ScpGlobal GlobalData;

        public string GetSchemaName(string name)
        {
            return name;
        }

        public void OnSchemaLoad(ISchemaReader reader)
        {
            CSVReader? csv = reader as CSVReader;
            GlobalData = new ScpGlobal();

            // 类型， 值， 名称， 注释， client server
//_CSV_LIST_BEGIN_
			GlobalData.iIntData = (csv.GetData((int)CSVDataType.Int, (int)CSVIndex.ScpGlobal_IntData, 1) as CSVData_Int).iValue;	// 有符号整形
			GlobalData.sStringData = (csv.GetData((int)CSVDataType.String, (int)CSVIndex.ScpGlobal_StringData, 1) as CSVData_String).sValue;	// 字符串数据
			GlobalData.uUintData = (csv.GetData((int)CSVDataType.Uint, (int)CSVIndex.ScpGlobal_UintData, 1) as CSVData_Uint).uValue;	// uint整形
			GlobalData.ulUlongData = (csv.GetData((int)CSVDataType.Ulong, (int)CSVIndex.ScpGlobal_UlongData, 1) as CSVData_Ulong).ulValue;	// 长整形数据
			GlobalData.fFloatData = (csv.GetData((int)CSVDataType.Float, (int)CSVIndex.ScpGlobal_FloatData, 1) as CSVData_Float).fValue;	// 浮点数
			GlobalData.arUintArrayData = (csv.GetData((int)CSVDataType.Array, (int)CSVIndex.ScpGlobal_UintArrayData, 1) as CSVData_Array).arValue;	// 无符号整形数组
			GlobalData.bBoolData = (csv.GetData((int)CSVDataType.Bool, (int)CSVIndex.ScpGlobal_BoolData, 1) as CSVData_Bool).bValue;	// 布尔数据
			GlobalData.icIacData = (csv.GetData((int)CSVDataType.IDAndCount, (int)CSVIndex.ScpGlobal_IacData, 1) as CSVData_IDAndCount).icValue;	// id和count数据
//_CSV_LIST_END_
        }
    }
}
