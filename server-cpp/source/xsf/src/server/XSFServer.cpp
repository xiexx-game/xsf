//////////////////////////////////////////////////////////////////////////
//
// 作者：Xoen Xie
// 时间：2020/04/08
// 文件：base\src\server\XSFServer.cpp
// 描述：Server
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#include "XSFServer.h"
#include "XSFLog.h"
#include "XSFEpoll.h"

#include <sys/eventfd.h>
#include <signal.h>
#include "XSFNet.h"
#include "XSFTimer.h"
#include "XSF.h"
#include "IXSFMessage.h"
#include <algorithm>
#include "XSFLogger.h"

#include <execinfo.h>
#include <dirent.h>
#include <unordered_set>
#include <map>

#include "tinyxml2.h"
using namespace tinyxml2;

namespace xsf
{
    void TERMHandler(int)
    {
        XSF_WARN("server stop by kill ...");
        
        XSFServer::Instance()->Stop();
    }

    void CustomHandler(int)
    {
        XSF_ERROR("server catch core ....");
        //DumpStack();

        char sCoreName[BUFFER_SIZE_256] = {0};
        const char * pName = EP2Name(XSFCore::GetServer()->GetSID()->S.type);
        time_t nTime = XSFCore::Time();
		char sTime[BUFFER_SIZE_64] = {0};
		XSF_TM sttime;
		XSFCore::GetUTCTM(nTime, sttime);
		sprintf(sTime, "%d%d%d-%d%d%d", sttime.year, sttime.mon, sttime.mday, sttime.hour, sttime.min, sttime.sec);
        sprintf( sCoreName, "./%s.%s.core", pName, sTime );

        char sShell[BUFFER_SIZE_512] = {0};
        sprintf( sShell, "gcore -o %s %u", sCoreName, getpid() );
        XSF_ERROR("============================================================");
        XSF_ERROR("sShell=%s", sShell);
        XSF_ERROR("============================================================");

        XSFCore::ShellExec(sShell);
        
        while(true)
        {
            if(  XSFServer::Instance()->GetLogger()->IsAllDone()  )
                break;
            else
            {
                if( XSFCore::Time() > nTime + 10 )
                {
                    break;
                }
            }
        }

        exit(0);
    }

    void XSFServer::Release(void)
    {
        XSF_INFO("XSFServer::Release start");
        XSF_DELETE_ARRAY(m_Modules);

        XSF_INFO("XSFServer::Release core release");
        XSF_RELEASE(m_pNet);
        XSF_RELEASE(m_pTimer);
        XSF_RELEASE(m_pLogger);

        if( m_nEventFD != INVALID_FD )
            EpollRelease(m_nEventFD);

        if( m_nEpollFD != INVALID_FD )
            EpollRelease(m_nEpollFD);

        delete this;
    }

    void XSFServer::SpeedUp(void)
    {
        uint64 nData = 1;
        int32 nRet = write(m_nEventFD, &nData, sizeof(nData));
        if (nRet != sizeof(nData))
        {
            XSF_ERROR("XSFServer::SpeedUp write error, nRet != sizeof(nData), nRet=%d", nRet);
        }
    }


    void XSFServer::Init(uint8 nLocalEP, int argc, char* argv[])
    {
        signal(SIGPIPE, SIG_IGN);
	    signal(SIGTERM, TERMHandler);

        signal(SIGSEGV, CustomHandler);
        signal(SIGFPE,  CustomHandler);
        signal(SIGILL,  CustomHandler);
        signal(SIGINT,  CustomHandler);  
        signal(SIGABRT, CustomHandler);
        signal(SIGXFSZ, CustomHandler);

        m_SID.S.type = nLocalEP;

        do
        {
            if(!ReadArgv(argc, argv))
                break;

            m_pLogger = new XSFLogger();
            if (!m_pLogger->Create(&m_InitData))
                break;

            m_pTimer = new XSFTimer();
            if (!m_pTimer->Create())
                break;

            m_pNet = new XSFNet();
            if (!m_pNet->Create())
                break;

            if(!ReadConfig())
                break;

            m_nEventFD = eventfd(0, 0);
            m_nEpollFD = EpollCreate();
            if (INVALID_FD == m_nEpollFD || INVALID_FD == m_nEventFD )
            {
                XSF_ERROR("XSFServer::Init INVALID_FD == m_nEpollFD || INVALID_FD == m_nEventFD");
                break;
            }

            if( !EpollAdd(m_nEpollFD, m_nEventFD, this) )
            {
                XSF_ERROR("XSFServer::Create EpollAdd error");
                break;
            }

            return;

        } while (false);

        std::exit(0);
    }

