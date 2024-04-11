//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/Message/Messages/MSG_Db_Dbc_Response.cs
// 作者：Xoen Xie
// 时间：4/11/2024
// 描述：db --> db connector 请求结果
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8618
using XsfPbid;
using XsfPb;
using XSF;

namespace XsfMsg
{
    public sealed class MSG_Db_Dbc_Response : IMessage
    {
        public override ushort ID { get { return (ushort)SMSGID.DbDbcResponse; } }

        public override byte DestEP { get { return (byte)EP.DB; } }

        private Db_Dbc_Response m_PB;
        public Db_Dbc_Response mPB
        {
            get
            {
                if (m_PB == null)
                    m_PB = new Db_Dbc_Response();

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
            var message = new MSG_Db_Dbc_Response();
            message.m_PB = Db_Dbc_Response.Parser.ParseFrom(data, offset, length);
            message.m_Executor = m_Executor;
            return message;
        }
    }
}
