//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/Message/Messages/MSG_Clt_G_Login.cs
// 作者：Xoen Xie
// 时间：2024/4/10
// 描述：client --> login 登录
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8618
using XsfPbid;
using XsfPb;
using XSF;

namespace XsfMsg
{
    public sealed class MSG_Clt_G_Login : IMessage
    {
        public override ushort ID { get { return (ushort)CMSGID.CltGLogin; } }

        public override byte DestEP { get { return (byte)EP.Game; } }

        private Clt_G_Login m_PB;
        public Clt_G_Login mPB
        {
            get
            {
                if (m_PB == null)
                    m_PB = new Clt_G_Login();

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
            var message = new MSG_Clt_G_Login();
            message.m_PB = Clt_G_Login.Parser.ParseFrom(data, offset, length);
            message.m_Executor = m_Executor;
            return message;
        }
    }
}