    void XSFServer::Run(void)
    {
        EpollEvent SPWaitEvent[MAX_EPOLL_EVENT];
	    uint64 nEventData = 0;
        m_nStatus = ServerStatus_Init;
        m_IsWorking = true;

        while( m_IsWorking )
        {
            int32 nEventCount = EpollWait(m_nEpollFD, SPWaitEvent, 16);
            if (nEventCount > 0)
            {
                EpollEvent * pEvent = &(SPWaitEvent[0]);
                if (this == pEvent->ud)
                {
                    read(m_nEventFD, &nEventData, sizeof(nEventData));
                }
            }

            StatusUpdate();

            bool bContinue = false;
            uint nContinueCount = 0;

            do
            {
                bContinue = false;

                if( m_pNet->Dispatch() )
                    bContinue = true;

                if( m_pTimer->Dispatch() )
                    bContinue = true;
                
                nContinueCount ++;
                if( nContinueCount >= 1000 )    // 如果已经执行太久了，就强制让出一下cpu
                    break;

            } while (bContinue);
        }

        Release();

        cout << "server shutdown ...." << endl;
    }

    void XSFServer::AddModule(IModule *pModule, ModuleInit * pInit)
    {
        if(pModule == nullptr || pInit == nullptr)
        {
            XSF_ERROR("XSFServer::AddModule pModule == nullptr || pInit == nullptr");
            return;
        }

        ModuleInfo info;
        info.pModule = pModule;
        info.pInit = pInit;
        m_InitList.push_back(info);

        if(info.pInit->nModuleID > m_nMaxModuleID)
            m_nMaxModuleID = info.pInit->nModuleID;
    }

    void XSFServer::DoStart(void)
    {
        if(m_nStatus == ServerStatus_WaitStart)
        {
            XSF_INFO("XSFServer::DoStart call ....");
            m_nStatus = ServerStatus_Start;
        }
    }

    // 停止服务器
    void XSFServer::Stop(void)
    {
        if(m_nStatus == ServerStatus_Running)
            m_nStatus = ServerStatus_StopCheck0;
        else
            m_nStatus = ServerStatus_Release;
    }


    bool XSFServer::ReadArgv(int32 argc, char* argv[])
    {
        char sOut[BUFFER_SIZE_64] = { 0 };

        if(!XSFCore::GetArgValue(argc, argv, "-tag", m_InitData.sTag) || strlen(m_InitData.sTag) <= 0)
        {
            cout << "启动参数中未制定 -tag 值" << endl;
            return false;
        }

        m_InitData.OutputConsole = XSFCore::GetArgValue(argc, argv, "-c", sOut);

        if(XSFCore::GetArgValue(argc, argv, "-i", sOut))
        {
            m_SID.S.server = strtol( sOut, 0, 10);
        }
        else
        {
            m_SID.S.server = 100;
        }

        if(!XSFCore::GetArgValue(argc, argv, "-rd", m_InitData.WorkDir))
        {
            XSFCore::GetWorkPath(m_InitData.WorkDir, sizeof(m_InitData.WorkDir));
        }

        return true;
    }

