//////////////////////////////////////////////////////////////////////////
// 
// 文件：XSF/Net/ConnectionTcp.cs
// 作者：Xoen Xie
// 时间：2023/07/01
// 描述：TCP连接
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using System;
using System.Net;
using System.Net.Sockets;

namespace XSF
{
    internal class ConnectionTcp : IConnection, IAsyncCallback
    {
        enum NetState
        { 
            None = 0,
            Connecting,
            Connected,
            Close,
            Release,
            NetStateError,
        }

        // 客户端接受包最大大小，32K
        private const ushort MAX_BUFFER_SIZE = 1024 * 32;

        private byte[] m_TempBuffer;
        private byte[] m_RecvBuffer;
        private byte[] m_LengthData;
        private byte[] m_IDData;

        private int m_nUintSize;
        private int m_nBufferLength;

        private Socket? m_Socket;
        private NetState m_nState;

        private INetHandler? m_Handler;
        private XSFWriter m_Writer;

        private AsyncCallback? m_AsyncConnect;
        private AsyncCallback? m_AsyncReceive;
        private AsyncCallback? m_AsyncSend;
        private AsyncCallback? m_AsyncAccept;

        public bool mIsConnected { get { return m_nState == NetState.Connected; } }

        public ConnectionTcp(INetHandler handler, AsyncCallback recevie, AsyncCallback send, AsyncCallback connect)
        {
            m_nState = NetState.None;

            m_TempBuffer = new byte[MAX_BUFFER_SIZE];
            m_RecvBuffer = new byte[MAX_BUFFER_SIZE];

            m_nUintSize = sizeof(uint);
            m_LengthData = new byte[4];
            m_IDData = new byte[2];
            m_nBufferLength = 0;

            m_Writer = new XSFWriter();

            m_AsyncReceive = recevie;
            m_AsyncSend = send;
            m_AsyncConnect = connect;

            m_Handler = handler;
        }

        public ConnectionTcp(Socket s, AsyncCallback? recevie, AsyncCallback? send)
        {
            m_nState = NetState.Connected;
            m_Socket = s;

            m_TempBuffer = new byte[MAX_BUFFER_SIZE];
            m_RecvBuffer = new byte[MAX_BUFFER_SIZE];

            m_nUintSize = sizeof(uint);
            m_LengthData = new byte[4];
            m_IDData = new byte[2];
            m_nBufferLength = 0;

            m_Writer = new XSFWriter();

            m_AsyncReceive = recevie;
            m_AsyncSend = send;
        }

        public void SetHandler(INetHandler handler)
        {
            m_Handler = handler;
        }

        public void Release()
        {
            m_Handler = null;
            Close();
            m_nState = NetState.Release;
        }

        public bool Connect(string ip, int port)
        {
            Serilog.Log.Information(string.Format("NetTcp.Connect Start Connect, ip={0}, port={1}", ip, port));
            if (m_nState == NetState.Connecting || m_nState == NetState.Connected)
            {
                Serilog.Log.Warning($"NetTcp.Connect igone .... m_nState:{m_nState}");
                return true;
            }

            try
            {
                m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                m_Socket.BeginConnect(ip, port, m_AsyncConnect, this);
            }
            catch (Exception e)
            {
                Serilog.Log.Error(string.Format("NetTcp.Connect caught exception, message={0}", e.Message));

                Close();
                XSFNet.Instance.PushEventError(this, m_Handler, NetError.Connect);

                return false;
            }

            m_nState = NetState.Connecting;

            return true;
        }

        public bool Listen(int port, AsyncCallback callback)
        {
            Serilog.Log.Information(string.Format("NetTcp.Listen Start Listen, port={0}", port));

            m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                IPAddress localAddress = IPAddress.Parse("0.0.0.0");
                IPEndPoint localEndPoint = new IPEndPoint(localAddress, port);
                m_Socket.Bind(localEndPoint);

                m_Socket.Listen(100);

                m_nState = NetState.Connected;

                m_AsyncAccept = callback;
                // 开始异步接受连接请求
                m_Socket.BeginAccept(m_AsyncAccept, this);
            }
            catch (Exception e)
            {
                Serilog.Log.Error(string.Format("NetTcp.Listen caught exception, message={0}", e.Message));

                Close();

                return false;
            }

            return true;
        }

