//////////////////////////////////////////////////////////////////////////
//
// 文件：source/xsf/inc/IXSFMessage.h
// 作者：Xoen Xie
// 时间：2023/08/14
// 描述：消息相关定义
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _I_XSF_MESSAGE_H_
#define _I_XSF_MESSAGE_H_

#include "XSFDef.h"
#include "XSFLog.h"
#include "StreamWriter.h"

namespace xsf
{
    class IMessage;
    class StreamReader;

    struct DataResult
    {
        uint16 nMessageID = 0;
        uint32 nRawID = 0;

        byte * pPBData = nullptr;
        uint32 nPBLength = 0;

        byte * pRawData = nullptr;
        uint32 nRawLength = 0;
    };


    // 消息具体的逻辑执行者
    struct IMessageExecutor
    {
        virtual ~IMessageExecutor(void) {}

        virtual void Execute(void * pNetObj, IMessage * pMessage, DataResult * pResult) = 0;
    };


    class IMessage
    {
    public:
        IMessage(void) {}
        virtual ~IMessage(void) {}

        virtual uint16 GetID(void) = 0;

        virtual byte GetEP(void) = 0;

        virtual void Import(const byte * pData, uint32 nLength) = 0;

        virtual uint32 GetLength(void) = 0;

        virtual void Export(StreamWriter * pWriter, uint32 nLength) = 0;

        void Release(void)
        {
            XSF_DELETE(m_pExecutor);

            delete this;
        }

        void Execute(void * pNetObj, IMessage * pMessage, DataResult * pResult)
        {
            if (m_pExecutor != nullptr)
                m_pExecutor->Execute(pNetObj, this, pResult);
            else
                XSF_WARN("IMessage::Execute m_pExecutor is not null, id=%u", GetID());
        }

        void SetExecutor( IMessageExecutor * pExecutor ) 
        {
            if( m_pExecutor == nullptr ) 
                m_pExecutor = pExecutor; 
            else
                XSF_WARN("IMessage::SetExecutor m_pExecutor is not null, id=%u", GetID());
        }

    private:
	    IMessageExecutor * m_pExecutor = nullptr;
    };


    struct IMessageHelper
    {
        virtual IMessage * GetMessage(uint16 nID) = 0;
    };

    struct INetPacker
    {
        virtual ~INetPacker(void) {}

        virtual uint32 GetMinLength(void) = 0;
        virtual uint32 GetMaxLength(void) = 0;

        virtual bool Read(byte * pRecvBuffer, uint32 nRecvLength, DataResult * pResult) = 0;

        virtual byte * Pack(IMessage * pMessage, uint32 & nLengthOut) = 0;
    };

    class ClientPacker : public INetPacker
    {
    public:
        virtual ~ClientPacker(void) {}

        virtual uint32 GetMinLength(void) override { return 0; }
        virtual uint32 GetMaxLength(void) override { return 0; }

        virtual bool Read(byte * pRecvBuffer, uint32 nRecvLength, DataResult * pResult) override { return true; }

        virtual byte * Pack(IMessage * pMessage, uint32 & nLengthOut) override;

    private:
        StreamWriter m_Writer;
    };


} // namespace xsf






#endif      // end of _I_XSF_MESSAGE_H_
