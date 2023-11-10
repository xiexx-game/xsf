//////////////////////////////////////////////////////////////////////////
//
// 文件：source/xsf/src/schema/csv_data/CSVData_String.cpp
// 作者：Xoen Xie
// 时间：2023/08/14
// 描述：CSV数据 字符串
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#include "CSVData_String.h"
#include "XSFMallocHook.h"
#include "XSF.h"

namespace xsf_scp
{
    bool CSVData_String::Read(uint32 row, uint32 col, const char * sData)
    {
        sValue = sData;
        return true;
    }
}
