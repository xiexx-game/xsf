//////////////////////////////////////////////////////////////////////////
//
// 文件：source/center/server/src/NodeManager.h
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：节点管理
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _NODE_MANAGER_H_
#define _NODE_MANAGER_H_

#include "NetPointManager.h"
using namespace xsf;
#include "Singleton.h"

class NodeManager : public NormalNetPointManager, public Singleton<NodeManager>
{
    enum RunStep
    {
        RunStep_None = 0,
        RunStep_StartServer,
        RunStep_WaitHandshake,
        RunStep_HandshakeDone,
        RunStep_OK,
    };

    typedef list<ServerInfo*> InfoList;
    typedef unordered_map<uint32, ServerInfo*> InfoMap;
public:
    bool Init(ModuleInit * pInit) override;

    bool Start(void) override;

    void OnClose(void) override;
    
    void DoRegist(void) override;

    uint8 OnStartCheck() override;

public:
    void OnNetPointLost( NetPoint * pPoint ) override;

    void OnNetPointConnected( NetPoint * pPoint ) override;

public:
    ServerInfo * AddNode(uint32 nID, const char * ip, vector<uint> & ports);

    void OnNodeOK(uint32 nID);

private:
    ServerInfo * GetLostNode(byte nEP, uint32 nID, bool CheckEqual);

    byte GetIndex(byte nEP)
    {
        byte nIndex = m_NodeIndex[nEP];
        m_NodeIndex[nEP] ++;

        return nIndex;
    } 

    void GetPort(byte nEP, ServerInfo * pInfo);

    uint32 GetNextPort(bool IsInner)
    {
        if(IsInner)
        {
            return m_nInnerPort ++;
        } 
        else 
        {
            return m_nOutPort ++;
        }
    }

private:
    InfoList m_LostList[EP_Max];
    InfoMap m_Nodes;
    byte m_NodeIndex[EP_Max];
    uint32 m_nInnerPort = 0;
    uint32 m_nOutPort = 0;

    uint32 m_nStartIndex = 0;
    byte m_nStep = 0;
    byte m_nCurStartEP = 0;
};

void SetNodeManager(void);


#endif      // end of _NODE_MANAGER_H_