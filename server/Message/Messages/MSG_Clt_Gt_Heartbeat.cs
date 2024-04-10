//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/Message/Messages/MSG_Clt_Gt_Heartbeat.cs
// 作者：Xoen Xie
// 时间：2024/4/10
// 描述：client --> gate 心跳
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8618
using XsfPbid;
using XsfPb;
using XSF;

namespace XsfMsg
{
    public sealed class MSG_Clt_Gt_Heartbeat : IMessage
    {
        public override ushort ID { get { return (ushort)CMSGID.CltGtHeartbeat; } }

        public override byte DestEP { get { return (byte)EP.Gate; } }

        private Clt_Gt_Heartbeat m_PB;
        public Clt_Gt_Heartbeat mPB
        {
            get
            {
                if (m_PB == null)
                    m_PB = new Clt_Gt_Heartbeat();

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
            var message = new MSG_Clt_Gt_Heartbeat();
            message.m_PB = Clt_Gt_Heartbeat.Parser.ParseFrom(data, offset, length);
            message.m_Executor = m_Executor;
            return message;
        }
    }
}
