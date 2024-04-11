//////////////////////////////////////////////////////////////////////////
//
// 文件：source/xsf/inc/XSFEpDef.h
// 作者：Xoen Xie
// 时间：2023/08/14
// 描述：消息EP相关定义
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _XSF_EP_DEF_H_
#define _XSF_EP_DEF_H_

#include "XSFDef.h"
#include "XSFMallocHook.h"
#include "tinyxml2.h"

namespace xsf
{
    // 端点类型，也可以用作服务器类型
    enum EMEndPoint
    {
        EP_None = 0,
        EP_Client,
        EP_Center,      // 中心服务器
        EP_Login,        // 登录服务器
        EP_Gate,        // 网关服务器
        EP_Game,          // 游戏服务器
        EP_DB,
        EP_Hub,

        EP_Max,         // 最大只能为30
    };

    inline uint8 Name2EP( const char * name)
    {
        if( strcasecmp(name, "center") == 0 )   return EP_Center;
        else if(strcasecmp(name, "gate") == 0)  return EP_Gate;
        else if(strcasecmp(name, "login") == 0)  return EP_Login;
        else if(strcasecmp(name, "game") == 0)  return EP_Game;
        else if(strcasecmp(name, "db") == 0)  return EP_DB;
        else if(strcasecmp(name, "hub") == 0)  return EP_Hub;
        else return EP_None;
    }

    inline const char * EP2Name( uint8 nEP )
    {
        switch (nEP)
        {
        case EP_Client: return "client";
        case EP_Center: return "center";
        case EP_Login:   return "login";
        case EP_Gate:   return "gate";
        case EP_Game:   return "game";
        case EP_DB:   return "db";
        case EP_Hub:   return "hub";

        default:           return "unkown";
        }
    }

    inline const char * EP2CNName( uint8 nEP )
    {
        switch (nEP)
        {
        case EP_Client: return "客户端";
        case EP_Center: return "中心服";
        case EP_Login:   return "登录服";
        case EP_Gate:   return "网关服";
        case EP_Game:   return "游戏服";
        case EP_DB:   return "数据服";
        case EP_Hub:   return "中转服";

        default:           return "未知";
        }
    }

    enum EMNodeStatus
    {
        NodeStatus_None = 0,
        NodeStatus_New,
        NodeStatus_Ok,
    };

    
    //////////////////////////////////////////////////////////////////////////
    //
    //  服务器节点配置
    //
    //////////////////////////////////////////////////////////////////////////
    struct ServerInfo
    {
        JEMALLOC_HOOK
        
        uint32 ID;
        char sIP[MAX_IP_SIZE] = {0};
        uint8 Status;
        uint32 Ports[EP_Max] = {0};
    };

    struct ServerNode
    {
        uint8 nEP;
        char sName[BUFFER_SIZE_32] = {0};

        bool Init(tinyxml2::XMLElement * pItemEle);
    };

    struct XSFConfig
    {
        char sName[BUFFER_SIZE_64] = {0};
        char sDesc[BUFFER_SIZE_64] = {0};

        bool AutoStart = false;
        uint32 HeartbeatCheck = 0;
        uint32 HeartbeatTimeout = 0;
        uint32 HeartbeatInterval = 0;
        uint32 ReconnectInterval = 0;

        uint32 ClientHeartbeatCheck = 0;
        uint32 ClientHeartbeatTimeout = 0;

        char MainCenterIP[MAX_IP_SIZE] = {0};
        uint32 CenterPort = 0;
        uint32 InnerPortStart = 0;
        uint32 OutPortStart = 0;
        uint32 GateMaxCount = 0;
        char Nats[MAX_IP_SIZE*2] = {0};
        uint32 AccountLifeTime = 0;
        uint32 ActorSaveTime = 0;
        uint32 ClientMaxMsgLength = 0;

        uint8 MaxGate = 0;
        uint32 ActorReleaseTime = 0;

        ServerNode Me;

        vector<ServerNode> NodeList;
    };


    //////////////////////////////////////////////////////////////////////////
    //
    //  服务器执行步骤
    //
    //////////////////////////////////////////////////////////////////////////
    enum EMModuleRunCode
    {
        ModuleRunCode_OK = 0,
        ModuleRunCode_Error,
        ModuleRunCode_Wait,
    };
    
} // namespace xsf


#endif      // end of _XSF_EP_DEF_H_
