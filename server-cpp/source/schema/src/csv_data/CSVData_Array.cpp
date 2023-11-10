//////////////////////////////////////////////////////////////////////////
//
// 文件：source/xsf/src/schema/csv_data/CSVData_Array.cpp
// 作者：Xoen Xie
// 时间：2023/08/14
// 描述：CSV数据 uint数组
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#include "CSVData_Array.h"
#include "XSFMallocHook.h"
#include "XSF.h"

namespace xsf_scp
{
    void CSVArray::Release()
    {
        nCount = 0;
        XSF_FREE(pDatas);
    }

    bool CSVData_Array::Read(uint32 row, uint32 col, const char * sData)
    {
        arValue.nCount = 0;
        arValue.pDatas = nullptr;

        if(strlen(sData) <= 0)
            return true;

        vector<string> datas;
        XSFCore::Split(sData, ',', datas);

        arValue.nCount = datas.size();
        arValue.pDatas = (uint*)xsf_malloc(sizeof(uint)*arValue.nCount);
        for(uint i = 0; i < arValue.nCount; i ++)
        {
            arValue.pDatas[i] = strtoul(datas[i].c_str(), 0, 10);
        }

        return true;
    }
}
