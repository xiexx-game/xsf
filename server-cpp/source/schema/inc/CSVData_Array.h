//////////////////////////////////////////////////////////////////////////
//
// 文件：source/xsf/src/schema/csv_data/CSVData_Array.h
// 作者：Xoen Xie
// 时间：2023/08/14
// 描述：CSV数据 uint数组
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _CSV_DATA_ARRAY_H_
#define _CSV_DATA_ARRAY_H_

#include "CSVData.h"


namespace xsf_scp
{
    struct CSVArray
    {
        uint nCount = 0;
        uint * pDatas = nullptr;

        void Release();
    };

    class CSVData_Array : public CSVData
    {
    public:
        uint8 GetType() override { return CSVDataType_Array; }

        bool Read(uint32 row, uint32 col, const char * sData) override;

    public:
        CSVArray arValue;
    };

} // namespace xsf


#endif // end of _CSV_DATA_H_