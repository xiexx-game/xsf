//////////////////////////////////////////////////////////////////////////
//
// 文件：source/gate/acceptor/inc/IGateAcceptor.h
// 作者：Xoen Xie
// 时间：2020/08/06
// 描述：网关接受器
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _I_GATE_ACCEPTOR_H_
#define _I_GATE_ACCEPTOR_H_

#include "NetPointManager.h"
using namespace xsf;

namespace GateA
{
    struct IGateAcceptor : public FastNetPointManager
    {
        virtual ~IGateAcceptor(void) {}

        // 断开一个客户端
        virtual void DisconnectClient(uint32 nClientID, uint32 nReason) = 0;

        // 发送一个消息到网关
        virtual void SendMessage2Gate(uint32 nGateID, IMessage *pMessage) = 0;

        // 发送消息到一个客户端
        virtual void SendMessage2Client(uint32 nClientID, IMessage *pMessage) = 0;

        // 广播消息到所有客户端
        virtual void Broadcast2AllClient(IMessage *pMessage) = 0;

        // 设置客户端转发内部的指定EP的服务器ID
        virtual void SetServerID(uint32 nClientID, uint8 nEP, uint32 nServerID) = 0;

        // 开始广播消息到指定客户端
        virtual void BeginBroadcast() = 0;

        // 添加广播消息的客户端
        virtual void PushClientID(uint nClientID) = 0;

        // 广播消息
        virtual void EndBroadcast(IMessage * pMessage) = 0;
    };

    struct IGateHandler
    {
        virtual ~IGateHandler(void) {}

        virtual void OnClientClose(uint32 nClientID) = 0;
    };

    void SetGateAcceptor(uint16 nModuleID, IGateHandler *pHandler);
    IGateAcceptor * GetGateAcceptor(void);
}

#endif // end of _I_GATE_ACCEPTOR_MODULE_H_