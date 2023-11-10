//////////////////////////////////////////////////////////////////////////
//
// 文件：source/xsf/src/net/NetPakcer.h
// 作者：Xoen Xie
// 时间：2020/02/29
// 描述：协议打包器
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _NET_PACKER_H_
#define _NET_PACKER_H_

#include "IXSFMessage.h"
#include "StreamWriter.h"

namespace xsf
{
    class ServerPacker : public INetPacker
    {
    public:
        uint32 GetMinLength(void) override { return 6; }
        uint32 GetMaxLength(void) override { return 0xFFFFFFFF; }

        bool Read(byte * pRecvBuffer, uint32 nRecvLength, DataResult * pResult) override;

        byte * Pack(IMessage * pMessage, uint32 & nLengthOut) override;

    private:
        StreamWriter m_Writer;
    };
}


#endif      // end of _NET_PACKER_H_