//////////////////////////////////////////////////////////////////////////
//
// 文件：source/xsf/src/schema/csv_data/CSVData_IDAndCount.cpp
// 作者：Xoen Xie
// 时间：2023/08/14
// 描述：CSV数据 id count组
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#include "CSVData_IDAndCount.h"
#include "XSFMallocHook.h"
#include "XSF.h"

namespace xsf_scp
{
    void CSVIdCountArray::Release()
    {
        nTotal = 0;
        XSF_FREE(pDatas);
    }

    bool CSVData_IDAndCount::Read(uint32 row, uint32 col, const char * sData)
    {
        icValue.nTotal = 0;
        icValue.pDatas = nullptr;

        if(strlen(sData) <= 0)
            return true;

        vector<string> datas;
        XSFCore::Split(sData, '|', datas);

        icValue.nTotal = datas.size();
        icValue.pDatas = (CSVIdCount*)xsf_malloc(sizeof(CSVIdCount)*icValue.nTotal);
        for(uint i = 0; i < icValue.nTotal; i ++)
        {
            vector<string> idc;
            XSFCore::Split(datas[i].c_str(), ':', idc);
            if(idc.size() != 2)
            {
                XSF_ERROR("CSVData_IDAndCount::Read idc.size() != 2, row=%d, col=%d, data=%s, idc=%s", row, col, sData, datas[i].c_str());
                return false;
            }

            icValue.pDatas[i].nID = strtoul(idc[0].c_str(), 0, 10);
            icValue.pDatas[i].nCount = strtoul(idc[1].c_str(), 0, 10);
        }

        return true;
    }
}
