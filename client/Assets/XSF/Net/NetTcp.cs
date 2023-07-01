//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\XSF\Net\NetTcp.cs
// 作者：Xoen Xie
// 时间：2023/07/01
// 描述：TCP连接
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using System;
using System.Net;
using System.Net.Sockets;


public class NetTcp
{
    enum NetInfoType
    {
        None = 0,
        Error,
        Connected,
        Data,
    }

    struct NetInfo
    {
        public NetInfoType type;
        public byte[] data;
        public NetError error;
    }

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

    private Socket m_Socket;
    private NetState m_nState;

    private INetHandler m_Handler;
    private XSFWriter m_Writer;

    private AsyncCallback m_AsyncConnect;
    private AsyncCallback m_AsyncReceive;
    private AsyncCallback m_AsyncSend;

    private XSFQueue<NetInfo> m_EventQueue;

    public bool mIsConnected { get { return m_nState == NetState.Connected; } }

    public NetTcp()
    {
        m_nState = NetState.None;

        m_TempBuffer = new byte[MAX_BUFFER_SIZE];
        m_RecvBuffer = new byte[MAX_BUFFER_SIZE];

        m_nUintSize = sizeof(uint);
        m_LengthData = new byte[4];
        m_IDData = new byte[2];
        m_nBufferLength = 0;

        m_Writer = new XSFWriter();

        m_AsyncConnect = new AsyncCallback(OnEndConnect);
        m_AsyncReceive = new AsyncCallback(OnEndReceive);
        m_AsyncSend = new AsyncCallback(OnEndSend);
    }

    public bool Create(INetHandler handler)
    {
        m_Handler = handler;
        m_EventQueue = new XSFQueue<NetInfo>();

        return true;
    }

    public void Release()
    {
        m_Handler = null;
        Close();
        m_nState = NetState.Release;
        m_EventQueue = null;
    }

    public bool Connect(string ip, int port)
    {
        XSF.Log(string.Format("NetTcp.Connect Start Connect, ip={0}, port={1}", ip, port));
        if (m_nState == NetState.Connecting || m_nState == NetState.Connected)
        {
            XSF.LogWarning($"NetTcp.Connect igone .... m_nState:{m_nState}");
            return true;
        }

        try
        {
            IPAddress[] address = Dns.GetHostAddresses(ip);
            for (int i = 0; i < address.Length; ++i)
                XSF.Log(string.Format("NetTcp.Connect Address:{0}, AddressFamily:{1}", address[i].ToString(), address[i].AddressFamily));

            m_Socket = new Socket(address[0].AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            m_Socket.BeginConnect(address[0], port, m_AsyncConnect, null);
        }
        catch (Exception e)
        {
            XSF.LogError(string.Format("NetTcp.Connect caught exception, message={0}", e.Message));

            Close();
            PushEventError(NetError.Connect);

            return false;
        }

        m_nState = NetState.Connecting;

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
            m_Socket.BeginSend(sendData, 0, nSendSize, SocketFlags.None, m_AsyncSend, null);
        }
        catch (SocketException e)
        {
            if (e.ErrorCode == (int)SocketError.WouldBlock)
            {
                XSF.LogWarning("NetTcp::Send SocketError.WouldBlock");
                return true;
            }

            XSF.LogError($"NetTcp::Send 1 caught a exception, error={e.ErrorCode}, message={e.Message}");
            Close();
            PushEventError(NetError.Send);
        }
        catch (Exception e)
        {
            XSF.LogError("NetTcp::Send 2 caught a exception, message:" + e.Message);
            Close();
            PushEventError(NetError.Send);
        }

        return true;
    }

    public void Receive()
    {
        if (m_nState != NetState.Connected)
            return;
        try
        {
            m_Socket.BeginReceive(m_RecvBuffer, m_nBufferLength, m_RecvBuffer.Length - m_nBufferLength, SocketFlags.None, m_AsyncReceive, null);
        }
        catch (SocketException e)
        {
            if (e.ErrorCode == (int)SocketError.WouldBlock)
            {
                XSF.LogWarning("NetTcp::Receive SocketError.WouldBlock");
                return;
            }

            XSF.LogError($"NetTcp::Receive 1 caught a exception, error={e.ErrorCode}, message={e.Message}");
            Close();
            PushEventError(NetError.Recv);
        }
        catch (Exception e)
        {
            XSF.LogError("NetTcp::Receive 2 caught a exception, message:" + e.Message);
            Close();
            PushEventError(NetError.Recv);
        }
    }


