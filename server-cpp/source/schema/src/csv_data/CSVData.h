//////////////////////////////////////////////////////////////////////////
//
// 文件：source/xsf/src/schema/csv_data/CSVData.h
// 作者：Xoen Xie
// 时间：2023/08/14
// 描述：CSV数据定义
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _CSV_DATA_H_
#define _CSV_DATA_H_

#include "XSFDef.h"
#include "IXSFSchema.h"
using namespace xsf;

namespace xsf_scp
{
    enum EMCSVDataType 
    {
        CSVDataType_None = 0,
        CSVDataType_Int,        // 有符号32位整形，配置为空是为0
        CSVDataType_Uint,       // 无符号32位整形，配置为空是为0
        CSVDataType_Ulong,      // 无符号64位整形，配置为空是为0
        CSVDataType_Float,     // 浮点，配置为空是为0
        CSVDataType_String,     // 字符串，配置为空是为空串
        CSVDataType_Bool,       // 布尔值，配置为空为false， 1或者true时为true
        CSVDataType_Array,      // 整形数组类型，配置为空无数据 num1:num2:num3
        CSVDataType_IDAndCount, // ID和count组合，id1:count1|id2:count2|id3:count3

        CSVDataType_Max,
    };


    class CSVData : public ICSVData
    {
    public:
        virtual ~CSVData() {}

        virtual uint8 GetType() = 0;

        static CSVData *GetData(uint8 nType);
        static CSVData *GetDataByName(const char *sName);

    private:
        static CSVData *m_Datas[CSVDataType_Max];
    };

    #define GET_DATA(_DESC, _TYPE, ROW, COL) static_cast<_DESC*>(pReader->GetData(_TYPE, ROW, COL))

} // namespace xsf


#endif // end of _CSV_DATA_H_