//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/Message/Messages/MSG_Gt_GtA_Heartbeat.cs
// 作者：Xoen Xie
// 时间：10/26/2023
// 描述：gate --> gate acceptor 心跳
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8618
using XsfPbid;
using XsfPb;
using XSF;

namespace XsfMsg
{
    public sealed class MSG_Gt_GtA_Heartbeat : IMessage
    {
        public override ushort ID { get { return (ushort)SMSGID.GtGtAHeartbeat; } }

        public override byte DestEP { get { return (byte)EP.Gate; } }

        private Gt_GtA_Heartbeat m_PB;
        public Gt_GtA_Heartbeat mPB
        {
            get
            {
                if (m_PB == null)
                    m_PB = new Gt_GtA_Heartbeat();

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
            var message = new MSG_Gt_GtA_Heartbeat();
            message.m_PB = Gt_GtA_Heartbeat.Parser.ParseFrom(data, offset, length);
            message.m_Executor = m_Executor;
            return message;
        }
    }
}
