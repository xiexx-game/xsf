//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/Hub/Node/LocalPacker.cs
// 作者：Xoen Xie
// 时间：2023/07/01
// 描述：网络协议数据打包器
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8603, CS8625

namespace XSF
{
    public sealed class LocalPacker : ServerPacker
    {
        public override byte[] Read(byte[] recvBuffer, int recvIndex, int nPackageLen, out IMessage message, out ushort nMessageID, out uint nRawID)
        {
            int pbLen = nPackageLen - sizeof(ushort) - sizeof(uint);
            //Serilog.Log.Information("Read pbLen=" + pbLen);

            var reader = new XSFReader(recvBuffer, recvIndex + sizeof(uint));
            reader.ReadUShort(out nMessageID);
            reader.ReadUInt(out nRawID);

            //Serilog.Log.Information("Read nMessageID=" + nMessageID + ", nRawID=" + nRawID);

            switch(nMessageID)
            {
            case (ushort)XsfPbid.SMSGID.HcHHandshake:
            case (ushort)XsfPbid.SMSGID.HcHHeartbeat:
                {
                    var msg = XSFCore.GetMessage(nMessageID);
                    message = msg.Import(reader.Buffer, reader.CurPos, pbLen);
                    return null;
                }

            default:
                {
                    byte []bufferOut = new byte[sizeof(uint) + nPackageLen];
                    Array.Copy(recvBuffer, recvIndex, bufferOut, 0, bufferOut.Length);
                    message = null;
                    return bufferOut;
                }
            }
        }
    }
}
