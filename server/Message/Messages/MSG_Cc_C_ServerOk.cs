//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/Message/Messages/MSG_Cc_C_ServerOk.cs
// 作者：Xoen Xie
// 时间：10/19/2023
// 描述：connector --> center 服务器已准备好
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8618

using XsfPb;
using XSF;

namespace XsfMsg
{
    public sealed class MSG_Cc_C_ServerOk : IMessage
    {
        public override ushort ID { get { return (ushort)SMSGID.CcCServerOk; } }

        private Cc_C_ServerOk m_PB;
        public Cc_C_ServerOk mPB
        {
            get
            {
                if (m_PB == null)
                    m_PB = new Cc_C_ServerOk();

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
            m_PB = Cc_C_ServerOk.Parser.ParseFrom(data, offset, length);
        }
    }
}
