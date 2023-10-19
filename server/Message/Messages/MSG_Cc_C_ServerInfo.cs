//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/Message/Messages/MSG_Cc_C_ServerInfo.cs
// 作者：Xoen Xie
// 时间：10/19/2023
// 描述：connector --> center 上行服务器信息数据
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8618

using XsfPb;
using XSF;

namespace XsfMsg
{
    public sealed class MSG_Cc_C_ServerInfo : IMessage
    {
        public override ushort ID { get { return (ushort)SMSGID.CcCServerInfo; } }

        private Cc_C_ServerInfo m_PB;
        public Cc_C_ServerInfo mPB
        {
            get
            {
                if (m_PB == null)
                    m_PB = new Cc_C_ServerInfo();

                return m_PB;
            }
        }

        public override void Export(XSFWriter writer)
        {
            // 写入PB数据
            byte[] bytes = new byte[mPB.CalculateSize()];
            mPB.WriteTo(new Google.Protobuf.CodedOutputStream(bytes));
            writer.WriteBuffer(bytes);
        }

        public override void Import(byte[] data, int offset, int length)
        {
            m_PB = Cc_C_ServerInfo.Parser.ParseFrom(data, offset, length);
        }
    }
}
