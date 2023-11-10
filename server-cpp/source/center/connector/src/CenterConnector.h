//////////////////////////////////////////////////////////////////////////
//
// 文件：center\connector\src\CenterConnector.h
// 作者：Xoen Xie
// 时间：2020/06/11
// 描述：中心服连接器
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _CENTER_CONNECTOR_H_
#define _CENTER_CONNECTOR_H_

#include "NetConnector.h"
using namespace xsf;

namespace xsf_msg
{
    class MSG_C_Cc_ServerInfo;
}

namespace CC
{
    struct IServerInfoHandler;

    class CenterConnector : public NetConnector
    {
        using InfoMap = unordered_map<uint32, ServerInfo *>;

    public:
        CenterConnector(void) {}
        ~CenterConnector(void) {}

    public:
        void DoRegist() override;

        bool Start() override;

        void Release(void) override;

        uint8 OnStartCheck() override;

        void OnOK(void) override;

    protected:
        void SendHandshake(void) override;
        void SendHeartbeat(void) override;

    public:
        void SetHandler(IServerInfoHandler *pHandler) { m_pHandler = pHandler; }
        void AddInfo(xsf_msg::MSG_C_Cc_ServerInfo * pMessage);
        void OnNodeLost(uint32 nID);
        void OnNodeOk(uint32 nID);

    private:
        InfoMap m_Nodes;
        IServerInfoHandler *m_pHandler = nullptr;
    };
}

#endif // end of _CENTER_CONNECTOR_H_