//////////////////////////////////////////////////////////////////////////
//
// 文件：source/xsf/src/schema/csv_data/CSVData_Float.h
// 作者：Xoen Xie
// 时间：2023/08/14
// 描述：CSV数据 float
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _CSV_DATA_FLOAT_H_
#define _CSV_DATA_FLOAT_H_

#include "CSVData.h"
using namespace xsf;

namespace xsf_scp
{
    class CSVData_Float : public CSVData
    {
    public:
        uint8 GetType() override { return CSVDataType_Float; }

        bool Read(uint32 row, uint32 col, const char * sData) override
        {
            fValue = strtof(sData, nullptr);

            return true;
        }

    public:
        float fValue;
    };

} // namespace xsf


#endif // end of _CSV_DATA_FLOAT_H_