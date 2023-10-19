//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/Message/Messages/MSG_Gt_Clt_Heartbeat.cs
// 作者：Xoen Xie
// 时间：2023/7/1
// 描述： gate --> client 心跳 反馈
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8618
using XsfPb;
using XSF;

namespace XsfMsg
{
    public sealed class MSG_Gt_Clt_Heartbeat : IMessage
    {
        public override ushort ID { get { return (ushort)CMSGID.GtCltHeartbeat; } }

        private Gt_Clt_Heartbeat m_PB;
        public Gt_Clt_Heartbeat mPB
        {
            get
            {
                if (m_PB == null)
                    m_PB = new Gt_Clt_Heartbeat();

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
            m_PB = Gt_Clt_Heartbeat.Parser.ParseFrom(data, offset, length);
        }
    }
}
