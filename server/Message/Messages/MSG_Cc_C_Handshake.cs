//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/Message/Messages/MSG_Cc_C_Handshake.cs
// 作者：Xoen Xie
// 时间：10/19/2023
// 描述：center connector --> center 握手
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8618

using XsfPb;
using XSF;

namespace XsfMsg
{
    public sealed class MSG_Cc_C_Handshake : IMessage
    {
        public override ushort ID { get { return (ushort)SMSGID.CcCHandshake; } }

        private Cc_C_Handshake m_PB;
        public Cc_C_Handshake mPB
        {
            get
            {
                if (m_PB == null)
                    m_PB = new Cc_C_Handshake();

                return m_PB;
            }
        }

        public override void Export(XSFWriter writer)
        {
            // 写入PB数据
            byte[] bytes = new byte[mPB.CalculateSize()];
            mPB.WriteTo(new Google.Protobuf.CodedOutputStream(bytes));
            writer.WriteBuffer(bytes);
        }

        public override void Import(byte[] data, int offset, int length)
        {
            m_PB = Cc_C_Handshake.Parser.ParseFrom(data, offset, length);
        }
    }
}
