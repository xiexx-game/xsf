//////////////////////////////////////////////////////////////////////////
//
// 文件：message\src\MessageModule.cpp
// 作者：Xoen Xie
// 时间：2020/05/29
// 描述：消息模块
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#include "MessageModule.h"
#include "DMessageInc.h"

namespace xsf_msg
{
    #define NEW_MESSAGE(_NAME, _ID)     m_Messages[_ID] = new MSG_##_NAME();

    bool MessageModule::Init(ModuleInit *pInit)
    {
        NEW_MESSAGE(Clt_Gt_Handshake, xsf_pbid::CMSGID::Clt_Gt_Handshake)
        NEW_MESSAGE(Clt_Gt_Handshake, xsf_pbid::CMSGID::Clt_Gt_Handshake)
        NEW_MESSAGE(Gt_Clt_Handshake, xsf_pbid::CMSGID::Gt_Clt_Handshake)
        NEW_MESSAGE(Clt_Gt_Heartbeat, xsf_pbid::CMSGID::Clt_Gt_Heartbeat)
        NEW_MESSAGE(Gt_Clt_Heartbeat, xsf_pbid::CMSGID::Gt_Clt_Heartbeat)
        NEW_MESSAGE(Gt_Clt_Disconnect, xsf_pbid::CMSGID::Gt_Clt_Disconnect)
        NEW_MESSAGE(Clt_G_Login, xsf_pbid::CMSGID::Clt_G_Login)
        NEW_MESSAGE(G_Clt_LoginResult, xsf_pbid::CMSGID::G_Clt_LoginResult)
        NEW_MESSAGE(G_Clt_TestData, xsf_pbid::CMSGID::G_Clt_TestData)
        NEW_MESSAGE(Cc_C_Handshake, xsf_pbid::SMSGID::Cc_C_Handshake)
        NEW_MESSAGE(C_Cc_Handshake, xsf_pbid::SMSGID::C_Cc_Handshake)
        NEW_MESSAGE(Cc_C_Heartbeat, xsf_pbid::SMSGID::Cc_C_Heartbeat)
        NEW_MESSAGE(Cc_C_ServerInfo, xsf_pbid::SMSGID::Cc_C_ServerInfo)
        NEW_MESSAGE(C_Cc_ServerInfo, xsf_pbid::SMSGID::C_Cc_ServerInfo)
        NEW_MESSAGE(Cc_C_ServerLost, xsf_pbid::SMSGID::Cc_C_ServerLost)
        NEW_MESSAGE(C_Cc_ServerLost, xsf_pbid::SMSGID::C_Cc_ServerLost)
        NEW_MESSAGE(Cc_C_ServerOk, xsf_pbid::SMSGID::Cc_C_ServerOk)
        NEW_MESSAGE(C_Cc_ServerOk, xsf_pbid::SMSGID::C_Cc_ServerOk)
        NEW_MESSAGE(C_Cc_Stop, xsf_pbid::SMSGID::C_Cc_Stop)
        NEW_MESSAGE(Gt_GtA_Handshake, xsf_pbid::SMSGID::Gt_GtA_Handshake)
        NEW_MESSAGE(GtA_Gt_Handshake, xsf_pbid::SMSGID::GtA_Gt_Handshake)
        NEW_MESSAGE(Gt_GtA_Heartbeat, xsf_pbid::SMSGID::Gt_GtA_Heartbeat)
        NEW_MESSAGE(Gt_GtA_ClientClose, xsf_pbid::SMSGID::Gt_GtA_ClientClose)
        NEW_MESSAGE(GtA_Gt_ClientDisconnect, xsf_pbid::SMSGID::GtA_Gt_ClientDisconnect)
        NEW_MESSAGE(GtA_Gt_ClientMessage, xsf_pbid::SMSGID::GtA_Gt_ClientMessage)
        NEW_MESSAGE(GtA_Gt_Broadcast, xsf_pbid::SMSGID::GtA_Gt_Broadcast)
        NEW_MESSAGE(GtA_Gt_SetServerId, xsf_pbid::SMSGID::GtA_Gt_SetServerId)

        XSFCore::mMessageHelper = this;
        
        return IModule::Init(pInit);
    }



    void MessageModule::Release(void)
    {
        for (int32 i = 0; i < xsf_pbid::SMSGID_Max; ++i)
        {
            if (m_Messages[i] != nullptr)
            {
                m_Messages[i]->Release();
                m_Messages[i] = nullptr;
            }
        }

        google::protobuf::ShutdownProtobufLibrary();

        delete this;
    }

    IMessage *MessageModule::GetMessage(uint16 nID)
    {
        if (nID >= xsf_pbid::SMSGID_Max)
        {
            XSF_ERROR("MessageModule::GetMessage nID:%u >= MSGID_Max", nID);
            return nullptr;
        }

        return m_Messages[nID];
    }

    void SetMessageModule(void)
    {
        MessageModule *pMessageModule = new MessageModule();

        ModuleInit *pInit = new ModuleInit();
        pInit->nModuleID = MESSAGE_MODULE_ID;
        strcpy(pInit->sName, "MessageModule");
        pInit->NoWaitStart = true;

        XSFCore::GetServer()->AddModule(pMessageModule, pInit);
    }
}
