//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\Scripts\Net\Messages\MSG_Clt_Gt_Heartbeat.cs
// 作者：Xoen Xie
// 时间：2023/7/1
// 描述： client --> gate 心跳 请求
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8618

using XsfPb;
using XSF;

namespace XsfMsg
{
    public sealed class MSG_Clt_Gt_Heartbeat : IMessage
    {
        public override ushort ID { get { return (ushort)CMSGID.CltGtHeartbeat; } }

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

        public override void Export(XSFWriter writer)
        {
            // 写入PB数据
            byte[] bytes = new byte[mPB.CalculateSize()];
            mPB.WriteTo(new Google.Protobuf.CodedOutputStream(bytes));
            writer.WriteBuffer(bytes);
        }

        public override void Import(byte[] data, int offset, int length)
        {
            m_PB = Clt_Gt_Heartbeat.Parser.ParseFrom(data, offset, length);
        }
    }
}
