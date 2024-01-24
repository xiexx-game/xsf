//////////////////////////////////////////////////////////////////////////
//
// 文件：schema\src\schemas\SchemaGlobal.cpp
// 作者：Unity editor
// 时间：2020/09/16
// 描述：全局变量配置
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#include "SchemaGlobal.h"
#include "SchemaIndex.h"

#include "XSF.h"
#include "CSVDataInc.h"

namespace xsf_scp
{
    bool SchemaGlobal::OnSchemaLoad(int32 nSchemaID, ISchemaReader *pReader)
    {
        // 类型， 值， 名称， 注释， client server
        //_CSV_LIST_BEGIN_
		GlobalData.iIntData = GET_DATA(CSVData_Int, CSVDataType_Int, CSVIndex_ScpGlobal_IntData, 3)->iValue;	// 有符号整形
		GlobalData.sStringData = GET_DATA(CSVData_String, CSVDataType_String, CSVIndex_ScpGlobal_StringData, 3)->sValue;	// 字符串数据
		GlobalData.uUintData = GET_DATA(CSVData_Uint, CSVDataType_Uint, CSVIndex_ScpGlobal_UintData, 3)->uValue;	// uint整形
		GlobalData.ulUlongData = GET_DATA(CSVData_Ulong, CSVDataType_Ulong, CSVIndex_ScpGlobal_UlongData, 3)->ulValue;	// 长整形数据
		GlobalData.fFloatData = GET_DATA(CSVData_Float, CSVDataType_Float, CSVIndex_ScpGlobal_FloatData, 3)->fValue;	// 浮点数
		GlobalData.arUintArrayData = GET_DATA(CSVData_Array, CSVDataType_Array, CSVIndex_ScpGlobal_UintArrayData, 3)->arValue;	// 无符号整形数组
		GlobalData.bBoolData = GET_DATA(CSVData_Bool, CSVDataType_Bool, CSVIndex_ScpGlobal_BoolData, 3)->bValue;	// 布尔数据
		GlobalData.icIacData = GET_DATA(CSVData_IDAndCount, CSVDataType_IDAndCount, CSVIndex_ScpGlobal_IacData, 3)->icValue;	// id和count数据
		GlobalData.uEnergyItem = GET_DATA(CSVData_Uint, CSVDataType_Uint, CSVIndex_ScpGlobal_EnergyItem, 3)->uValue;	// 体力道具ID
        //_CSV_LIST_END_

        XSF_INFO("SchemaGlobal::OnSchemaLoad iIntData=%d", GlobalData.iIntData);
        XSF_INFO("SchemaGlobal::OnSchemaLoad sStringData=%s", GlobalData.sStringData.c_str());
        XSF_INFO("SchemaGlobal::OnSchemaLoad uUintData=%u", GlobalData.uUintData);
        XSF_INFO("SchemaGlobal::OnSchemaLoad ulUlongData=%lu", GlobalData.ulUlongData);

        return true;
    }
}
