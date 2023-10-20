//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/Message/Messages/MSG_Clt_L_Login.cs
// 作者：Xoen Xie
// 时间：10/19/2023
// 描述：client --> login 登录
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8618

using XsfPb;
using XSF;

namespace XsfMsg
{
    public sealed class MSG_Clt_L_Login : IMessage
    {
        public override ushort ID { get { return (ushort)CMSGID.CltLLogin; } }

        private Clt_L_Login m_PB;
        public Clt_L_Login mPB
        {
            get
            {
                if (m_PB == null)
                    m_PB = new Clt_L_Login();

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
            var message = new MSG_Clt_L_Login();
            message.m_PB = Clt_L_Login.Parser.ParseFrom(data, offset, length);
            message.m_Executor = m_Executor;
            return message;
        }
    }
}
