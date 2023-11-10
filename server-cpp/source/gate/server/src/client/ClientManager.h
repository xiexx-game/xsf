//////////////////////////////////////////////////////////////////////////
//
// 文件：source/gate/server/src/client/ClientManager.h
// 作者：Xoen Xie
// 时间：2020/08/05
// 描述：client 管理
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _CLIENT_MANAGER_H_
#define _CLIENT_MANAGER_H_

#include "XSF.h"
#include "Singleton.h"
using namespace xsf;

class Client;
class LocalPacker;

class ClientManager : public IModule, public INetHandler, public Singleton<ClientManager>
{
public:
    ClientManager(void) {}
    ~ClientManager(void) {}

    Client * New(void);

    void Delete( Client * pClient );

    Client * GetClient( uint32 nClientID );

    void Broadcast( const byte * pData, uint32 nDataLen );

public:
    bool Init( ModuleInit * pInit ) override;

    void Release(void) override;

    void DoRegist() override;

    bool Start() override;

    void OnClose(void) override;

    uint8 OnCloseCheck() override;

public:
    // 接受到连接
    INetHandler * OnAccept(IConnection * pIncomingConn) override;

    // 已连接到远程主机
    void OnConnected( IConnection * pConection ) override;

    // 连接出错或断开
    void OnError( IConnection * pConection, uint32 nErrorCode ) override;

    // 收到数据
    virtual void OnRecv(IConnection * pConection, DataResult * pResult) override;

private:
    Client ** m_Clients = nullptr;
    uint32 m_nSize = 0;
    uint32 m_nTotal = 0;

    uint32 m_nPort = 0;     // 监听端口
    uint8 m_nGlobalKey = 0;

    IConnection * m_pConnection = nullptr;

    LocalPacker * m_pPakcer = nullptr;
};

void SetClientManager(void);




#endif      // end of _CLIENT_MANAGER_H_