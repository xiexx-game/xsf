//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/Message/Messages/MSG_GtA_Gt_ClientDisconnect.cs
// 作者：Xoen Xie
// 时间：10/26/2023
// 描述：gate acceptor --> gate 主动关闭客户端连接
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8618
using XsfPbid;
using XsfPb;
using XSF;

namespace XsfMsg
{
    public sealed class MSG_GtA_Gt_ClientDisconnect : IMessage
    {
        public override ushort ID { get { return (ushort)SMSGID.GtAGtClientDisconnect; } }

        public override byte DestEP { get { return (byte)EP.Gate; } }

        private GtA_Gt_ClientDisconnect m_PB;
        public GtA_Gt_ClientDisconnect mPB
        {
            get
            {
                if (m_PB == null)
                    m_PB = new GtA_Gt_ClientDisconnect();

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
            var message = new MSG_GtA_Gt_ClientDisconnect();
            message.m_PB = GtA_Gt_ClientDisconnect.Parser.ParseFrom(data, offset, length);
            message.m_Executor = m_Executor;
            return message;
        }
    }
}
