//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/Message/Messages/MSG_GtA_Gt_SetServerID.cs
// 作者：Xoen Xie
// 时间：10/25/2023
// 描述：gate acceptor --> gate 设置服务器id
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8618

using XsfPb;
using XSF;

namespace XsfMsg
{
    public sealed class MSG_GtA_Gt_SetServerID : IMessage
    {
        public override ushort ID { get { return (ushort)SMSGID.GtAGtSetServerId; } }

        private GtA_Gt_SetServerID m_PB;
        public GtA_Gt_SetServerID mPB
        {
            get
            {
                if (m_PB == null)
                    m_PB = new GtA_Gt_SetServerID();

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
            var message = new MSG_GtA_Gt_SetServerID();
            message.m_PB = GtA_Gt_SetServerID.Parser.ParseFrom(data, offset, length);
            message.m_Executor = m_Executor;
            return message;
        }
    }
}
