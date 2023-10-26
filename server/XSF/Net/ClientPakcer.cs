
//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/Gate/Client/ClientPakcer.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：客户端消息打包器
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8603, CS8625

namespace XSF
{
    public class ClientPakcer : INetPacker
    {
        private XSFWriter m_Writer;

        public override int PackMinLength { get { return 6; } }

        public override int PackMaxLength { get { return (int)XSFUtil.Config.ClientMsgMaxLength; } }

        public ClientPakcer()
        {
            m_Writer = new XSFWriter();
            
        }

        public override byte[] Pack(IMessage message)
        {
            m_Writer.Clear();
            byte[] pbData = message.Export();
            //Serilog.Log.Information("pack pbData length=" + pbData.Length);
            uint pbLen = (uint)pbData.Length;
            // | 包长(uint 4字节) | 消息ID(ushort 2字节) | pb data |
            uint total = sizeof(ushort) + pbLen;
            //Serilog.Log.Information("total = " + total);
            m_Writer.WriteUInt(total);
            m_Writer.WriteUShort(message.ID);
            m_Writer.WriteBuffer(pbData);

            byte[] totalData = new byte[m_Writer.Size];
            //Serilog.Log.Information("send total = " + totalData.Length);
            Array.Copy(m_Writer.Buffer, totalData, m_Writer.Size);

            return totalData;
        }
    }
}