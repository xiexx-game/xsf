//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\XSF\Net\NetDef.cs
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

    public interface INetHandler
    {
        void OnConnected();
        void OnRecv(byte[] data);
        void OnError(NetError nErrorCode);
    }

    public class NetPackageParseException : Exception
    {
        public override string Message { get { return "Net ReceiveData catch Exception"; } }
    }
}

