//////////////////////////////////////////////////////////////////////////
//
// 文件：source/xsf/src/schema/csv_data/CSVData_String.h
// 作者：Xoen Xie
// 时间：2023/08/14
// 描述：CSV数据 字符串
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _CSV_DATA_STRING_H_
#define _CSV_DATA_STRING_H_

#include "CSVData.h"


namespace xsf_scp
{
    class CSVData_String : public CSVData
    {
    public:
        uint8 GetType() override { return CSVDataType_String; }

        bool Read(uint32 row, uint32 col, const char * sData) override;

    public:
        string sValue;
    };

} // namespace xsf


#endif // end of _CSV_DATA_H_