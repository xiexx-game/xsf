//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/Message/Messages/MSG_L_Clt_LoginResult.cs
// 作者：Xoen Xie
// 时间：10/25/2023
// 描述：login --> client 登录结果
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8618

using XsfPb;
using XSF;

namespace XsfMsg
{
    public sealed class MSG_L_Clt_LoginResult : IMessage
    {
        public override ushort ID { get { return (ushort)CMSGID.LCltLoginResult; } }

        private L_Clt_LoginResult m_PB;
        public L_Clt_LoginResult mPB
        {
            get
            {
                if (m_PB == null)
                    m_PB = new L_Clt_LoginResult();

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
            var message = new MSG_L_Clt_LoginResult();
            message.m_PB = L_Clt_LoginResult.Parser.ParseFrom(data, offset, length);
            message.m_Executor = m_Executor;
            return message;
        }
    }
}
