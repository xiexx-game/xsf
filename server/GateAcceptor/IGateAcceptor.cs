//////////////////////////////////////////////////////////////////////////
//
// 文件：server/GateAcceptor/IGateAcceptor.cs
// 作者：Xoen Xie
// 时间：2023/06/15
// 描述：网关接收器
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8618
using XSF;

namespace GateA
{
    public interface IGateHandler
    {
        void OnClientClose(uint nClientID);
    }

    public abstract class IGateAcceptor : FastNPManager
    {
        internal static GateAcceptor m_Instance;

        public static IGateAcceptor Instance { get { return m_Instance; } }

        // 断开指定客户端
        public abstract void DisconnectClient(uint nClientID, uint nReason);

        // 发送消息到网关
        public abstract void SendMessage2Gate(uint nGateID, IMessage message);

        // 发送消息到客户端
        public abstract void SendMessage2Client(uint nClientID, IMessage message);

        // 广播消息到所有客户端
        public abstract void Broadcast2AllClient(IMessage message);

        // 设置客户端转发内部的指定EP的服务器ID
        public abstract void SetServerID(uint nClientID, byte nEP, uint nServerID);

        // 开始广播消息到指定客户端
        public abstract void BeginBroadcast();

        // 添加广播消息的客户端
        public abstract void PushClientID(uint nClientID);

        // 广播消息
        public abstract void EndBroadcast(IMessage message);

        public static void CreateModule(int nID, IGateHandler handler)
        {
            var ga = new GateAcceptor();
            ga.m_Handler = handler;

            FastNPManagerInit init = new FastNPManagerInit();
            init.ID = nID;
            init.Name = "GateAcceptor";
            init.MaxSize = (int)XSFUtil.Config.MaxGate;

            XSFUtil.Server.AddModule(ga, init);

            m_Instance = ga;
        }
    }
}