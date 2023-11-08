//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/Message/Messages/MSG_G_Clt_LoginResult.cs
// 作者：Xoen Xie
// 时间：10/26/2023
// 描述：game --> client 登录结果
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8618
using XsfPbid;
using XsfPb;
using XSF;

namespace XsfMsg
{
    public sealed class MSG_G_Clt_LoginResult : IMessage
    {
        public override ushort ID { get { return (ushort)CMSGID.GCltLoginResult; } }

        public override byte DestEP { get { return (byte)EP.Game; } }

        private G_Clt_LoginResult m_PB;
        public G_Clt_LoginResult mPB
        {
            get
            {
                if (m_PB == null)
                    m_PB = new G_Clt_LoginResult();

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
            var message = new MSG_G_Clt_LoginResult();
            message.m_PB = G_Clt_LoginResult.Parser.ParseFrom(data, offset, length);
            message.m_Executor = m_Executor;
            return message;
        }
    }
}
