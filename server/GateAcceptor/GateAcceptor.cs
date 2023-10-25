//////////////////////////////////////////////////////////////////////////
//
// 文件：server/GateAcceptor/GateAcceptor.cs
// 作者：Xoen Xie
// 时间：2023/06/15
// 描述：网关接收器
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8602, CS8600, CS8618

using XSF;
using Google.Protobuf.Collections;

namespace GateA
{
    internal class GateAcceptor : IGateAcceptor
    {
        internal IGateHandler m_Handler;

        public override int Port { get { return (int)XSFUtil.Server.Ports[(int)EP.Gate]; } }

        protected override NetPoint NewNP()
        {
            return new Gate();
        }


        // 断开指定客户端
        public override void DisconnectClient(uint nClientID, uint nResult)
        {

        }

        // 发送消息到网关
        public override void SendMessage2Gate(uint nGateID, IMessage message)
        {
            
        }

        // 发送消息到客户端
        public override void SendMessage2Client(uint nClientID, IMessage message)
        {
            
        }

        // 广播消息到所有客户端
        public override void Broadcast2AllClient(IMessage message)
        {
            
        }

        // 设置客户端转发内部的指定EP的服务器ID
        public override void SetServerID(uint nClientID, byte nEP, uint nServerID)
        {
            
        }

        // 开始广播消息到指定客户端
        public override void BeginBroadcast()
        {
            
        }

        // 添加广播消息的客户端
        public override void PushClientID(uint nClientID)
        {
            var cid = ClientID.GetCID(nClientID);
            Gate gate = Get(cid.Gate) as Gate;
            gate.Add(nClientID);
        }

        // 广播消息
        public override void EndBroadcast(IMessage message)
        {
            for(int i = 0; i < m_NetPoints.Length; i ++)
            {
                Gate gate = m_NetPoints[i] as Gate;
                if(gate != null)
                {
                    gate.Boradcast(message);
                }
            }
        }
    }
}