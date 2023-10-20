//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/Message/Messages/MSG_Clt_Gt_Handshake.cs
// 作者：Xoen Xie
// 时间：10/19/2023
// 描述：client --> gate 握手请求
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8618

using XsfPb;
using XSF;

namespace XsfMsg
{
    public sealed class MSG_Clt_Gt_Handshake : IMessage
    {
        public override ushort ID { get { return (ushort)CMSGID.CltGtHandshake; } }

        private Clt_Gt_Handshake m_PB;
        public Clt_Gt_Handshake mPB
        {
            get
            {
                if (m_PB == null)
                    m_PB = new Clt_Gt_Handshake();

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
            var message = new MSG_Clt_Gt_Handshake();
            message.m_PB = Clt_Gt_Handshake.Parser.ParseFrom(data, offset, length);
            message.m_Executor = m_Executor;
            return message;
        }
    }
}
