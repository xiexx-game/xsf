//////////////////////////////////////////////////////////////////////////
//
// 文件：source/xsf/src/net/NetPacker.cpp
// 作者：Xoen Xie
// 时间：2020/02/29
// 描述：协议打包器
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#include "NetPakcer.h"
#include "StreamReader.h"

namespace xsf
{
    bool ServerPacker::Read(byte * pRecvBuffer, uint32 nRecvLength, DataResult * pResult)
    {
        StreamReader reader = {"ServerPacker::Read"};
        reader.Init(pRecvBuffer+sizeof(uint32), nRecvLength-sizeof(uint32));
        reader.ReadUint16(pResult->nMessageID);
        reader.ReadUint32(pResult->nRawID);

        pResult->pPBData = reader.Buffer();
        pResult->nPBLength = reader.Size();

        //XSF_INFO("ServerPacker::Read pResult->nMessageID=%u, pResult->nRawID=%u, total length=%u, pb length=%u", pResult->nMessageID, pResult->nRawID, nRecvLength, pResult->nPBLength);

        return true;
    }

    byte * ServerPacker::Pack(IMessage * pMessage, uint32 & nLengthOut)
    {
        m_Writer.Clear();

        uint32 nPbLength = pMessage->GetLength();

        // | 包长(uint 4字节) | 消息ID(ushort 2字节) | rawID(uint 4字节) | pb data |
        uint32 total = sizeof(uint16) + sizeof(uint32) + nPbLength;

        //XSF_INFO("ServerPacker::Pack  id=%u, nPbLength=%u, total=%u", pMessage->GetID(), nPbLength, total + 4);

        m_Writer.WriteUint32(total);
        m_Writer.WriteUint16(pMessage->GetID());
        m_Writer.WriteUint32(pMessage->GetID());
        pMessage->Export(&m_Writer, nPbLength);

        nLengthOut = m_Writer.Size();
        return m_Writer.Buffer();
    }


    byte * ClientPacker::Pack(IMessage * pMessage, uint32 & nLengthOut)
    {
        m_Writer.Clear();

        uint32 nPbLength = pMessage->GetLength();

        // | 包长(uint 4字节) | 消息ID(ushort 2字节) | pb data |
        uint total = sizeof(uint16) + nPbLength;
        m_Writer.WriteUint32(total);
        m_Writer.WriteUint16(pMessage->GetID());
        pMessage->Export(&m_Writer, nPbLength);

        nLengthOut = m_Writer.Size();
        return m_Writer.Buffer();
    }
}