//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/XSF/Net/NetPacker.cs
// 作者：Xoen Xie
// 时间：2023/07/01
// 描述：网络协议数据打包器
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8603

namespace XSF
{
    public sealed class ServerPacker : INetPacker
    {
        private XSFWriter m_Writer;

        public int PackMinLength { get { return 6; } }

        public int PackMaxLength { get { return int.MaxValue; } }

        public ServerPacker()
        {
            m_Writer = new XSFWriter();
            
        }

        public byte[] Read(byte[] recvBuffer, int recvIndex, int nPackageLen, out IMessage message, out ushort nMessageID, out uint nRawID)
        {
            int pbLen = nPackageLen - sizeof(ushort) - sizeof(uint);
            Serilog.Log.Information("Read pbLen=" + pbLen);

            var reader = new XSFReader(recvBuffer, recvIndex + sizeof(uint));
            reader.ReadUShort(out nMessageID);
            reader.ReadUInt(out nRawID);

            Serilog.Log.Information("Read nMessageID=" + nMessageID + ", nRawID=" + nRawID);

            var msg = XSFUtil.GetMessage(nMessageID);
            message = msg.Import(reader.Buffer, reader.CurPos, pbLen);
            
            return null;
        }

        public byte[] Pack(IMessage message)
        {
            m_Writer.Clear();
            byte[] pbData = message.Export();
            Serilog.Log.Information("pack pbData length=" + pbData.Length);
            uint pbLen = (uint)pbData.Length;
            // | 包长(uint 4字节) | 消息ID(ushort 2字节) | rawID(uint 4字节) | pb data |
            uint total = sizeof(ushort) + sizeof(uint) + pbLen;
            Serilog.Log.Information("total = " + total);
            m_Writer.WriteUInt(total);
            m_Writer.WriteUShort(message.ID);
            m_Writer.WriteUInt((uint)message.ID);
            m_Writer.WriteBuffer(pbData);

            byte[] totalData = new byte[m_Writer.Size];
            Serilog.Log.Information("send total = " + totalData.Length);
            Array.Copy(m_Writer.Buffer, totalData, m_Writer.Size);

            return totalData;
        }
    }
}
