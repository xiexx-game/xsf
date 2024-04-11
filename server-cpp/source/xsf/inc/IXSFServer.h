//////////////////////////////////////////////////////////////////////////
//
// 文件：source/xsf/inc/IXSFServer.h
// 作者：Xoen Xie
// 时间：2023/08/14
// 描述：Server
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _I_XSF_SERVER_H_
#define _I_XSF_SERVER_H_

#include "XSFDef.h"
#include "XSFServerDef.h"

namespace xsf
{
    struct ModuleInit
    {
        uint16 nModuleID = 0;
        char sName[BUFFER_SIZE_64] = {0};
        bool NoWaitStart = false;
    };

    struct IModule
    {
        virtual ~IModule(void) {}

        uint16 GetModuleID(void) { return m_nID; }

        const char * GetModuleName(void) { return m_sName; }

        bool IsNoWaitStart(void) { return m_bNoWaitStart; }

        virtual bool Init( ModuleInit * pInit ) 
        { 
            m_nID = pInit->nModuleID;
            strcpy(m_sName, pInit->sName);
            m_bNoWaitStart = pInit->NoWaitStart;
            
            return true; 
        }

        virtual void DoRegist(void) {}

        virtual bool Start(void) { return true; }

        virtual void Release(void) { delete this; }

        // 检查本模块是否开启完成
        virtual uint8 OnStartCheck(void) { return ModuleRunCode_OK; }

        // 检查本模块是否关闭完成
        virtual uint8 OnCloseCheck(void) { return ModuleRunCode_OK; }

        virtual void OnUpdate(uint32 nDeltaTime) {}

        // 服务器所有模块开启完毕后调用
        virtual void OnOK(void) {}

        // 服务器所有模块都检测关闭完成后调用
        virtual void DoClose(void) {}

         // 服务器关服前调用
        virtual void OnStartClose() {}

        virtual void Stop(void) {}

    private:
        bool m_bNoWaitStart;

    protected:
        uint16 m_nID = 0;
        char m_sName[BUFFER_SIZE_64] = {0};
    };


    //////////////////////////////////////////////////////////////////////////
    //
    //  One Server
    //
    //////////////////////////////////////////////////////////////////////////
    struct ServerInit
    {
        bool OutputConsole = false;
        char sTag[BUFFER_SIZE_64] = {0};
        uint8 nLogLevel = 0;
        char WorkDir[BUFFER_SIZE_1024] = {0};
    };

    enum EMServerRunStatus
    {
        ServerRunStatus_None = 0,
        ServerRunStatus_Ready,          // 服务器已准备好
        ServerRunStatus_Open,           // 服务器已开放登录
    };

    struct IServer
    {
        virtual ~IServer(void) {}

        // 获取当前服务器ID
        virtual const SID * GetSID(void) = 0;

        virtual const uint32 * GetPorts(void) = 0;

        virtual const ServerInit * GetInitData(void) = 0;

        virtual const XSFConfig * GetConfig(void) = 0;

        virtual bool IsRunning(void) = 0;

        virtual void SetPort(uint8 nEP, uint32 nPort) = 0;

        virtual void Init(uint8 nLocalEP, int32 argc, char* argv[]) = 0;

        virtual void SetID(uint32 nID) = 0;

        virtual void Run(void) = 0;

        virtual void AddModule(IModule *pModule, ModuleInit * pInit) = 0;

        virtual IModule * GetModule( uint16 nModuleID ) = 0;

        virtual void DoStart(void) = 0;

        virtual void Stop(void) = 0;

        virtual void SpeedUp(void) = 0;
    };


    #define GET_MODULE( _DEST, _MODULE_ID, _CLASS )     XSF_CAST(_DEST, XSFCore::GetServer()->GetModule(_MODULE_ID), _CLASS );

} // namespace xsf


#endif      // end of _I_XSF_SERVER_H_