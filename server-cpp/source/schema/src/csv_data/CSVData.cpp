//////////////////////////////////////////////////////////////////////////
//
// 文件：source/xsf/src/schema/csv_data/CSVData.cpp
// 作者：Xoen Xie
// 时间：2023/08/14
// 描述：CSV数据定义
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#include "CSVData.h"
#include "CSVDataInc.h"
#include "XSF.h"
using namespace xsf;

namespace xsf_scp
{
    CSVData *CSVData::m_Datas[CSVDataType_Max] = { nullptr };

    CSVData * CSVData::GetData(uint8 nType)
    {
        if(m_Datas[nType] == nullptr)
        {
            switch(nType)
            {
            case CSVDataType_Int:  m_Datas[nType] = new CSVData_Int();  break;
            case CSVDataType_Uint:  m_Datas[nType] = new CSVData_Uint();  break;
            case CSVDataType_Ulong:  m_Datas[nType] = new CSVData_Ulong();  break;
            case CSVDataType_Bool:  m_Datas[nType] = new CSVData_Bool();  break;
            case CSVDataType_Float:  m_Datas[nType] = new CSVData_Float();  break;
            case CSVDataType_Array:  m_Datas[nType] = new CSVData_Array();  break;
            case CSVDataType_String:  m_Datas[nType] = new CSVData_String();  break;
            case CSVDataType_IDAndCount:  m_Datas[nType] = new CSVData_IDAndCount();  break;

            default:
                XSF_ERROR("CSVData::GetData type error, type=%d", nType);
                return nullptr;
            }
        }

        return m_Datas[nType];
    }

    CSVData * CSVData::GetDataByName(const char *sName)
    {
        uint8 nType = 0;
        if (strcasecmp(sName, "int") == 0)
            nType = CSVDataType_Int;
        else if (strcasecmp(sName, "uint") == 0)
            nType = CSVDataType_Uint;
        else if (strcasecmp(sName, "ulong") == 0)
            nType = CSVDataType_Ulong;
        else if (strcasecmp(sName, "float") == 0)
            nType = CSVDataType_Float;
        else if (strcasecmp(sName, "bool") == 0)
            nType = CSVDataType_Bool;
        else if (strcasecmp(sName, "string") == 0)
            nType = CSVDataType_String;
        else if (strcasecmp(sName, "array") == 0)
            nType = CSVDataType_Array;
        else if (strcasecmp(sName, "iac") == 0)
            nType = CSVDataType_IDAndCount;
        else
        {
            XSF_ERROR("CSVData GetDataByName name error, name=%s", sName);
        }
        
        return GetData(nType);
    }
}
