//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/Message/Messages/MSG_G_G_HubTest.cs
// 作者：Xoen Xie
// 时间：4/11/2024
// 描述：game --> game 中转测试
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8618
using XsfPbid;
using XsfPb;
using XSF;

namespace XsfMsg
{
    public sealed class MSG_G_G_HubTest : IMessage
    {
        public override ushort ID { get { return (ushort)SMSGID.GGHubTest; } }

        public override byte DestEP { get { return (byte)EP.Game; } }

        private G_G_HubTest m_PB;
        public G_G_HubTest mPB
        {
            get
            {
                if (m_PB == null)
                    m_PB = new G_G_HubTest();

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
            var message = new MSG_G_G_HubTest();
            message.m_PB = G_G_HubTest.Parser.ParseFrom(data, offset, length);
            message.m_Executor = m_Executor;
            return message;
        }
    }
}
