//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\Scripts\Net\XSFNet.cs
// 作者：Xoen Xie
// 时间：2022/08/14
// 描述：网络管理
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using UnityEngine;
using XSF;

namespace XsfNet
{
    public sealed class XSFNet : Singleton<XSFNet>, IUpdateNode, IEventSink
    {
        public NetClient mClient { get; private set; }

        private uint m_nClientID;

        // 服务器当前时间戳：毫秒
        public ulong ServerTimeMS { get { return m_ServerTimeStart + (XSFCore.CurrentMS - m_TimeClient); } }

        // 服务器当前时间戳：秒
        public uint ServerTime { get { return (uint)(ServerTimeMS / 1000); } }

        public uint RTT { get; private set; }

        public bool Init()
        {
            MessagePool.Instance.Init();
            XSFEvent.Instance.Subscribe(this, (uint)ClientEventID.NetError, 0);

            m_nClientID = 1;

            return true;
        }

        public void Start()
        {
            MessagePool.Instance.Init();
        }

        public void Release()
        {
            if (mClient != null)
            {
                mClient.Release();
                mClient = null;
            }
        }

        public NetClient CreateNetClient(string sIP, int nPort)
        {
            if (mClient != null)
            {
                Debug.Log("CreateNetClient mClient.Release");
                mClient.Release();
            }

            mClient = new NetClient();
            mClient.mID = m_nClientID++;
            if (!mClient.Create(sIP, nPort))
            {
                mClient.Release();
                mClient = null;
            }

            mClient.Connect();

            XSFUpdate.Instance.Add(this);

            return mClient;
        }

        public void SendMessage(IMessage message)
        {
            if (mClient == null)
            {
                Debug.LogWarning("NetManager.SendMessage mClient == null, message=" + message.ID);
                return;
            }


            mClient.SendMessage(message);
        }


        public bool IsUpdateWroking
        {
            get { return mClient != null; }
        }

        public void OnUpdate()
        {
            mClient.Update();
        }

        public void OnFixedUpdate()
        {
            mClient.FixedUpdate();
        }

        public bool OnEvent(uint nEventID, uint nObjectID, object context)
        {
            if (nEventID == (uint)ClientEventID.NetError)
            {
                mClient.Release();
                mClient = null;
            }

            return true;
        }

        private ulong m_ServerTimeStart;    // 本地客户端模拟的服务器开始时间
        private ulong m_TimeClient;         // 本地客户端模拟服务器时间的开始时间

        public void SetTime(ulong timeClient, ulong timeServer)
        {
            m_TimeClient = XSFCore.CurrentMS;

            var timeOffset = m_TimeClient - timeClient;
            RTT = (uint)(timeOffset / 2);
            m_ServerTimeStart = timeServer + RTT;
        }
    }
}