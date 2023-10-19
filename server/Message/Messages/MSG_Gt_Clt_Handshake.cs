//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\Scripts\Net\Messages\MSG_Gt_Clt_Handshake.cs
// 作者：Xoen Xie
// 时间：2023/7/1
// 描述： gate --> client 握手反馈
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8618
using XsfPb;
using XSF;

namespace XsfMsg
{
    public sealed class MSG_Gt_Clt_Handshake : IMessage
    {
        public override ushort ID { get { return (ushort)CMSGID.GtCltHandshake; } }

        private Gt_Clt_Handshake m_PB;
        public Gt_Clt_Handshake mPB
        {
            get
            {
                if (m_PB == null)
                    m_PB = new Gt_Clt_Handshake();

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
            m_PB = Gt_Clt_Handshake.Parser.ParseFrom(data, offset, length);
        }
    }
}
