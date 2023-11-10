//////////////////////////////////////////////////////////////////////////
//
// 文件：source/xsf/inc/NetPoint.h
// 作者：Xoen Xie
// 时间：2023/08/14
// 描述：一个网络节点对象
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _NET_POINT_H_
#define _NET_POINT_H_

#include "XSFDef.h"
#include "IXSFNet.h"
#include "XSFMallocHook.h"
#include "StreamReader.h"
#include "StreamWriter.h"
#include "IXSFTimer.h"
#include "IXSFMessage.h"

namespace xsf
{
    class INetPointManager;

    class NetPoint : public INetHandler, public ITimerHandler
    {
        friend class INetPointManager;
        friend class NormalNetPointManager;
        friend class FastNetPointManager;

        TIMER_ID_START
            TimerID_HTCheck,
        TIMER_ID_END

    public:
        NetPoint(void) 
            : m_Timers(TimerID_Max)
        {
            m_ID.ID = 0;
        }

        virtual ~NetPoint(void) {}

        JEMALLOC_HOOK;

        bool Create( INetPointManager * pOwner, IConnection * pConnection );

        void Close(void);

        const SID * GetSID(void) { return &m_ID; }
        void SetID( uint32 nID ) { m_ID.ID = nID; }

        INetPointManager * GetOwner(void) { return m_pOwner; }

        IConnection * GetConnection(void) { return m_pConnection; }

        void SendMessage( IMessage * pMessage );

        void SendData( const byte * pData, uint32 nDataLen );

        void UpdateHTTime(void);

        const char * GetRemoteIP(void);

    public:
        // 接受到连接
        INetHandler * OnAccept(IConnection * pIncomingConn) override;

        // 已连接到远程主机
        void OnConnected( IConnection * pConection ) override;

        // 连接出错或断开
        void OnError( IConnection * pConection, uint32 nErrorCode ) override;

        // 收到数据
        virtual void OnRecv(IConnection * pConection, DataResult * pResult) override;

    public:
        TIMER_FUNCTION_DEFINE;

    private:
        void OnTimerHTCheck(bool bLastCall);

    protected:
        SID m_ID;                   // 该网点的服务器ID
        
        IConnection * m_pConnection = nullptr;

        INetPointManager * m_pOwner = nullptr;

        uint32 m_nLastHTTime = 0;      // 上一次心跳的时间

        TimerManager m_Timers;
    };
}





#endif      // end of _NET_POINT_H_