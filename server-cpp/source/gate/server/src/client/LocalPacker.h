//////////////////////////////////////////////////////////////////////////
//
// 文件：source/gate/server/src/client/LocalPacker.h
// 作者：Xoen Xie
// 时间：2020/02/29
// 描述：消息打包器
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _LOCAL_PACKER_H_
#define _LOCAL_PACKER_H_

#include "XSF.h"
using namespace xsf;

class LocalPacker : public ClientPacker
{
public:
    uint32 GetMinLength(void) override { return 6; }
    uint32 GetMaxLength(void) override { return XSFCore::GetServer()->GetConfig()->ClientMaxMsgLength; }

    bool Read(byte * pRecvBuffer, uint32 nRecvLength, DataResult * pResult) override;

private:
    StreamWriter m_Writer;
};





#endif      // end of _LOCAL_PACKER_H_