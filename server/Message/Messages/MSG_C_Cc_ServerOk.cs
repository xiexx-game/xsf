//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/Message/Messages/MSG_C_Cc_ServerOk.cs
// 作者：Xoen Xie
// 时间：4/11/2024
// 描述：center --> connector 服务器已准备好
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8618
using XsfPbid;
using XsfPb;
using XSF;

namespace XsfMsg
{
    public sealed class MSG_C_Cc_ServerOk : IMessage
    {
        public override ushort ID { get { return (ushort)SMSGID.CCcServerOk; } }

        public override byte DestEP { get { return (byte)EP.Center; } }

        private C_Cc_ServerOk m_PB;
        public C_Cc_ServerOk mPB
        {
            get
            {
                if (m_PB == null)
                    m_PB = new C_Cc_ServerOk();

                return m_PB;
            }
        }

        public override byte[] Export()
        {
            byte[] bytes = new byte[mPB.CalculateSize()];
            mPB.WriteTo(new Google.Protobuf.CodedOutputStream(bytes));

            return bytes;
        }

        public override IMessage Import(byte[] data, int offset, int length)
        {
            var message = new MSG_C_Cc_ServerOk();
            message.m_PB = C_Cc_ServerOk.Parser.ParseFrom(data, offset, length);
            message.m_Executor = m_Executor;
            return message;
        }
    }
}
