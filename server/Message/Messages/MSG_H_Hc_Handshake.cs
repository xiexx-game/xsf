//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/Message/Messages/MSG_H_Hc_Handshake.cs
// 作者：Xoen Xie
// 时间：4/11/2024
// 描述：hub --> hub connector 握手反馈
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8618
using XsfPbid;
using XsfPb;
using XSF;

namespace XsfMsg
{
    public sealed class MSG_H_Hc_Handshake : IMessage
    {
        public override ushort ID { get { return (ushort)SMSGID.HHcHandshake; } }

        public override byte DestEP { get { return (byte)EP.Hub; } }

        private H_Hc_Handshake m_PB;
        public H_Hc_Handshake mPB
        {
            get
            {
                if (m_PB == null)
                    m_PB = new H_Hc_Handshake();

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
            var message = new MSG_H_Hc_Handshake();
            message.m_PB = H_Hc_Handshake.Parser.ParseFrom(data, offset, length);
            message.m_Executor = m_Executor;
            return message;
        }
    }
}
