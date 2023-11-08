
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
    public sealed class LocalPakcer : ClientPakcer
    {
        public override byte[] Read(byte[] recvBuffer, int recvIndex, int nPackageLen, out IMessage message, out ushort nMessageID, out uint nRawID)
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
            case (ushort)XsfPbid.CMSGID.CltGtHandshake:
            case (ushort)XsfPbid.CMSGID.CltGtHeartbeat:
                {
                    var msg = XSFCore.GetMessage(nMessageID);
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
    }
}