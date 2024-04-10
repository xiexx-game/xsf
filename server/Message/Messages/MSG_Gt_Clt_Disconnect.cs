//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/Message/Messages/MSG_Gt_Clt_Disconnect.cs
// 作者：Xoen Xie
// 时间：2024/4/10
// 描述：gate --> client 服务器断开连接
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8618
using XsfPbid;
using XsfPb;
using XSF;

namespace XsfMsg
{
    public sealed class MSG_Gt_Clt_Disconnect : IMessage
    {
        public override ushort ID { get { return (ushort)CMSGID.GtCltDisconnect; } }

        public override byte DestEP { get { return (byte)EP.Gate; } }

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

        public override IMessage Import(byte[] data, int offset, int length)
        {
            var message = new MSG_Gt_Clt_Disconnect();
            message.m_PB = Gt_Clt_Disconnect.Parser.ParseFrom(data, offset, length);
            message.m_Executor = m_Executor;
            return message;
        }
    }
}
