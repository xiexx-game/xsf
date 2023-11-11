//////////////////////////////////////////////////////////////////////////
//
// 文件：schema\src\schemas\SchemaItem.cpp
// 作者：Xoen Xie
// 时间：11/10/2023
// 描述：物品配置
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#include "SchemaItem.h"
#include "SchemaIndex.h"

#include "XSF.h"
#include "CSVDataInc.h"

namespace xsf_scp
{
    bool SchemaItem::OnSchemaLoad(int32 nSchemaID, ISchemaReader *pReader)
    {
        for(uint32 i = 0; i < pReader->RowCount(); i ++)
        {
            ScpItem * scp = new ScpItem();
            //_CSV_LIST_BEGIN_
			scp->uId = GET_DATA(CSVData_Uint, CSVDataType_Uint, i, CSVIndex_ScpItem_id)->uValue;;	// 编号
			scp->iType = GET_DATA(CSVData_Int, CSVDataType_Int, i, CSVIndex_ScpItem_type)->iValue;;	// 类型
			scp->fParam1 = GET_DATA(CSVData_Float, CSVDataType_Float, i, CSVIndex_ScpItem_param1)->fValue;;	// 参数1
			scp->ulParam2 = GET_DATA(CSVData_Ulong, CSVDataType_Ulong, i, CSVIndex_ScpItem_param2)->ulValue;;	// 参数2
			scp->arParam3 = GET_DATA(CSVData_Array, CSVDataType_Array, i, CSVIndex_ScpItem_param3)->arValue;;	// 参数3
			scp->icParam4 = GET_DATA(CSVData_IDAndCount, CSVDataType_IDAndCount, i, CSVIndex_ScpItem_param4)->icValue;;	// 参数4
			scp->bParam5 = GET_DATA(CSVData_Bool, CSVDataType_Bool, i, CSVIndex_ScpItem_param5)->bValue;;	// 参数5
            //_CSV_LIST_END_

            if(!(m_Datas.insert(DataMap::value_type(scp->uId, scp)).second))
            {
                XSF_ERROR("SchemaItem::OnSchemaLoad id exist, id=%u", scp->uId);
                return false;
            }
        }

        return true;
    }

    void *SchemaItem::Get(uint32 nID1, uint32 nID2)
    {
        auto it = m_Datas.find(nID1);
        if(it != m_Datas.end())
            return it->second;
    
        return nullptr;
    }
}
