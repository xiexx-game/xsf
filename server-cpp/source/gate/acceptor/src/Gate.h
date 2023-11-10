//////////////////////////////////////////////////////////////////////////
//
// 文件：source/gate/acceptor/src/Gate.h
// 作者：Xoen Xie
// 时间：2020/02/29
// 描述：网关对象
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _GATE_H_
#define _GATE_H_

#include "NetPoint.h"
using namespace xsf;

namespace GateA
{
    class Gate : public NetPoint
    {
    public:
        JEMALLOC_HOOK

        void Add(uint32 nClientID)
        {
            m_ClientIDs.push_back(nClientID);
        }

        void Clear(void)
        {
            m_ClientIDs.clear();
        }

        void Broadcast(const byte * pData, uint32 nDataLength);

    private:
        vector<uint32> m_ClientIDs;
    };
}

#endif // end of _GATE_H_