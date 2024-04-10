//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/Message/Messages/MSG_Dbc_Db_Request.cs
// 作者：Xoen Xie
// 时间：2024/4/10
// 描述：db connector --> db 请求
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8618
using XsfPbid;
using XsfPb;
using XSF;

namespace XsfMsg
{
    public sealed class MSG_Dbc_Db_Request : IMessage
    {
        public override ushort ID { get { return (ushort)SMSGID.DbcDbRequest; } }

        public override byte DestEP { get { return (byte)EP.DB; } }

        private Dbc_Db_Request m_PB;
        public Dbc_Db_Request mPB
        {
            get
            {
                if (m_PB == null)
                    m_PB = new Dbc_Db_Request();

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
            var message = new MSG_Dbc_Db_Request();
            message.m_PB = Dbc_Db_Request.Parser.ParseFrom(data, offset, length);
            message.m_Executor = m_Executor;
            return message;
        }
    }
}
