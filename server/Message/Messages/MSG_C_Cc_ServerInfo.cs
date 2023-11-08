//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/Message/Messages/MSG_C_Cc_ServerInfo.cs
// 作者：Xoen Xie
// 时间：10/26/2023
// 描述：center --> connector 下行服务器信息数据
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8618
using XsfPbid;
using XsfPb;
using XSF;

namespace XsfMsg
{
    public sealed class MSG_C_Cc_ServerInfo : IMessage
    {
        public override ushort ID { get { return (ushort)SMSGID.CCcServerInfo; } }

        public override byte DestEP { get { return (byte)EP.Center; } }

        private C_Cc_ServerInfo m_PB;
        public C_Cc_ServerInfo mPB
        {
            get
            {
                if (m_PB == null)
                    m_PB = new C_Cc_ServerInfo();

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
            var message = new MSG_C_Cc_ServerInfo();
            message.m_PB = C_Cc_ServerInfo.Parser.ParseFrom(data, offset, length);
            message.m_Executor = m_Executor;
            return message;
        }
    }
}
