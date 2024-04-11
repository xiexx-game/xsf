//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/Message/Messages/MSG_Db_Dbc_Handshake.cs
// 作者：Xoen Xie
// 时间：4/11/2024
// 描述：db --> db connector 握手反馈
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8618
using XsfPbid;
using XsfPb;
using XSF;

namespace XsfMsg
{
    public sealed class MSG_Db_Dbc_Handshake : IMessage
    {
        public override ushort ID { get { return (ushort)SMSGID.DbDbcHandshake; } }

        public override byte DestEP { get { return (byte)EP.DB; } }

        private Db_Dbc_Handshake m_PB;
        public Db_Dbc_Handshake mPB
        {
            get
            {
                if (m_PB == null)
                    m_PB = new Db_Dbc_Handshake();

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
            var message = new MSG_Db_Dbc_Handshake();
            message.m_PB = Db_Dbc_Handshake.Parser.ParseFrom(data, offset, length);
            message.m_Executor = m_Executor;
            return message;
        }
    }
}