        public bool Send(byte[] data, ushort length)
        {
            if (m_nState != NetState.Connected)
                return false;

            
            m_Writer.Clear();
            m_Writer.WriteUInt(length);
            m_Writer.WriteBuffer(data, length);

            int nSendSize = (int)m_Writer.Size;
            byte []sendData = new byte[nSendSize];
            Array.Copy(m_Writer.Buffer, sendData, nSendSize);

            try
            {
                m_Socket?.BeginSend(sendData, 0, nSendSize, SocketFlags.None, m_AsyncSend, this);
            }
            catch (SocketException e)
            {
                if (e.ErrorCode == (int)SocketError.WouldBlock)
                {
                    Serilog.Log.Warning("NetTcp::Send SocketError.WouldBlock");
                    return true;
                }

                Serilog.Log.Error($"NetTcp::Send 1 caught a exception, error={e.ErrorCode}, message={e.Message}");
                Close();
                XSFNet.Instance.PushEventError(this, m_Handler, NetError.Send);
            }
            catch (Exception e)
            {
                Serilog.Log.Error("NetTcp::Send 2 caught a exception, message:" + e.Message);
                Close();
                XSFNet.Instance.PushEventError(this, m_Handler, NetError.Send);
            }

            return true;
        }

        public void Receive()
        {
            if (m_nState != NetState.Connected)
                return;
            try
            {
                m_Socket?.BeginReceive(m_RecvBuffer, m_nBufferLength, m_RecvBuffer.Length - m_nBufferLength, SocketFlags.None, m_AsyncReceive, null);
            }
            catch (SocketException e)
            {
                if (e.ErrorCode == (int)SocketError.WouldBlock)
                {
                    Serilog.Log.Warning("NetTcp::Receive SocketError.WouldBlock");
                    return;
                }

                Serilog.Log.Error($"NetTcp::Receive 1 caught a exception, error={e.ErrorCode}, message={e.Message}");
                Close();
                XSFNet.Instance.PushEventError(this, m_Handler, NetError.Recv);
            }
            catch (Exception e)
            {
                Serilog.Log.Error("NetTcp::Receive 2 caught a exception, message:" + e.Message);
                Close();
                XSFNet.Instance.PushEventError(this, m_Handler, NetError.Recv);
            }
        }

        public void DoConnect(IAsyncResult iar)
        {
            try
            {
                m_Socket?.EndConnect(iar);
                m_nState = NetState.Connected;
                XSFNet.Instance.PushEventConnected(this, m_Handler);
                Serilog.Log.Information("NetTcp::OnEndConnect connected ...");
            }
            catch (Exception e)
            {
                Serilog.Log.Error("NetTcp::OnEndConnect caught a exception, message:" + e.Message);
                Close();
                XSFNet.Instance.PushEventError(this, m_Handler, NetError.Connect);
                return;
            }

            Receive();
        }

        public void DoSend(IAsyncResult iar)
        {
            try
            {
                m_Socket?.EndSend(iar);
            }
            catch (SocketException e)
            {
                if (e.ErrorCode == (int)SocketError.WouldBlock)
                {
                    Serilog.Log.Warning("NetTcp::OnEndSend SocketError.WouldBlock");
                    return;
                }

                Serilog.Log.Error($"NetTcp::OnEndSend 1 caught a exception, error={e.ErrorCode}, message={e.Message}");
                Close();
                XSFNet.Instance.PushEventError(this, m_Handler, NetError.Send);
            }
            catch (Exception e)
            {
                Serilog.Log.Error($"NetTcp::OnEndSend 2 caught a exception, message={e.Message}");
                Close();
                XSFNet.Instance.PushEventError(this, m_Handler, NetError.Send);
            }
        }

        public void DoReceive(IAsyncResult iar)
        {
            try
            {
                int nReceiveLength = 0;
                if(m_Socket != null)
                {
                    nReceiveLength = m_Socket.EndReceive(iar);
                    OnReceiveData(nReceiveLength);
                }
            }
            catch (SocketException e)
            {
                if (e.ErrorCode == (int)SocketError.WouldBlock)
                {
                    Serilog.Log.Warning("NetTcp::OnEndSend SocketError.WouldBlock");
                    return;
                }

                Serilog.Log.Error($"NetTcp::OnEndReceive 1 caught a exception, error={e.ErrorCode}, message={e.Message}");
                Close();
                XSFNet.Instance.PushEventError(this, m_Handler,NetError.Recv);
                return;
            }
            catch (Exception e)
            {
                Serilog.Log.Error($"NetTcp::OnEndReceive 2 caught a exception, message={e.Message}");
                Close();
                XSFNet.Instance.PushEventError(this, m_Handler,NetError.Recv);
                return;
            }

            Receive();
        }

