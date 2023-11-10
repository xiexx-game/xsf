//////////////////////////////////////////////////////////////////////////
//
// 文件：source/xsf/src/schema/csv_data/CSVData_Int.h
// 作者：Xoen Xie
// 时间：2023/08/14
// 描述：CSV数据 int
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _CSV_DATA_INT_H_
#define _CSV_DATA_INT_H_

#include "CSVData.h"
using namespace xsf;

namespace xsf_scp
{
    class CSVData_Int : public CSVData
    {
    public:
        uint8 GetType() override { return CSVDataType_Int; }

        bool Read(uint32 row, uint32 col, const char * sData) override
        {
            iValue = strtol( sData, 0, 10);

            return true;
        }

    public:
        int32 iValue;
    };

} // namespace xsf


#endif // end of _CSV_DATA_INT_H_