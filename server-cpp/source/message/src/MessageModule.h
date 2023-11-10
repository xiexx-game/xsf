//////////////////////////////////////////////////////////////////////////
//
// 文件：message\src\MessageModule.h
// 作者：Xoen Xie
// 时间：2020/05/29
// 描述：消息模块
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _MESSAGE_MODULE_H_
#define _MESSAGE_MODULE_H_

#include "XSFDef.h"
#include "SMessageID.pb.h"
#include "XSF.h"

using namespace xsf;

namespace xsf_msg
{
    class MessageModule : public IModule, public IMessageHelper
    {
    public:
        bool Init(ModuleInit *pInit) override;

        void Release(void) override;

    public:
        IMessage * GetMessage(uint16 nID) override;

    private:
        IMessage *m_Messages[xsf_pbid::SMSGID::SMSGID_Max] = {nullptr};
    };

}

#endif // end of _MESSAGE_MODULE_H_