//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\Scripts\Net\Messages\MSG_Clt_G_Login.cs
// 作者：Xoen Xie
// 时间：10/26/2023
// 描述：client --> login 登录
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using XsfPb;
using XsfPbid;
using XsfNet;

namespace XsfMsg
{
    public sealed class MSG_Clt_G_Login : IMessage
    {
        public override CMSGID ID { get { return CMSGID.CltGLogin; } }

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

        public override void Import(byte[] data, int offset, int length)
        {
            m_PB = Clt_G_Login.Parser.ParseFrom(data, offset, length);
        }

        // 消息处理接口
        public override void Execute(NetClient client)
        {



        }
    }
}