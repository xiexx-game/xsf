//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/Message/Messages/MSG_Gt_GtA_Handshake.cs
// 作者：Xoen Xie
// 时间：10/26/2023
// 描述：gate --> gate acceptor 握手
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8618
using XsfPbid;
using XsfPb;
using XSF;

namespace XsfMsg
{
    public sealed class MSG_Gt_GtA_Handshake : IMessage
    {
        public override ushort ID { get { return (ushort)SMSGID.GtGtAHandshake; } }

        public override byte DestEP { get { return (byte)EP.Gate; } }

        private Gt_GtA_Handshake m_PB;
        public Gt_GtA_Handshake mPB
        {
            get
            {
                if (m_PB == null)
                    m_PB = new Gt_GtA_Handshake();

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
            var message = new MSG_Gt_GtA_Handshake();
            message.m_PB = Gt_GtA_Handshake.Parser.ParseFrom(data, offset, length);
            message.m_Executor = m_Executor;
            return message;
        }
    }
}
