//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\Scripts\Net\Messages\MSG_Clt_Gt_Heartbeat.cs
// 作者：Xoen Xie
// 时间：10/26/2023
// 描述：client --> gate 心跳
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using XsfPb;
using XsfPbid;
using XsfNet;

namespace XsfMsg
{
    public sealed class MSG_Clt_Gt_Heartbeat : IMessage
    {
        public override CMSGID ID { get { return CMSGID.CltGtHeartbeat; } }

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

        public override void Import(byte[] data, int offset, int length)
        {
            m_PB = Clt_Gt_Heartbeat.Parser.ParseFrom(data, offset, length);
        }

        // 消息处理接口
        public override void Execute(NetClient client)
        {



        }
    }
}