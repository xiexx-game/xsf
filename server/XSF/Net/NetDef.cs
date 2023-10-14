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
        bool Connect(string ip, int port);
        bool Send(byte[] data, ushort length);
        void Close();
        void Receive();
    }

    public interface INetHandler
    {
        void OnConnected(IConnection connection);
        void OnRecv(IConnection connection, byte[]? data);
        void OnError(IConnection connection, NetError nErrorCode);
        void OnAccept(IConnection connection);
    }

    internal class NetPackageParseException : Exception
    {
        public override string Message { get { return "Net ReceiveData catch Exception"; } }
    }

    internal interface IAsyncCallback
    {
        void DoConnect(IAsyncResult iar);
        void DoSend(IAsyncResult iar);
        void DoReceive(IAsyncResult iar);
        void DoAccept(IAsyncResult iar);
    }
}


