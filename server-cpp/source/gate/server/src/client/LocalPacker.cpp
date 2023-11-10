//////////////////////////////////////////////////////////////////////////
//
// 文件：source/gate/server/src/client/LocalPacker.cpp
// 作者：Xoen Xie
// 时间：2020/02/29
// 描述：消息打包器
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#include "LocalPacker.h"
#include "StreamReader.h"

bool LocalPacker::Read(byte * pRecvBuffer, uint32 nRecvLength, DataResult * pResult)
{
    StreamReader reader = {"LocalPacker::Read"};
    reader.Init(pRecvBuffer+sizeof(uint32), nRecvLength-sizeof(uint32));
    reader.ReadUint16(pResult->nMessageID);
    reader.ReadUint32(pResult->nRawID);

    XSF_INFO("LocalPacker::Read pResult->nMessageID=%u, pResult->nRawID=%u", pResult->nMessageID, pResult->nRawID);

    pResult->pRawData = pRecvBuffer;
    pResult->nRawLength = nRecvLength;

    pResult->pPBData = reader.Buffer();
    pResult->nPBLength = reader.Size();

    return true;
}