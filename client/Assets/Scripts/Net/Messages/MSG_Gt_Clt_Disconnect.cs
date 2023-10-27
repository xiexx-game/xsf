//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\Scripts\Net\Messages\MSG_Gt_Clt_Disconnect.cs
// 作者：Xoen Xie
// 时间：10/26/2023
// 描述：gate --> client 服务器断开连接
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using XsfPb;
using XsfNet;

namespace XsfMsg
{
    public sealed class MSG_Gt_Clt_Disconnect : IMessage
    {
        public override CMSGID ID { get { return CMSGID.GtCltDisconnect; } }

        private Gt_Clt_Disconnect m_PB;
        public Gt_Clt_Disconnect mPB
        {
            get
            {
                if (m_PB == null)
                    m_PB = new Gt_Clt_Disconnect();

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
            m_PB = Gt_Clt_Disconnect.Parser.ParseFrom(data, offset, length);
        }

        // 消息处理接口
        public override void Execute(NetClient client)
        {



        }
    }
}