//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\Scripts\Net\Messages\MSG_G_Clt_LoginResult.cs
// 作者：Xoen Xie
// 时间：10/26/2023
// 描述：game --> client 登录结果
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using XsfPb;
using XsfNet;

namespace XsfMsg
{
    public sealed class MSG_G_Clt_LoginResult : IMessage
    {
        public override CMSGID ID { get { return CMSGID.GCltLoginResult; } }

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

        public override void Import(byte[] data, int offset, int length)
        {
            m_PB = G_Clt_LoginResult.Parser.ParseFrom(data, offset, length);
        }

        // 消息处理接口
        public override void Execute(NetClient client)
        {
            UnityEngine.Debug.Log("G_Clt_LoginResult result=" + mPB.Result);
        }
    }
}