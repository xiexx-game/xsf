//////////////////////////////////////////////////////////////////////////
//
// 文件：schema\src\SchemaModule.cpp
// 作者：Xoen Xie
// 时间：2020/05/21
// 描述：配置模块
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#include "SchemaModule.h"
#include "XSF.h"
#include "SchemaLoad.h"
#include "SchemaHeaders.h"
#include "CSVData.h"

using namespace xsf;

namespace xsf_scp
{
    bool SchemaModule::LoadSchema(uint32 nSchemaID, const char *sName, uint8 nType)
    {
        if (m_SchemaList[nSchemaID] == nullptr)
        {
            switch (nSchemaID)
            {
            case SchemaID_Load:
            {
                SchemaLoad *pSchema = new SchemaLoad();
                pSchema->SetModule(this);
                m_SchemaList[nSchemaID] = pSchema;
            }
            break;

            SCHEMA_CREATE

            default:
                break;
            }
        }

        if (m_SchemaList[nSchemaID] == nullptr)
        {
            XSF_ERROR("SchemaModule::LoadSchema m_SchemaList[nSchemaID:%u (%s)] == nullptr", nSchemaID, sName);
            return false;
        }

        char sSchemaPath[BUFFER_SIZE_256] = {0};

        auto pInitData = XSFCore::GetServer()->GetInitData();
        if (SchemaType_CSV == nType)
            sprintf(sSchemaPath, "%s/scp/%s.csv", pInitData->WorkDir, sName);
        else
            sprintf(sSchemaPath, "%s/scp/%s.xml", pInitData->WorkDir, sName);

        if (!XSFCore::FileExists(sSchemaPath))
        {
            XSF_ERROR("SchemaModule::LoadSchema schema not exist, schema:%s, id:%u", sSchemaPath, nSchemaID);
            return false;
        }

        XSF_INFO("Start Load Schema %s:%u", sName, nSchemaID);

        return XSFCore::LoadSchema(m_SchemaList[nSchemaID], nType, nSchemaID, sSchemaPath);
    }

    bool SchemaModule::Start(void)
    {
        XSFCore::mSchemaHelper = this;
        return LoadSchema(SchemaID_Load, "Load", SchemaType_XML);
    }

    void SchemaModule::Release(void)
    {
        for (uint32 i = 0; i < SchemaID_Max; ++i)
        {
            XSF_RELEASE(m_SchemaList[i]);
        }

        delete this;
    }

    ICSVData *SchemaModule::GetData(byte type)
    {
        return CSVData::GetData(type);
    }

    ISchema *SchemaModule::GetSchema(uint32 nID)
    {
        return m_SchemaList[nID];
    }

    void SetSchemaModule(void)
    {
        SchemaModule *pSchemaModule = new SchemaModule();

        ModuleInit *pInit = new ModuleInit();
        pInit->nModuleID = SCHEMA_MODULE_ID;
        strcpy(pInit->sName, "SchemaModule");
        pInit->NoWaitStart = true;

        XSFCore::GetServer()->AddModule(pSchemaModule, pInit);
    }
}