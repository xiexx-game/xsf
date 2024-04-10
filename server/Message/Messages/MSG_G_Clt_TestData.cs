//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/Message/Messages/MSG_G_Clt_TestData.cs
// 作者：Xoen Xie
// 时间：2024/4/10
// 描述：game --> client 测试协议
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8618
using XsfPbid;
using XsfPb;
using XSF;

namespace XsfMsg
{
    public sealed class MSG_G_Clt_TestData : IMessage
    {
        public override ushort ID { get { return (ushort)CMSGID.GCltTestData; } }

        public override byte DestEP { get { return (byte)EP.Game; } }

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

        public override IMessage Import(byte[] data, int offset, int length)
        {
            var message = new MSG_G_Clt_TestData();
            message.m_PB = G_Clt_TestData.Parser.ParseFrom(data, offset, length);
            message.m_Executor = m_Executor;
            return message;
        }
    }
}
