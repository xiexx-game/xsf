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
        int nRowIndex = 0;

        //_CSV_LIST_BEGIN_
        GlobalData.iIntData = GET_DATA(CSVData_Int, CSVDataType_Int, nRowIndex++, 1)->iValue;          // 有符号整形
        GlobalData.sStringData = GET_DATA(CSVData_String, CSVDataType_String, nRowIndex++, 1)->sValue; // 字符串数据
        GlobalData.uUintData = GET_DATA(CSVData_Uint, CSVDataType_Uint, nRowIndex++, 1)->uValue;       // uint整形
        GlobalData.ulUlongData = GET_DATA(CSVData_Ulong, CSVDataType_Ulong, nRowIndex++, 1)->ulValue;  // 长整形数据
        //_CSV_LIST_END_

        XSF_INFO("SchemaGlobal::OnSchemaLoad iIntData=%d", GlobalData.iIntData);
        XSF_INFO("SchemaGlobal::OnSchemaLoad sStringData=%s", GlobalData.sStringData.c_str());
        XSF_INFO("SchemaGlobal::OnSchemaLoad uUintData=%u", GlobalData.uUintData);
        XSF_INFO("SchemaGlobal::OnSchemaLoad ulUlongData=%lu", GlobalData.ulUlongData);

        return true;
    }
}