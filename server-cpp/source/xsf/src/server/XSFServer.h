//////////////////////////////////////////////////////////////////////////
//
// 文件：source/xsf/src/server/XSFServer.h
// 作者：Xoen Xie
// 时间：2023/08/14
// 描述：XSF Server
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _XSF_SERVER_H_
#define _XSF_SERVER_H_

#include "XSFDef.h"
#include "IXSFServer.h"

#include "XSFMallocHook.h"
#include "Singleton.h"

namespace xsf
{
    // 服务器运行状态
    enum EMServerStatus
    {
        ServerStatus_None = 0,
        ServerStatus_Init,
        
        ServerStatus_WaitStart,
        ServerStatus_Start,
        ServerStatus_RunningCheck,
        ServerStatus_Running,
        ServerStatus_StopCheck0,
        ServerStatus_StopCheck,
        ServerStatus_Close,
        ServerStatus_Release,
    };

    struct ModuleInfo
    {
        IModule * pModule = nullptr;
        ModuleInit *pInit;
    };

    class XSFLogger;
    class XSFTimer;
    class XSFNet;

    class XSFServer : public IServer, public Singleton<XSFServer>
    {
    public:
        XSFServer(void) 
        {
            m_SID.ID = 0;
        }

        ~XSFServer(void) {}

        void Release(void);

        XSFLogger * GetLogger(void) { return m_pLogger; }
        XSFTimer * GetTimer(void) { return m_pTimer; }
        XSFNet * GetNet(void) { return m_pNet; }

    public:
        // 获取当前服务器ID
        const SID * GetSID(void) override { return &m_SID; }

        const uint32 * GetPorts(void) override { return m_nPorts; }

        const ServerInit * GetInitData(void) override { return &m_InitData; }

        const XSFConfig * GetConfig(void) override { return &m_Config; }

        virtual bool IsRunning(void) { return m_nStatus == ServerStatus_Running; }

        void SetPort(uint8 nEP, uint32 nPort) {
            m_nPorts[nEP] = nPort;
        }

        void Init(uint8 nLocalEP, int32 argc, char* argv[]) override;

        void SetID(uint32 nID) override {
            m_SID.ID = nID;
        }

        void Run(void) override;

        void AddModule(IModule *pModule, ModuleInit * pInit) override;

        IModule * GetModule( uint16 nModuleID ) override { return m_Modules[nModuleID]; }

        void DoStart(void) override;

        void Stop(void) override;

        void SpeedUp(void) override;

    private:
        bool ReadArgv(int argc, char* argv[]);
        bool ReadConfig();
        void StatusUpdate(void);

    private:
        SID m_SID;
        uint32 m_nPorts[EP_Max] = {0};
        ServerInit m_InitData;
        XSFConfig m_Config;

        XSFLogger * m_pLogger = nullptr;
        XSFTimer * m_pTimer = nullptr;
        XSFNet * m_pNet = nullptr;

        uint32 m_nModuleCount = 0;
        IModule** m_Modules = nullptr;

        uint16 m_nMaxModuleID = 0;
        vector<ModuleInfo> m_InitList;
        list<IModule*> m_StepModule;
        uint32 m_LastUpdateTime = 0;
        uint8 m_nStatus = ServerStatus_None;
        int32 m_nEventFD = INVALID_FD;
        int32 m_nEpollFD = INVALID_FD;
        bool m_IsWorking = false;
    };

} // namespace xsf

#endif      // end of _XSF_SERVER_H_