    public void DisConnect()
    {
        if (m_nState != NetState.Connected && m_nState != NetState.Connecting )
            return;

        Close();
        PushEventError(NetError.None);
    }


    private void Close()
    {
        if (m_nState == NetState.Release || m_nState == NetState.Close)
            return;

        m_nState = NetState.Close;

        if (m_Socket != null)
        {
            try
            {
                XSF.Log("NetTcp Close Shutdown");
                m_Socket.Shutdown(SocketShutdown.Both);
            }
            catch
            {

            }

            try
            {
                XSF.Log("NetTcp Close Close");
                m_Socket.Close();
            }
            catch
            {

            }

            m_Socket = null;
        }
    }

    private void PushEventError(NetError error)
    {
        if (m_nState == NetState.Release || m_nState == NetState.None)
            return;

        NetInfo info;
        info.type = NetInfoType.Error;
        info.data = null;
        info.error = error;
        m_EventQueue.Push(info);
    }

    private void PushEventData(byte[] data)
    {
        if (m_nState == NetState.Release || m_nState == NetState.None)
            return;

        NetInfo info;
        info.type = NetInfoType.Data;
        info.data = data;
        info.error = NetError.None;
        m_EventQueue.Push(info);
        
    }

    private void PushEventConnected()
    {
        if (m_nState == NetState.Release || m_nState == NetState.None)
            return;

        NetInfo info;
        info.type = NetInfoType.Connected;
        info.data = null;
        info.error = NetError.None;
        m_EventQueue.Push(info);
    }

    public void Dispath()
    {
        if (m_nState == NetState.Release || m_nState == NetState.None)
        {
            return;
        }
            
        NetInfo info;
        while (m_EventQueue != null && m_EventQueue.Pop(out info))
        {
            switch (info.type)
            {
                case NetInfoType.Connected:
                    m_Handler.OnConnected();
                    break;

                case NetInfoType.Error:
                    m_Handler.OnError(info.error);
                    break;

                case NetInfoType.Data:
                    m_Handler.OnRecv(info.data);
                    break;
            }
        }
    }


    private void OnEndConnect(IAsyncResult iar)
    {
        try
        {
            m_Socket.EndConnect(iar);
            m_nState = NetState.Connected;
            PushEventConnected();
            XSF.Log("NetTcp::OnEndConnect connected ...");
        }
        catch (Exception e)
        {
            XSF.LogError("NetTcp::OnEndConnect caught a exception, message:" + e.Message);
            Close();
            PushEventError(NetError.Connect);
            return;
        }

        Receive();
    }

    private void OnEndSend(IAsyncResult iar)
    {
        if (m_nState != NetState.Connected)
            return;
        try
        {
            m_Socket.EndSend(iar);
        }
        catch (SocketException e)
        {
            if (e.ErrorCode == (int)SocketError.WouldBlock)
            {
                XSF.LogWarning("NetTcp::OnEndSend SocketError.WouldBlock");
                return;
            }

            XSF.LogError($"NetTcp::OnEndSend 1 caught a exception, error={e.ErrorCode}, message={e.Message}");
            Close();
            PushEventError(NetError.Send);
        }
        catch (Exception e)
        {
            XSF.LogError($"NetTcp::OnEndSend 2 caught a exception, message={e.Message}");
            Close();
            PushEventError(NetError.Send);
        }
    }

    private void OnEndReceive(IAsyncResult iar)
    {
        if (m_nState != NetState.Connected)
            return;
        try
        {
            int nReceiveLength = m_Socket.EndReceive(iar);
            OnReceiveData(nReceiveLength);
        }
        catch (SocketException e)
        {
            if (e.ErrorCode == (int)SocketError.WouldBlock)
            {
                XSF.LogWarning("NetTcp::OnEndSend SocketError.WouldBlock");
                return;
            }

            XSF.LogError($"NetTcp::OnEndReceive 1 caught a exception, error={e.ErrorCode}, message={e.Message}");
            Close();
            PushEventError(NetError.Recv);
            return;
        }
        catch (Exception e)
        {
            XSF.LogError($"NetTcp::OnEndReceive 2 caught a exception, message={e.Message}");
            Close();
            PushEventError(NetError.Recv);
            return;
        }

        Receive();
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
            PushEventData(packetData);
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
                PushEventData(packetData);

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
