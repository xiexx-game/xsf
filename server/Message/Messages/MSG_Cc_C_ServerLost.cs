//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/Message/Messages/MSG_Cc_C_ServerLost.cs
// 作者：Xoen Xie
// 时间：4/11/2024
// 描述：connector --> center 服务器离线
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8618
using XsfPbid;
using XsfPb;
using XSF;

namespace XsfMsg
{
    public sealed class MSG_Cc_C_ServerLost : IMessage
    {
        public override ushort ID { get { return (ushort)SMSGID.CcCServerLost; } }

        public override byte DestEP { get { return (byte)EP.Center; } }

        private Cc_C_ServerLost m_PB;
        public Cc_C_ServerLost mPB
        {
            get
            {
                if (m_PB == null)
                    m_PB = new Cc_C_ServerLost();

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
            var message = new MSG_Cc_C_ServerLost();
            message.m_PB = Cc_C_ServerLost.Parser.ParseFrom(data, offset, length);
            message.m_Executor = m_Executor;
            return message;
        }
    }
}
