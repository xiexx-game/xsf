//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/Message/Messages/MSG_Gt_GtA_ClientClose.cs
// 作者：Xoen Xie
// 时间：10/26/2023
// 描述：gete --> gate acceptor 客户端连接关闭
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8618

using XsfPb;
using XSF;

namespace XsfMsg
{
    public sealed class MSG_Gt_GtA_ClientClose : IMessage
    {
        public override ushort ID { get { return (ushort)SMSGID.GtGtAClientClose; } }

        public override byte DestEP { get { return (byte)EP.Gate; } }

        private Gt_GtA_ClientClose m_PB;
        public Gt_GtA_ClientClose mPB
        {
            get
            {
                if (m_PB == null)
                    m_PB = new Gt_GtA_ClientClose();

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
            var message = new MSG_Gt_GtA_ClientClose();
            message.m_PB = Gt_GtA_ClientClose.Parser.ParseFrom(data, offset, length);
            message.m_Executor = m_Executor;
            return message;
        }
    }
}
