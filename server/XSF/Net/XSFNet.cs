//////////////////////////////////////////////////////////////////////////
// 
// 文件：XSF/Net/XSFNet.cs
// 作者：Xoen Xie
// 时间：2023/07/01
// 描述：网络模块
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using System;
using System.Net;
using System.Net.Sockets;

namespace XSF
{
    internal sealed class XSFNet : Singleton<XSFNet>
    {
        enum NetInfoType
        {
            None = 0,
            Error,
            Connected,
            Data,
            Accept,
        }

        struct NetInfo
        {
            public IConnection connection;
            public INetHandler? handler;
            public NetInfoType type;
            public byte[]? data;
            public NetError error;
        }

        private object m_Lock;

        private AsyncCallback m_AsyncConnect;
        private AsyncCallback m_AsyncReceive;
        private AsyncCallback m_AsyncSend;
        private AsyncCallback m_AsyncAccept;

        private XSFQueue<NetInfo> m_EventQueue;

        public XSFNet()
        {
            m_EventQueue = new XSFQueue<NetInfo>();

            m_AsyncConnect = new AsyncCallback(OnEndConnect);
            m_AsyncReceive = new AsyncCallback(OnEndReceive);
            m_AsyncSend = new AsyncCallback(OnEndSend);
            m_AsyncAccept = new AsyncCallback(OnEndAccept);

            m_Lock = new object();
        }

        internal IConnection? Listen(INetHandler handler, int port)
        {
            ConnectionTcp tcp = new ConnectionTcp(handler, m_AsyncReceive, m_AsyncSend, m_AsyncConnect);
            if( tcp.Listen(port, m_AsyncAccept) )
                return tcp;

            return null;
        }

        internal IConnection? Connect(INetHandler handler, string ip, int port)
        {
            ConnectionTcp tcp = new ConnectionTcp(handler, m_AsyncReceive, m_AsyncSend, m_AsyncConnect);
            if(tcp.Connect(ip, port))
            {
                return tcp;
            }

            return null;
        }

        internal void PushEventError(IConnection connection, INetHandler? handler, NetError error)
        {
            NetInfo info;
            info.type = NetInfoType.Error;
            info.data = null;
            info.error = error;
            info.connection = connection;
            info.handler = handler;

            lock (m_Lock)
            {
                m_EventQueue.Push(info);
            }
        }

        internal void PushEventData(IConnection connection, INetHandler? handler, byte[] data)
        {
            NetInfo info;
            info.type = NetInfoType.Data;
            info.data = data;
            info.error = NetError.None;
            info.connection = connection;
            info.handler = handler;

            lock (m_Lock)
            {
                m_EventQueue.Push(info);
            }
        }

        internal void PushEventConnected(IConnection connection, INetHandler? handler)
        {
            NetInfo info;
            info.type = NetInfoType.Connected;
            info.data = null;
            info.error = NetError.None;
            info.connection = connection;
            info.handler = handler;

            lock (m_Lock)
            {
                m_EventQueue.Push(info);
            }
        }

        internal void PushEventAccept(IConnection connection, INetHandler? handler)
        {
            NetInfo info;
            info.type = NetInfoType.Accept;
            info.data = null;
            info.error = NetError.None;
            info.connection = connection;
            info.handler = handler;

            lock (m_Lock)
            {
                m_EventQueue.Push(info);
            }
        }

        public void Dispath()
        {
            NetInfo info;
            while (m_EventQueue != null && m_EventQueue.Pop(out info))
            {
                switch (info.type)
                {
                    case NetInfoType.Connected:
                        info.handler?.OnConnected(info.connection);
                        break;

                    case NetInfoType.Error:
                        info.handler?.OnError(info.connection, info.error);
                        break;

                    case NetInfoType.Data:
                        info.handler?.OnRecv(info.connection, info.data);
                        break;

                    case NetInfoType.Accept:
                        info.handler?.OnAccept(info.connection);
                        break;

                    default:
                        Serilog.Log.Error("XSFNet.Dispath type error, type={0}", info.type);
                        break;
                }
            }
        }


        private void OnEndConnect(IAsyncResult iar)
        {
            IAsyncCallback? callback = iar.AsyncState as IAsyncCallback;
            callback?.DoConnect(iar);
        }

        private void OnEndSend(IAsyncResult iar)
        {
            IAsyncCallback? callback = iar.AsyncState as IAsyncCallback;
            callback?.DoSend(iar);
        }

        private void OnEndReceive(IAsyncResult iar)
        {
            IAsyncCallback? callback = iar.AsyncState as IAsyncCallback;
            callback?.DoReceive(iar);
            
        }

        private void OnEndAccept(IAsyncResult iar)
        {
            IAsyncCallback? callback = iar.AsyncState as IAsyncCallback;
            callback?.DoAccept(iar);
        }
    }
}