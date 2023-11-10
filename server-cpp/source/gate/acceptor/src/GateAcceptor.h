//////////////////////////////////////////////////////////////////////////
//
// 文件：source/gate/acceptor/src/GateAcceptor.h
// 作者：Xoen Xie
// 时间：2020/08/06
// 描述：网关接收器
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _GATE_ACCEPTOR_H_
#define _GATE_ACCEPTOR_H_

#include "IGateAcceptor.h"
#include "Singleton.h"

namespace GateA
{
    class GateAcceptor : public IGateAcceptor, public Singleton<GateAcceptor>
    {
    public:
        GateAcceptor(void) {}
        ~GateAcceptor(void) {}

        void SetHandler(IGateHandler *pHandler) { m_pHandler = pHandler; }
        IGateHandler *GetHandler(void) { return m_pHandler; }

    public:
        void DoRegist(void) override;

        void Release(void) override;
    protected:
        uint16 GetListenPort(void) override;

        NetPoint * NewNP(void) override;

    public:
        // 断开一个客户端
        void DisconnectClient(uint32 nClientID, uint32 nReason) override;

        // 发送一个消息到网关
        void SendMessage2Gate(uint32 nGateID, IMessage *pMessage) override;

        // 发送消息到一个客户端
        void SendMessage2Client(uint32 nClientID, IMessage *pMessage) override;

        // 广播消息到所有客户端
        void Broadcast2AllClient(IMessage *pMessage) override;

        // 设置客户端转发内部的指定EP的服务器ID
        void SetServerID(uint32 nClientID, uint8 nEP, uint32 nServerID) override;

        // 开始广播消息到指定客户端
        void BeginBroadcast() override;

        // 添加广播消息的客户端
        void PushClientID(uint nClientID) override;

        // 广播消息
        void EndBroadcast(IMessage * pMessage) override;


    private:
        IGateHandler *m_pHandler = nullptr;
    };
}

#endif // end of _GATE_ACCEPTOR_H_