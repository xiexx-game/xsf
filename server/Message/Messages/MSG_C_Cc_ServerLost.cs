//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/Message/Messages/MSG_C_Cc_ServerLost.cs
// 作者：Xoen Xie
// 时间：2024/4/10
// 描述：center --> connector 服务器离线
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8618
using XsfPbid;
using XsfPb;
using XSF;

namespace XsfMsg
{
    public sealed class MSG_C_Cc_ServerLost : IMessage
    {
        public override ushort ID { get { return (ushort)SMSGID.CCcServerLost; } }

        public override byte DestEP { get { return (byte)EP.Center; } }

        private C_Cc_ServerLost m_PB;
        public C_Cc_ServerLost mPB
        {
            get
            {
                if (m_PB == null)
                    m_PB = new C_Cc_ServerLost();

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
            var message = new MSG_C_Cc_ServerLost();
            message.m_PB = C_Cc_ServerLost.Parser.ParseFrom(data, offset, length);
            message.m_Executor = m_Executor;
            return message;
        }
    }
}
