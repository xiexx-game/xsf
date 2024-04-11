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

        public override int Port { get { return (int)XSFCore.Server.Ports[(int)EP.Gate]; } }

        protected override NetPoint NewNP()
        {
            return new Gate();
        }

        public override void DoRegist()
        {
            XSFCore.SetMessageExecutor((ushort)XsfPbid.SMSGID.GtGtAHandshake, new Executor_Gt_GtA_Handshake());
            XSFCore.SetMessageExecutor((ushort)XsfPbid.SMSGID.GtGtAHeartbeat, new Executor_Gt_GtA_Heartbeat());
            XSFCore.SetMessageExecutor((ushort)XsfPbid.SMSGID.GtGtAClientClose, new Executor_Gt_GtA_ClientClose());
        }


        // 断开指定客户端
        public override void DisconnectClient(uint nClientID, uint nReason)
        {
            var message = XSFCore.GetMessage((ushort)XsfPbid.SMSGID.GtAGtClientDisconnect) as XsfMsg.MSG_GtA_Gt_ClientDisconnect;
            message.mPB.ClientId = nClientID;
            message.mPB.Reason = nReason;
            var cid = ClientID.GetCID(nClientID);
            SendMessage2Gate(cid.Gate, message);
        }

        // 发送消息到网关
        public override void SendMessage2Gate(uint nGateID, IMessage message)
        {
            var np = Get(nGateID);
            if(np != null)
            {
                np.SendMessage(message);
            }
            else
            {
                Serilog.Log.Error("SendMessage2Gate gate not found, gate id=" + nGateID);
            }
        }

        // 发送消息到客户端
        public override void SendMessage2Client(uint nClientID, IMessage message)
        {
            var messageWrap = XSFCore.GetMessage((ushort)XsfPbid.SMSGID.GtAGtClientMessage) as XsfMsg.MSG_GtA_Gt_ClientMessage;
            messageWrap.mPB.ClientId.Clear();
            messageWrap.mPB.ClientId.Add(nClientID);

            var data = XSFCore.ClientPakcer.Pack(message);
            messageWrap.mPB.ClientMessage = Google.Protobuf.ByteString.CopyFrom(data);

            var cid = ClientID.GetCID(nClientID);
            SendMessage2Gate(cid.Gate, messageWrap);
        }

        // 广播消息到所有客户端
        public override void Broadcast2AllClient(IMessage message)
        {
            var data = XSFCore.ClientPakcer.Pack(message);

            var messageWrap = XSFCore.GetMessage((ushort)XsfPbid.SMSGID.GtAGtBroadcast) as XsfMsg.MSG_GtA_Gt_Broadcast;
            messageWrap.mPB.ClientMessage = Google.Protobuf.ByteString.CopyFrom(data);
            Broadcast(messageWrap, 0);
        }

        // 设置客户端转发内部的指定EP的服务器ID
        public override void SetServerID(uint nClientID, byte nEP, uint nServerID)
        {
            var message = XSFCore.GetMessage((ushort)XsfPbid.SMSGID.GtAGtSetServerId) as XsfMsg.MSG_GtA_Gt_SetServerId;
            message.mPB.ClientId = nClientID;
            message.mPB.Ep = nEP;
            message.mPB.ServerId = nServerID;

            var cid = ClientID.GetCID(nClientID);
            SendMessage2Gate(cid.Gate, message);
        }

        // 开始广播消息到指定客户端
        public override void BeginBroadcast()
        {
            for(int i = 0; i < m_NetPoints.Length; i ++)
            {
                Gate gate = m_NetPoints[i] as Gate;
                if(gate != null)
                {
                    gate.Clear();
                }
            }
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
            var data = XSFCore.ClientPakcer.Pack(message);
            var dataString = Google.Protobuf.ByteString.CopyFrom(data);

            for(int i = 0; i < m_NetPoints.Length; i ++)
            {
                Gate gate = m_NetPoints[i] as Gate;
                if(gate != null)
                {
                    gate.Boradcast(dataString);
                }
            }
        }

        public override void OnNPConnected(NetPoint np)
        {
            Serilog.Log.Information($"网关连入 {np.SID.ID}-{np.SID.Index}");
        }

        public override void OnNPLost(NetPoint np)
        {
            Serilog.Log.Information($"网关断开 {np.SID.ID}-{np.SID.Index}");
        }
    }
}