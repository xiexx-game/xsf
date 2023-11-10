//////////////////////////////////////////////////////////////////////////
//
// 文件：source/gate/acceptor/src/Gate.cpp
// 作者：Xoen Xie
// 时间：2020/02/29
// 描述：网关对象
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#include "Gate.h"
#include "DMessageInc.h"

namespace GateA
{
    void Gate::Broadcast(const byte * pData, uint32 nDataLength)
    {
        if(m_ClientIDs.size() > 0)
        {
            XSF_CAST(pMessage, XSFCore::GetMessage(xsf_pbid::GtA_Gt_ClientMessage), xsf_msg::MSG_GtA_Gt_ClientMessage);
            pMessage->mPB.clear_client_id();
            for(uint32 i = 0; i < m_ClientIDs.size(); i++)
            {
                pMessage->mPB.add_client_id(m_ClientIDs[i]);
            }

            pMessage->mPB.set_client_message(pData, nDataLength);

            SendMessage(pMessage);
        }
    }
}