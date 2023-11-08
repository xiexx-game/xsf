//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/Message/Messages/MSG_GtA_Gt_ClientMessage.cs
// 作者：Xoen Xie
// 时间：10/26/2023
// 描述：gate acceptor --> gate 发往客户端消息
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8618
using XsfPbid;
using XsfPb;
using XSF;

namespace XsfMsg
{
    public sealed class MSG_GtA_Gt_ClientMessage : IMessage
    {
        public override ushort ID { get { return (ushort)SMSGID.GtAGtClientMessage; } }

        public override byte DestEP { get { return (byte)EP.Gate; } }

        private GtA_Gt_ClientMessage m_PB;
        public GtA_Gt_ClientMessage mPB
        {
            get
            {
                if (m_PB == null)
                    m_PB = new GtA_Gt_ClientMessage();

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
            var message = new MSG_GtA_Gt_ClientMessage();
            message.m_PB = GtA_Gt_ClientMessage.Parser.ParseFrom(data, offset, length);
            message.m_Executor = m_Executor;
            return message;
        }
    }
}