    bool XSFServer::ReadConfig()
    {
        char sConfigFile[BUFFER_SIZE_1024] = { 0 };
        sprintf(sConfigFile, "%s/config/%s/config.xml", m_InitData.WorkDir, m_InitData.sTag);

        XSF_INFO("XSFServer::ReadConfig load config path:[%s]", sConfigFile);

        if( !XSFCore::FileExists(sConfigFile) )
        {
            XSF_ERROR("XSFServer::ReadConfig global.xml not exist, path=%s", sConfigFile);
            return false;
        }

        XMLDocument doc;
        XMLElement* rootElement = nullptr;
        XMLError errID = doc.LoadFile( sConfigFile );
        if ( XMLError::XML_SUCCESS != errID )
        {
            XSF_ERROR( "XSFServer::ReadConfig, global error:[%d] sConfigFile:%s", errID, sConfigFile );
            return false;
        }

        rootElement = doc.FirstChildElement("root");
        if( rootElement == nullptr )
        {
            XSF_ERROR("XSFServer::ReadConfig, global no root element");
            return false;
        }

        //////////////////////////////////////////////////////////////////////////
        // 读取基础配置
        XMLElement * pConfigEle = XML_FIRST_CHILD(rootElement, "config");
        if( pConfigEle == nullptr )
        {
            XSF_ERROR("XSFServer::ReadConfig config element not found");
            return false;
        }

        XML_TEXT(pConfigEle, "name", m_Config.sName);
        XML_TEXT(pConfigEle, "desc", m_Config.sDesc);
        XML_BOOL(pConfigEle, "auto_start", m_Config.AutoStart);
        XML_UINT(pConfigEle, "htcheck", m_Config.HeartbeatCheck);
        XML_UINT(pConfigEle, "httimeout", m_Config.HeartbeatTimeout);
        XML_UINT(pConfigEle, "htinterval", m_Config.HeartbeatInterval);
        XML_UINT(pConfigEle, "rc_interval", m_Config.ReconnectInterval);

        XML_UINT(pConfigEle, "client_htcheck", m_Config.ClientHeartbeatCheck);
        XML_UINT(pConfigEle, "client_httimeout", m_Config.ClientHeartbeatTimeout);
        XML_TEXT(pConfigEle, "main_center_ip", m_Config.MainCenterIP);
        XML_UINT(pConfigEle, "center_port", m_Config.CenterPort);
        XML_UINT(pConfigEle, "inner_port_start", m_Config.InnerPortStart);
        XML_UINT(pConfigEle, "out_port_start", m_Config.OutPortStart);
        XML_UINT(pConfigEle, "gate_max_count", m_Config.GateMaxCount);
        XML_TEXT(pConfigEle, "nats", m_Config.Nats);
        XML_UINT(pConfigEle, "account_lifetime", m_Config.AccountLifeTime);
        XML_UINT(pConfigEle, "actor_save", m_Config.ActorSaveTime);
        XML_UINT(pConfigEle, "client_max_msg_length", m_Config.ClientMaxMsgLength);
        XML_UINT(pConfigEle, "max_gate", m_Config.MaxGate);
        XML_UINT(pConfigEle, "actor_release", m_Config.ActorReleaseTime);
        
        m_Config.Me.nEP = m_SID.S.type;
        if(m_SID.S.type == EP_Center)
        {
            m_SID.S.index = 1;
        }

        XMLElement * pServerEle = XML_FIRST_CHILD(rootElement, "server");
        XMLElement* pServerItemEle = XML_FIRST_CHILD( pServerEle, nullptr );
        while(pServerItemEle != nullptr)
        {
            char ep[BUFFER_SIZE_32] = {0};
            XML_STRING_ATTR(pServerItemEle, "ep", ep);
            auto nEP = Name2EP(ep);
            const char * sEPName = EP2CNName(nEP);    
            if(nEP == EP_None)
            {
                XSF_ERROR("ServerNode ep error, ep=%s", ep);
                return false;
            }

            uint32 nCount = 0;
            XML_UINT_ATTR(pServerItemEle, "count", nCount);
            if(nCount <= 0)
                nCount = 1;

            for(uint32 i = 0; i < nCount; i ++)
            {
                ServerNode node;
                node.nEP = nEP;
                strcpy(node.sName, sEPName);
                m_Config.NodeList.push_back(node);
            }

            pServerItemEle = XML_NEXT_SIBLING(pServerItemEle);
        }

        bool IsFind = false;
        if(m_SID.S.type == EP_Center)
        {
            IsFind = true;
        }

        for(uint32 i = 0; i < m_Config.NodeList.size(); i ++)
        {
            XSF_INFO(">> Server List: name=%s, type=%s", m_Config.NodeList[i].sName, EP2Name(m_Config.NodeList[i].nEP));
            if(m_Config.NodeList[i].nEP == m_SID.S.type)
            {
                m_Config.Me = m_Config.NodeList[i];
                IsFind = true;
            }
        }

        if(IsFind)
        {
            XSF_INFO(">> Server Find: name=%s, type=%s", m_Config.Me.sName, EP2Name(m_Config.Me.nEP));
            return true;
        }
        else
        {
            XSF_ERROR("服务器启动失败, 未找到ep相关配置, type=%s", EP2Name(m_SID.S.type));
            return false;
        }
    }

