//////////////////////////////////////////////////////////////////////////
// 
// 文件：XSF/Net/NetDef.cs
// 作者：Xoen Xie
// 时间：2023/07/01
// 描述：网络定义
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using System;

namespace XSF
{
    public enum NetError
    {
        None = 0,
        Connect,
        Recv,
        Send,
        Package,
    }

    public interface IConnection 
    {
        string RemoteIP { get; }
        
        bool Connect(string ip, int port);

        bool SendMessage(IMessage message);

        bool SendData(byte[] data);
        bool Close();
        void DoStart(INetHandler handler);
    }

    public interface INetHandler
    {
        void OnConnected(IConnection connection);
        void OnRecv(IConnection connection, IMessage message, ushort nMessageID, uint nRawID, byte[]? data);
        void OnError(IConnection connection, NetError nErrorCode);
        void OnAccept(IConnection connection);
    }

    internal class NetPackageParseException : Exception
    {
        public NetPackageParseException(string msg)
            : base(msg)
        {
            
        }
    }

    internal interface IAsyncCallback
    {
        void DoConnect(IAsyncResult iar);
        void DoSend(IAsyncResult iar);
        void DoReceive(IAsyncResult iar);
        void DoAccept(IAsyncResult iar);
    }

    public interface INetPacker
    {
        int PackMinLength { get; }

        int PackMaxLength { get; }

        byte[] Read(byte[] recvBuffer, int recvIndex, int nPackageLen, out IMessage message, out ushort nMessageID, out uint nRawID);

        byte[] Pack(IMessage message);
    }
}


