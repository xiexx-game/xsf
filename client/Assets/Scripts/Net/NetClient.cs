//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\Scripts\Net\NetClient.cs
// 作者：Xoen Xie
// 时间：2022/08/14
// 描述：网络客户端
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using System;
using UnityEngine;
using XsfPb;
using XSF;
using XsfMsg;

namespace XsfNet
{
    public enum ClientEventID
    {
        AllStartupStepDone = 1000,

        NetConnecting,
        NetConnected,
        NetError,
        NetHandshakeDone,

        LoadingDone,
    }

    public sealed class NetClient : INetHandler
    {
        private string m_sIP;
        private int m_nPort;

        private NetTcp m_Connection;
        private XSFWriter m_Writer;
        private XSFReader m_Reader;

        public bool m_bHandshake;

        private float m_fLastHeartbeatTime = 0;

        public uint mID;

        public NetClient()
        {
            m_Writer = new XSFWriter();
            m_Reader = new XSFReader();

            m_Connection = new NetTcp();
        }

        public bool Create(string sIP, int nPort)
        {

            m_sIP = sIP;
            m_nPort = nPort;

            m_Connection.Create(this);

            m_bHandshake = false;

            m_fLastHeartbeatTime = 0;

            return true;
        }

        public void Release()
        {
            m_Connection.Release();

            m_bHandshake = false;
        }

        public void DisConnect()
        {
            m_Connection.DisConnect();
        }

        public bool Connect()
        {
            if (!m_Connection.Connect(m_sIP, m_nPort))
            {
                return false;
            }

            XSFEvent.Instance.Fire((uint)ClientEventID.NetConnecting, 0);

            return true;
        }

        public void SendMessage(IMessage message)
        {
            if (m_Connection == null || !m_Connection.mIsConnected)
            {
                Debug.LogError($"NetClient.SendMessage connection not connected, msg id={message.ID}");
                return;
            }

            m_Writer.Clear();
            byte[] pbData = message.Export();
            uint pbLen = (uint)pbData.Length;
            // | 包长(uint 4字节) | 消息ID(ushort 2字节) | rawID(uint 4字节) | pb data |
            uint total = sizeof(ushort) + sizeof(uint) + pbLen;
            m_Writer.WriteUInt(total);
            m_Writer.WriteUShort((ushort)message.ID);
            m_Writer.WriteUInt((uint)message.ID);
            m_Writer.WriteBuffer(pbData);

            byte[] totalData = new byte[m_Writer.Size];
            Array.Copy(m_Writer.Buffer, totalData, m_Writer.Size);

            m_Connection.Send(totalData, (ushort)totalData.Length);
        }

        public void Update()
        {
            if (!m_Connection.mIsConnected)
                return;

            if (m_bHandshake)
            {
                float fCurTime = Time.realtimeSinceStartup;
                if (fCurTime >= XSFConfig.Instance.HeartbeatInterval + m_fLastHeartbeatTime)
                {
                    m_fLastHeartbeatTime = fCurTime;
                    SendHeartbeat();
                }
            }
        }

        public void FixedUpdate()
        {
            m_Connection.Dispath();
        }

        public void OnConnected()
        {
            XSFEvent.Instance.Fire((uint)ClientEventID.NetConnected, mID, null, true);

            Debug.Log($"NetClient::OnConnected ip={m_sIP}, port={m_nPort}");

            // 连接成功发送握手消息
            SendHandshake();
            m_fLastHeartbeatTime = 0;
        }

        public void OnRecv(byte[] data)
        {
            m_Reader.Init(data);
            ushort nMsgID = 0;

            m_Reader.ReadUShort(out nMsgID);

            Debug.Log($"NetClient OnRecv, msg id={nMsgID}");

            IMessage message = MessagePool.Instance.Get((XsfPb.CMSGID)nMsgID);
            message.Import(data, 2, data.Length - 2);
            message.Execute(this);
        }

        public void OnError(NetError nErrorCode)
        {
            Debug.LogError($"NetClient::OnError, 网络出错，错误码={nErrorCode}");

            XSFEvent.Instance.Fire((uint)ClientEventID.NetError, mID, (uint)nErrorCode, true);
        }




        private void SendHandshake()
        {
            SendMessage(MessagePool.Instance.Get(CMSGID.CltGtHandshake));
        }

        public void SendHeartbeat()
        {
            MSG_Clt_Gt_Heartbeat msg = MessagePool.Instance.Get(CMSGID.CltGtHeartbeat) as MSG_Clt_Gt_Heartbeat;
            msg.mPB.Time = XSFCore.CurrentMS;
            SendMessage(msg);
        }

        public void OnHandshake()
        {
            XSFCore.DebugLog("NetClient.OnHandshake start ...");
            XSFEvent.Instance.Fire((uint)ClientEventID.NetHandshakeDone, mID, null, true);
            m_bHandshake = true;
        }

    }
}

