//////////////////////////////////////////////////////////////////////////
//
// 文件：source/xsf/src/schema/csv_data/CSVData_IDAndCount.h
// 作者：Xoen Xie
// 时间：2023/08/14
// 描述：CSV数据 id count组
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _CSV_DATA_ID_AND_COUNT_H_
#define _CSV_DATA_ID_AND_COUNT_H_

#include "CSVData.h"
using namespace xsf;

namespace xsf_scp
{
    struct CSVIdCount
    {
        uint nID = 0;
        uint nCount = 0;
    };

    struct CSVIdCountArray
    {
        uint nTotal = 0;
        CSVIdCount *pDatas = nullptr;

        void Release();
    };

    class CSVData_IDAndCount : public CSVData
    {
    public:
        uint8 GetType() override { return CSVDataType_IDAndCount; }

        bool Read(uint32 row, uint32 col, const char * sData) override;

    public:
        CSVIdCountArray icValue;
    };

} // namespace xsf


#endif // end of _CSV_DATA_H_