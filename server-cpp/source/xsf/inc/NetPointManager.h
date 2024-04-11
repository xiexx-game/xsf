//////////////////////////////////////////////////////////////////////////
//
// 文件：source/xsf/inc/NetPointManager.h
// 作者：Xoen Xie
// 时间：2023/08/14
// 描述：网点管理器
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _NET_POINT_MANAGER_H_
#define _NET_POINT_MANAGER_H_

#include "XSFDef.h"
#include "IXSFNet.h"
#include "XSFMallocHook.h"
#include "IXSFServer.h"

namespace xsf
{
    struct INetPacker;

    struct NetPointManagerInit : public ModuleInit
    {
        uint16 nPort = 0;
        INetPacker * packer = nullptr;
    };

    class NetPoint;

    class INetPointManager : public INetHandler, public IModule
    {
        using NetPointList = list<NetPoint*>;
    public:
        INetPointManager(void) 
        {
            //m_ID.ID = 0;
        }

        virtual ~INetPointManager(void) {}

    public:
        virtual bool Init( ModuleInit * pInit ) override;

        virtual bool Start() override;

        virtual void Release(void) override;

        virtual uint8 OnCloseCheck() override 
        {
            if(GetTotal() > 0)
            {
                return ModuleRunCode_Wait;
            }

            return ModuleRunCode_OK;
        }

    public:
        virtual bool Add( NetPoint * pPoint ) = 0;

        virtual void Delete( NetPoint * pPoint ) = 0;

        virtual uint32 GetTotal(void) = 0;

        virtual NetPoint * GetNetPoint(uint32 nID) = 0;

        virtual bool CanConnected(void) { return true; }

        virtual void Broadcast(IMessage * pMessage, uint32 nSkipID) = 0;

    protected:
        bool DeleteFromWaitList(NetPoint * pPoint);

        virtual NetPoint * NewNP(void);

        virtual uint16 GetListenPort(void) { return m_nListenPort; }

        virtual void OnNetPointLost( NetPoint * pPoint ) { }

        virtual void OnNetPointConnected( NetPoint * pPoint ) { }

    public:
        // 接受到连接
        INetHandler * OnAccept(IConnection * pIncomingConn) override;

        // 已连接到远程主机
        void OnConnected( IConnection * pConection ) override;

        // 连接出错或断开
        void OnError( IConnection * pConection, uint32 nErrorCode ) override;

        // 收到数据
        void OnRecv(IConnection * pConection, DataResult * pResult) override;

    private:
        IConnection * m_pConnection = nullptr;
        uint16 m_nListenPort = 0;

        NetPointList m_WaitList;

        INetPacker * m_Packer = nullptr;
    };


    //////////////////////////////////////////////////////////////////////////
    // 一个使用map管理和查询网点的管理器
    class NormalNetPointManager : public INetPointManager
    {
        using NetPointMap = unordered_map<uint32, NetPoint*>;

    public:
        virtual bool Add( NetPoint * pPoint ) override;

        virtual void Delete( NetPoint * pPoint ) override;

        uint32 GetTotal(void) override { return m_NetPoints.size(); }

        NetPoint * GetNetPoint(uint32 nID) override
        {
            auto it = m_NetPoints.find(nID);
            if( m_NetPoints.end() != it )
            {
                return it->second;
            }

            return nullptr;
        }

        void Broadcast(IMessage * pMessage, uint32 nSkipID) override;

    protected:
        NetPointMap m_NetPoints;
    };


    //////////////////////////////////////////////////////////////////////////
    // 一个使用数组管理和查询网点的管理器
    struct FastNetPointManagerInit : public NetPointManagerInit
    {
        uint32 nMaxSize;
    };

    class FastNetPointManager : public INetPointManager
    {
    public:
        virtual bool Init( ModuleInit * pInit ) override;

        virtual void Release(void) override;

        virtual bool Add( NetPoint * pPoint ) override;

        virtual void Delete( NetPoint * pPoint ) override;

        uint32 GetTotal(void) override { return m_nCount; }

        NetPoint * GetNetPoint(uint32 nID) override
        {
            if( nID >= m_nSize )
                return nullptr;

            return m_pNetPoints[nID];
        }

        void Broadcast(IMessage * pMessage, uint32 nSkipID) override;

    protected:
        uint32 m_nSize = 0;
        uint32 m_nCount = 0;
        NetPoint ** m_pNetPoints = nullptr;
    };
}



#endif      // end of _NET_POINT_MANAGER_H_