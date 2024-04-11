//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/Message/Messages/MSG_Dbc_Db_Heartbeat.cs
// 作者：Xoen Xie
// 时间：4/11/2024
// 描述：db connector --> db 心跳
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8618
using XsfPbid;
using XsfPb;
using XSF;

namespace XsfMsg
{
    public sealed class MSG_Dbc_Db_Heartbeat : IMessage
    {
        public override ushort ID { get { return (ushort)SMSGID.DbcDbHeartbeat; } }

        public override byte DestEP { get { return (byte)EP.DB; } }

        private Dbc_Db_Heartbeat m_PB;
        public Dbc_Db_Heartbeat mPB
        {
            get
            {
                if (m_PB == null)
                    m_PB = new Dbc_Db_Heartbeat();

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
            var message = new MSG_Dbc_Db_Heartbeat();
            message.m_PB = Dbc_Db_Heartbeat.Parser.ParseFrom(data, offset, length);
            message.m_Executor = m_Executor;
            return message;
        }
    }
}
