
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

using XSF;

namespace GateClient
{
    public sealed class ClientPakcer : INetPacker
    {
        private XSFWriter m_Writer;

        public int PackMinLength { get { return 6; } }

        public int PackMaxLength { get { return (int)XSFUtil.Config.ClientMsgMaxLength; } }

        public ClientPakcer()
        {
            m_Writer = new XSFWriter();
            
        }

        public byte[] Read(byte[] recvBuffer, int recvIndex, int nPackageLen, out IMessage message, out ushort nMessageID, out uint nRawID)
        {
            int pbLen = nPackageLen - sizeof(ushort) - sizeof(uint);
            //Serilog.Log.Information("Read pbLen=" + pbLen);

            var reader = new XSFReader(recvBuffer, recvIndex + sizeof(uint));
            reader.ReadUShort(out nMessageID);
            reader.ReadUInt(out nRawID);

            //Serilog.Log.Information("Read nMessageID=" + nMessageID + ", nRawID=" + nRawID);

            byte []bufferOut;

            switch(nMessageID)
            {
            case (ushort)XsfPb.CMSGID.CltGtHandshake:
            case (ushort)XsfPb.CMSGID.CltGtHeartbeat:
                {
                    var msg = XSFUtil.GetMessage(nMessageID);
                    message = msg.Import(reader.Buffer, reader.CurPos, pbLen);
                    return null;
                }

            default:
                {
                    bufferOut = new byte[sizeof(uint) + nPackageLen];
                    Array.Copy(recvBuffer, recvIndex, bufferOut, 0, bufferOut.Length);
                    message = null;
                    return bufferOut;
                }
            }
        }

        public byte[] Pack(IMessage message)
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