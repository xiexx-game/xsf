//////////////////////////////////////////////////////////////////////////
//
// 文件：message\inc\DMessageDef.h
// 作者：Xoen Xie
// 时间：2020/05/29
// 描述：消息相关定义
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _D_MESSAGE_DEF_H_
#define _D_MESSAGE_DEF_H_

namespace xsf_msg
{
    #define CONCAT(a, b) a##b

    #define MESSAGE(_NAME, _EP, _ID)														    \
    class MSG_##_NAME : public IMessage												            \
    {																							\
    public:																						\
        MSG_##_NAME(void) {}																	\
        ~MSG_##_NAME(void) {}																    \
        uint16 GetID(void) override { return _ID; }						                        \
        byte GetEP(void) override { return _EP; }                                               \
        uint32 GetLength(void) override;                                                        \
        void Import(const byte * pData, uint32 nLength) override;								\
        void Export(StreamWriter * pWriter, uint32 nLength) override;							\
    public:																						\
        _NAME mPB;															                    \
    };																							\
                                                                                                \


	#define MESSAGE_FUNCTIONS(_NAME)													        \
    void MSG_##_NAME::Import(const byte * pData, uint32 nLength)								\
    {																							\
        mPB.ParseFromArray(pData, nLength);								                        \
    }																							\
                                                                                                \
    uint32 MSG_##_NAME::GetLength(void)                                                         \
    {                                                                                           \
        return mPB.ByteSizeLong();                                                              \
    }                                                                                           \
                                                                                                \
    void MSG_##_NAME::Export(StreamWriter * pWriter, uint32 nLength)							\
    {																							\
        byte * pBuffer = pWriter->BeginWriteBuffer(nLength);							        \
        mPB.SerializeToArray(pBuffer, nLength);												    \
        pWriter->EndWriteBuffer();																\
    }																							\
                                                                                                \

	
	

    #define MESSAGE_EXECUTOR(_NAME)														\
    class Executor_##_NAME : public xsf::IMessageExecutor							    \
    {																					\
    public:																				\
        void Execute(void * pNetObj, IMessage * pMessage, DataResult * pResult) override;	\
    };																					\
                                                                                        \


    #define MESSAGE_EXECUTOR_EXECUTE(_NAME)     void Executor_##_NAME::Execute(void * pNetObj, IMessage * pMessage, DataResult * pResult)


    #define SET_MESSAGE_EXECUTOR(_MSG_MODULE, _ID, _MESSAGE)     XSFCore::SetMessageExecutor( _ID, new Executor_##_MESSAGE())
    #define GET_MESSAGE(_DEST, _ID, _MESSAGE)			    xsf_msg::MSG_##_MESSAGE * _DEST = static_cast<xsf_msg::MSG_##_MESSAGE*>(XSFCore::GetMessage(xsf_pb::##_ID))

}


#endif      // end of _D_MESSAGE_DEF_H_