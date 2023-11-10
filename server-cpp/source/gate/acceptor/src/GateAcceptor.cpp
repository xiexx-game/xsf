//////////////////////////////////////////////////////////////////////////
//
// 文件：source/gate/acceptor/src/GateAcceptor.cpp
// 作者：Xoen Xie
// 时间：2020/08/06
// 描述：网关接收器
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#include "GateAcceptor.h"
#include "Gate.h"
#include "GtaExecutors.h"
#include "NetPoint.h"

namespace GateA
{
    void GateAcceptor::DoRegist(void)
    {
        XSFCore::SetMessageExecutor(xsf_pbid::Gt_GtA_Handshake, new Executor_Gt_GtA_Handshake());
        XSFCore::SetMessageExecutor(xsf_pbid::Gt_GtA_Heartbeat, new Executor_Gt_GtA_Heartbeat());
        XSFCore::SetMessageExecutor(xsf_pbid::Gt_GtA_ClientClose, new Executor_Gt_GtA_ClientClose());
    }

    void GateAcceptor::Release(void)
    {
        XSF_DELETE(m_pHandler);

        IGateAcceptor::Release();
    }

    uint16 GateAcceptor::GetListenPort(void)
    {
        const xsf::uint32 * ports = XSFCore::GetServer()->GetPorts();
        return ports[EP_Gate];
    }

    NetPoint *GateAcceptor::NewNP(void)
    {
        return new Gate();
    }

    // 断开一个客户端
    void GateAcceptor::DisconnectClient(uint32 nClientID, uint32 nReason)
    {
        XSF_CAST(pMessage, XSFCore::GetMessage(xsf_pbid::GtA_Gt_ClientDisconnect), xsf_msg::MSG_GtA_Gt_ClientDisconnect);
        pMessage->mPB.set_client_id(nClientID);
        pMessage->mPB.set_reason(nReason);

        SID sid;
        sid.ID = nClientID;
        SendMessage2Gate(sid.C.gate, pMessage);
    }

    // 发送一个消息到网关
    void GateAcceptor::SendMessage2Gate(uint32 nGateID, IMessage *pMessage)
    {
        auto np = GetNetPoint(nGateID);
        if(np != nullptr)
        {
            np->SendMessage(pMessage);
        }
        else
        {
            XSF_ERROR("GateAcceptor::SendMessage2Gate gate not found, gate id=%u", nGateID);
        }
    }

    // 发送消息到一个客户端
    void GateAcceptor::SendMessage2Client(uint32 nClientID, IMessage *pMessage)
    {
        XSF_CAST(pMessageWrap, XSFCore::GetMessage(xsf_pbid::GtA_Gt_ClientMessage), xsf_msg::MSG_GtA_Gt_ClientMessage);
        pMessageWrap->mPB.clear_client_id();
        pMessageWrap->mPB.add_client_id(nClientID);

        uint32 nLength = 0;
        const byte *pData = XSFCore::GetClientPacker()->Pack(pMessage, nLength);

        pMessageWrap->mPB.set_client_message(pData, nLength);

        SID sid;
        sid.ID = nClientID;
        SendMessage2Gate(sid.C.gate, pMessageWrap);
    }

    // 广播消息到所有客户端
    void GateAcceptor::Broadcast2AllClient(IMessage *pMessage)
    {
        uint32 nLength = 0;
        const byte *pData = XSFCore::GetClientPacker()->Pack(pMessage, nLength);

        XSF_CAST(pMessageWrap, XSFCore::GetMessage(xsf_pbid::GtA_Gt_Broadcast), xsf_msg::MSG_GtA_Gt_Broadcast);
        pMessageWrap->mPB.set_client_message(pData, nLength);

        Broadcast(pMessageWrap, 0);
    }

    // 设置客户端转发内部的指定EP的服务器ID
    void GateAcceptor::SetServerID(uint32 nClientID, uint8 nEP, uint32 nServerID)
    {
        XSF_CAST(pMessage, XSFCore::GetMessage(xsf_pbid::GtA_Gt_SetServerId), xsf_msg::MSG_GtA_Gt_SetServerId);
        pMessage->mPB.set_client_id(nClientID);
        pMessage->mPB.set_ep(nEP);
        pMessage->mPB.set_server_id(nServerID);

        SID sid;
        sid.ID = nClientID;
        SendMessage2Gate(sid.C.gate, pMessage);
    }

    // 开始广播消息到指定客户端
    void GateAcceptor::BeginBroadcast()
    {
        for(uint32 i = 0; i < m_nSize; i ++)
        {
            XSF_CAST(pGate, m_pNetPoints[i], Gate);
            if(pGate != nullptr)
            {
                pGate->Clear();
            }
        }
    }

    // 添加广播消息的客户端
    void GateAcceptor::PushClientID(uint nClientID)
    {
        SID sid;
        sid.ID = nClientID;

        XSF_CAST(pGate, m_pNetPoints[sid.C.gate], Gate);
        pGate->Add(nClientID);
    }

    // 广播消息
    void GateAcceptor::EndBroadcast(IMessage * pMessage)
    {
        uint32 nLength = 0;
        const byte *pData = XSFCore::GetClientPacker()->Pack(pMessage, nLength);

        for(uint32 i = 0; i < m_nSize; i ++)
        {
            XSF_CAST(pGate, m_pNetPoints[i], Gate);
            if(pGate != nullptr)
            {
                pGate->Broadcast(pData, nLength);
            }
        }
    }

    void SetGateAcceptor(uint16 nModuleID, IGateHandler *pHandler)
    {
        FastNetPointManagerInit *pInit = new FastNetPointManagerInit();
        pInit->nModuleID = nModuleID;
        strcpy(pInit->sName, "GateAcceptor");
        pInit->nMaxSize = XSFCore::GetServer()->GetConfig()->MaxGate;

        GateAcceptor::Instance()->SetHandler(pHandler);

        XSFCore::GetServer()->AddModule(GateAcceptor::Instance(), pInit);
    }

    IGateAcceptor * GetGateAcceptor(void)
    {
        return GateAcceptor::Instance();
    }
}