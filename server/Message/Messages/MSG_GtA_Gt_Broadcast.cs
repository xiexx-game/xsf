//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/Message/Messages/MSG_GtA_Gt_Broadcast.cs
// 作者：Xoen Xie
// 时间：4/11/2024
// 描述：gate acceptor --> gate 广播消息
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8618
using XsfPbid;
using XsfPb;
using XSF;

namespace XsfMsg
{
    public sealed class MSG_GtA_Gt_Broadcast : IMessage
    {
        public override ushort ID { get { return (ushort)SMSGID.GtAGtBroadcast; } }

        public override byte DestEP { get { return (byte)EP.Gate; } }

        private GtA_Gt_Broadcast m_PB;
        public GtA_Gt_Broadcast mPB
        {
            get
            {
                if (m_PB == null)
                    m_PB = new GtA_Gt_Broadcast();

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
            var message = new MSG_GtA_Gt_Broadcast();
            message.m_PB = GtA_Gt_Broadcast.Parser.ParseFrom(data, offset, length);
            message.m_Executor = m_Executor;
            return message;
        }
    }
}