        public void DoAccept(IAsyncResult iar)
        {
            try
            {
                // 接受传入的连接
                if(m_Socket != null)
                {
                    Socket clientSocket = m_Socket.EndAccept(iar);

                    // 处理连接建立后的操作
                    ConnectionTcp tcp = new ConnectionTcp(clientSocket, m_AsyncReceive, m_AsyncSend);
                    XSFNet.Instance.PushEventAccept(tcp, m_Handler);

                    // 开始异步接受下一个连接请求
                    m_Socket?.BeginAccept(m_AsyncAccept, m_Socket);
                }
            }
            catch(SocketException e)
            {
                Serilog.Log.Error($"NetTcp::OnEndAccept caught a exception, message={e.Message}");
            }
        }


        public void Close()
        {
            if (m_nState == NetState.Release || m_nState == NetState.Close)
                return;

            m_nState = NetState.Close;

            if (m_Socket != null)
            {
                try
                {
                    Serilog.Log.Information("NetTcp Close Shutdown");
                    m_Socket.Shutdown(SocketShutdown.Both);
                }
                catch
                {

                }

                try
                {
                    Serilog.Log.Information("NetTcp Close Close");
                    m_Socket.Close();
                }
                catch
                {

                }

                m_Socket = null;
            }
        }

        

        private int GetPackageLength( byte [] buffer, int startPos)
        {
            m_LengthData[0] = buffer[startPos];
            m_LengthData[1] = buffer[startPos+1];
            m_LengthData[2] = buffer[startPos+2];
            m_LengthData[3] = buffer[startPos+3];

            return BitConverter.ToInt32(m_LengthData, 0);
        }

        private void OnReceiveData(int nReceiveLength)
        {
            // 总数据长度
            int nTotalLen = m_nBufferLength + nReceiveLength;
            if ( nTotalLen <= m_nUintSize )   // 数据太小
            {
                m_nBufferLength += nReceiveLength;
                return;
            }

            int nPackageLen = GetPackageLength(m_RecvBuffer, 0);

            if (nPackageLen < 8)
            {
                throw new NetPackageParseException();
            }

            // 如果总长度 < 数据长度+ 数据头， 说明数据还未接收完
            if (nTotalLen < (int)(nPackageLen + m_nUintSize ))
            {
                m_nBufferLength += nReceiveLength;
                return;
            }
            else if (nTotalLen == nPackageLen + m_nUintSize)       // 刚好收到一个包
            {

                byte[] packetData = new byte[nPackageLen];
                Array.Copy(m_RecvBuffer, m_nUintSize, packetData, 0, nPackageLen);
                XSFNet.Instance.PushEventData(this, m_Handler, packetData);
                m_nBufferLength = 0;
                return;
            }
            else    // 收到了多个包，要把包都拆出来
            {
                Array.Copy(m_RecvBuffer, m_TempBuffer, nTotalLen);     // 把已经接收到的数据备份一下
                int nReadPos = 0;
                while (true)
                {
                    byte[] packetData = new byte[nPackageLen];
                    Array.Copy(m_TempBuffer, nReadPos + m_nUintSize, packetData, 0, nPackageLen);
                    XSFNet.Instance.PushEventData(this, m_Handler, packetData);

                    nReadPos += nPackageLen + m_nUintSize;
                    m_nBufferLength = nTotalLen - nReadPos;   // 剩余数据长度

                    if (m_nBufferLength <= m_nUintSize)      // 如果剩余的长度不够两个字节
                    {
                        Array.Copy(m_TempBuffer, nReadPos, m_RecvBuffer, 0, m_nBufferLength);       // 把剩余的数据拷贝回去，等待下剩余的数据
                        break;
                    }
                    else    // 如果超过四个字节
                    {
                        nPackageLen = GetPackageLength(m_TempBuffer, nReadPos);       // 得到下一个包的数据长度
                        if (nPackageLen <= 0)
                        {
                            throw new NetPackageParseException();
                        }

                        // 数据不够，继续去等待下一段数据
                        if (m_nBufferLength < nPackageLen + m_nUintSize )
                        {
                            Array.Copy(m_TempBuffer, nReadPos, m_RecvBuffer, 0, m_nBufferLength);
                            break;
                        }
                    }
                }
            }
        }
    }
}