    void XSFServer::StatusUpdate(void)
    {
        switch (m_nStatus)
        {
        case ServerStatus_Init:
            {
                m_nMaxModuleID ++;
                m_Modules = new IModule*[m_nMaxModuleID];
                memset(m_Modules, 0, sizeof(IModule*)*m_nMaxModuleID);
                for(auto it = m_InitList.begin(); it != m_InitList.end(); it ++)
                {
                    ModuleInfo info = *it;
                    m_Modules[info.pInit->nModuleID] = info.pModule;
                    XSF_INFO("模块初始化, id=%u, name=%s", info.pInit->nModuleID, info.pInit->sName);
                    if(!info.pModule->Init(info.pInit))
                    {
                        m_nStatus = ServerStatus_Release;
                        return;
                    }

                    delete info.pInit;
                }

                for(uint16 i = 0; i < m_nMaxModuleID; i ++)
                {
                    if(m_Modules[i] != nullptr)
                    {
                        XSF_INFO("模块注册, id=%u, name=%s", m_Modules[i]->GetModuleID(), m_Modules[i]->GetModuleName());
                        m_Modules[i]->DoRegist();
                    }
                }

                int32 WaitStart = 0;
                for(uint16 i = 0; i < m_nMaxModuleID; i ++)
                {
                    if(m_Modules[i] != nullptr)
                    {
                        if(m_Modules[i]->IsNoWaitStart())
                        {
                            XSF_INFO("模块不等待开始, id=%u, name=%s", m_Modules[i]->GetModuleID(), m_Modules[i]->GetModuleName());
                            if(!m_Modules[i]->Start())
                            {
                                m_nStatus = ServerStatus_Release;
                                return;
                            }
                        }
                        else
                        {
                            WaitStart ++;
                        }
                    }
                }

                if(WaitStart > 0)
                {
                    m_nStatus = ServerStatus_WaitStart;
                }
                else
                {
                    m_nStatus = ServerStatus_Start;
                }
            }
            break;

        case ServerStatus_Start:
            {
                for(uint16 i = 0; i < m_nMaxModuleID; i ++)
                {
                    if(m_Modules[i] != nullptr && !m_Modules[i]->IsNoWaitStart())
                    {
                        XSF_INFO("模块等待开始, id=%u, name=%s", m_Modules[i]->GetModuleID(), m_Modules[i]->GetModuleName());
                        if(!m_Modules[i]->Start())
                        {
                            m_nStatus = ServerStatus_Release;
                            return;
                        }
                    }
                }

                for(uint16 i = 0; i < m_nMaxModuleID; i ++)
                {
                    if(m_Modules[i] != nullptr)
                    {
                        m_StepModule.push_back(m_Modules[i]);
                    }
                }

                XSF_INFO("进入开服检测...");
                m_nStatus = ServerStatus_RunningCheck;
            }
            break;

        case ServerStatus_RunningCheck:
            {
                if(m_StepModule.size() > 0)
                {
                    auto it = m_StepModule.begin();
                    IModule * pModule = *it;
                    uint8 nRunCode = pModule->OnStartCheck();
                    switch (nRunCode)
                    {
                    case ModuleRunCode_OK:
                        {
                            XSF_INFO("模块启动OK, id=%u, name=%s", pModule->GetModuleID(), pModule->GetModuleName());
                            m_StepModule.erase(it);
                        }
                        break;

                    case ModuleRunCode_Error:
                        m_nStatus = ServerStatus_Release;
                        break;
                    
                    default:
                        break;
                    }
                }
                else
                {
                    XSF_INFO("==== 所有模块都已启动OK ====");
                    for(uint16 i = 0; i < m_nMaxModuleID; i ++)
                    {
                        if(m_Modules[i] != nullptr)
                        {
                            m_Modules[i]->OnOK();
                        }
                    }

                    m_nStatus = ServerStatus_Running;
                    m_LastUpdateTime = XSFCore::TickCount();
                }
            }
            break;

        case ServerStatus_Running:
            {
                auto current = XSFCore::TickCount();
                if(current >= m_LastUpdateTime + 20)
                {
                    auto nDeltaTime = current - m_LastUpdateTime;
                    for(uint16 i = 0; i < m_nMaxModuleID; i ++)
                    {
                        if(m_Modules[i] != nullptr)
                        {
                            m_Modules[i]->OnUpdate(nDeltaTime);
                        }
                    }

                    m_LastUpdateTime = current;
                }
            }
            break;

        case ServerStatus_StopCheck0:
            {
                for(uint16 i = 0; i < m_nMaxModuleID; i ++)
                {
                    if(m_Modules[i] != nullptr)
                    {
                        XSF_INFO("模块开始关闭, id=%u, name=%s", m_Modules[i]->GetModuleID(), m_Modules[i]->GetModuleName());
                        m_Modules[i]->OnStartClose();
                    }
                }

                for(int32 i = m_nMaxModuleID - 1; i >= 0; i --)
                {
                    if(m_Modules[i] != nullptr)
                    {
                        m_StepModule.push_back(m_Modules[i]);
                    }
                }

                XSF_INFO("进入关服检测...");
                m_nStatus = ServerStatus_StopCheck;
            }
            break;

        case ServerStatus_StopCheck:
            {
                if(m_StepModule.size() > 0)
                {
                    auto it = m_StepModule.begin();
                    IModule * pModule = *it;
                    uint8 nRunCode = pModule->OnCloseCheck();
                    if (nRunCode == ModuleRunCode_OK)
                    {
                        XSF_INFO("模块关闭OK, id=%u, name=%s", pModule->GetModuleID(), pModule->GetModuleName());
                        m_StepModule.erase(it);
                    }
                }
                else
                {
                    m_nStatus = ServerStatus_Close;
                }
            }
            break;

        case ServerStatus_Close:
            {
                XSF_INFO("==== 所有模块都已正常关闭，执行最后关闭操作 ====");
                for(uint16 i = 0; i < m_nMaxModuleID; i ++)
                {
                    if(m_Modules[i] != nullptr)
                    {
                        m_Modules[i]->DoClose();
                    }
                }

                m_nStatus = ServerStatus_Release;
            }
            break;

        case ServerStatus_Release:
            {
                XSF_INFO("==== 开始释放所有模块 ====");
                for(uint16 i = 0; i < m_nMaxModuleID; i ++)
                {
                    if(m_Modules[i] != nullptr)
                    {
                        m_Modules[i]->Release();
                    }
                }

                m_IsWorking = false;
            }
            break;
        
        default:
            break;
        }
    }
} // namespace xsf
