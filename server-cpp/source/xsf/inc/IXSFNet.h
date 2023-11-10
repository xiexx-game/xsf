//////////////////////////////////////////////////////////////////////////
//
// 文件：base\inc\IXSFNet.h
// 作者：Xoen Xie
// 时间：2023/08/14
// 描述：网络相关定义
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _I_XSF_NET_H_
#define _I_XSF_NET_H_

#include "XSFDef.h"
#include "IXSFMessage.h"

namespace xsf
{
    enum EMNetCode
    {
        NetCode_None = 0,

        NetCode_Close = 1000,     // 本地连接断开
        NetCode_Disconnect = 1001,

        NetCode_ErrorRemote = 2000,		    // 获取远程信息错误，一般发生在getaddrinfo时
        NetCode_ErrorConnect = 2001,				// 连接失败
        NetCode_ErrorSocketAdd= 2002,			    // 套接字添加到epoll中失败
        NetCode_ErrorRecv = 2003,				    // 接收数据时错误
        NetCode_ErrorHeadRecv = 2004,			    // 接收数据时错误
        NetCode_ErrorSend = 2005,                  // 发送数据错误
        NetCode_ErrorAccept = 2006,                // 接收连接时错误
    };

    struct IConnection;
    struct DataResult;
    struct IMessage;

    struct INetHandler
    {
        // 接受到连接
        virtual INetHandler * OnAccept( IConnection * pIncomingConn ) = 0;

        // 已连接到远程主机
        virtual void OnConnected( IConnection * pConection ) = 0;

        // 连接出错或断开
        virtual void OnError( IConnection * pConection, uint32 nErrorCode ) = 0;

        // 收到数据
        virtual void OnRecv( IConnection * pConection, DataResult * pResult) = 0;
    };

    struct IConnection
    {
        virtual ~IConnection(void) {}
        
        // 监听一个端口
        virtual bool Listen(uint16 nPort) = 0;

        // 连接一个地址
        virtual bool Connect( const char * sIP, uint16 nPort) = 0;

        // 发送消息
        virtual bool SendMessage(IMessage * pMessage) = 0;

        // 发送数据
        virtual bool Send( const byte * pData, uint32 nDataLen ) = 0;
        
        // 设置事件句柄
        virtual void SetNetHandler( INetHandler * pHandler ) = 0;

        // 获取事件句柄
        virtual INetHandler * GetNetHandler(void) const = 0;

        // 获取对端IP
        virtual const char * GetRemoteIP(void) = 0;

        // 获取对端端口
        virtual uint32 GetRemotePort(void) = 0;

        // 关闭连接
        virtual void Close(void) = 0;

        virtual int32 GetSocketID(void) = 0;
    };


    #define CONNECTION_CLOSE(_P)            \
    if( _P != nullptr )                     \
    {                                       \
        _P->Close();                        \
        _P = nullptr;                       \
    }                                       \


}


#endif      // end of _I_XSF_NET_H_