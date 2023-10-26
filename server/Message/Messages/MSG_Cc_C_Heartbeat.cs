//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/Message/Messages/MSG_Cc_C_Heartbeat.cs
// 作者：Xoen Xie
// 时间：10/26/2023
// 描述：center connector --> center 心跳
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8618

using XsfPb;
using XSF;

namespace XsfMsg
{
    public sealed class MSG_Cc_C_Heartbeat : IMessage
    {
        public override ushort ID { get { return (ushort)SMSGID.CcCHeartbeat; } }

        public override byte DestEP { get { return (byte)EP.Center; } }

        private Cc_C_Heartbeat m_PB;
        public Cc_C_Heartbeat mPB
        {
            get
            {
                if (m_PB == null)
                    m_PB = new Cc_C_Heartbeat();

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
            var message = new MSG_Cc_C_Heartbeat();
            message.m_PB = Cc_C_Heartbeat.Parser.ParseFrom(data, offset, length);
            message.m_Executor = m_Executor;
            return message;
        }
    }
}
