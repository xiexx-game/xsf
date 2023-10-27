//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\Scripts\Net\Messages\MSG_G_Clt_TestData.cs
// 作者：Xoen Xie
// 时间：10/26/2023
// 描述：game --> client 测试协议
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using XsfPb;
using XsfNet;

namespace XsfMsg
{
    public sealed class MSG_G_Clt_TestData : IMessage
    {
        public override CMSGID ID { get { return CMSGID.GCltTestData; } }

        private G_Clt_TestData m_PB;
        public G_Clt_TestData mPB
        {
            get
            {
                if (m_PB == null)
                    m_PB = new G_Clt_TestData();

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
            m_PB = G_Clt_TestData.Parser.ParseFrom(data, offset, length);
        }

        // 消息处理接口
        public override void Execute(NetClient client)
        {



        }
    }
}