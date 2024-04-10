//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/Message/Messages/MSG_Hc_H_Heartbeat.cs
// 作者：Xoen Xie
// 时间：2024/4/10
// 描述：hub connector --> hub 心跳
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8618
using XsfPbid;
using XsfPb;
using XSF;

namespace XsfMsg
{
    public sealed class MSG_Hc_H_Heartbeat : IMessage
    {
        public override ushort ID { get { return (ushort)SMSGID.HcHHeartbeat; } }

        public override byte DestEP { get { return (byte)EP.Hub; } }

        private Hc_H_Heartbeat m_PB;
        public Hc_H_Heartbeat mPB
        {
            get
            {
                if (m_PB == null)
                    m_PB = new Hc_H_Heartbeat();

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
            var message = new MSG_Hc_H_Heartbeat();
            message.m_PB = Hc_H_Heartbeat.Parser.ParseFrom(data, offset, length);
            message.m_Executor = m_Executor;
            return message;
        }
    }
}
