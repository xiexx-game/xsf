//////////////////////////////////////////////////////////////////////////
//
// 文件：source/xsf/src/schema/csv_data/CSVData_Bool.h
// 作者：Xoen Xie
// 时间：2023/08/14
// 描述：CSV数据 bool
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _CSV_DATA_BOOL_H_
#define _CSV_DATA_BOOL_H_

#include "CSVData.h"
using namespace xsf;

namespace xsf_scp
{
    class CSVData_Bool : public CSVData
    {
    public:
        uint8 GetType() override { return CSVDataType_Bool; }

        bool Read(uint32 row, uint32 col, const char * sData) override
        {
            bValue = false;

            if(strcasecmp(sData, "1") == 0 || strcasecmp(sData, "true") == 0)
                bValue = true;

            return true;
        }

    public:
        bool bValue;
    };

} // namespace xsf


#endif // end of _CSV_DATA_BOOL_H_