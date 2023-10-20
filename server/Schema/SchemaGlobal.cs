//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\Scripts\Schema\SchemaGlobal.cs
// 作者：Xoen Xie
// 时间：2023/6/19
// 描述：全局变量配置
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8600, CS8602, CS8618

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
            int nRowIndex = 0;
//_CSV_LIST_BEGIN_
			GlobalData.iIntData = (csv.GetData((int)CSVDataType.Int, nRowIndex++, 1) as CSVData_Int).iValue;	// 有符号整形
			GlobalData.sStringData = (csv.GetData((int)CSVDataType.String, nRowIndex++, 1) as CSVData_String).sValue;	// 字符串数据
			GlobalData.uUintData = (csv.GetData((int)CSVDataType.Uint, nRowIndex++, 1) as CSVData_Uint).uValue;	// uint整形
			GlobalData.ulUlongData = (csv.GetData((int)CSVDataType.Ulong, nRowIndex++, 1) as CSVData_Ulong).ulValue;	// 长整形数据
//_CSV_LIST_END_
        }
    }
}
