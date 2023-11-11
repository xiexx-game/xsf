//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\Scripts\Schema\SchemaGlobal.cs
// 作者：Xoen Xie
// 时间：2023/6/19
// 描述：全局变量配置
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using UnityEngine;
using XSF;

namespace XsfScp
{
    public class SchemaGlobal : ISchema
    {
        public ScpGlobal GlobalData { get; private set; }

        public string GetSchemaName(string name)
        {
            return name;
        }

        public bool OnSchemaLoad(ISchemaReader reader)
        {
            CSVReader csv = reader as CSVReader;
            GlobalData = new ScpGlobal();

            // 类型， 值， 名称， 注释， client server
            //_CSV_LIST_BEGIN_
			GlobalData.iIntData = (csv.GetData((int)CSVDataType.Int, (int)CSVIndex.ScpGlobal_IntData, 1) as CSVData_Int).iValue;	// 有符号整形
			GlobalData.sStringData = (csv.GetData((int)CSVDataType.String, (int)CSVIndex.ScpGlobal_StringData, 1) as CSVData_String).sValue;	// 字符串数据
			GlobalData.uUintData = (csv.GetData((int)CSVDataType.Uint, (int)CSVIndex.ScpGlobal_UintData, 1) as CSVData_Uint).uValue;	// uint整形
			GlobalData.ulUlongData = (csv.GetData((int)CSVDataType.Ulong, (int)CSVIndex.ScpGlobal_UlongData, 1) as CSVData_Ulong).ulValue;	// 长整形数据
            //_CSV_LIST_END_

            return true;
        }
    }
}
