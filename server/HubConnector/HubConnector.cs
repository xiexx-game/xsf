//////////////////////////////////////////////////////////////////////////
//
// 文件：server/HubConnector/HubConnector.cs
// 作者：Xoen Xie
// 时间：2023/06/15
// 描述：中转服连接器
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8602, CS8600, CS8618

using XSF;
using Google.Protobuf.Collections;

namespace HubC
{
    internal class HubConnector : IHubConnector
    {
        private XSFWriter m_Writer;

        public HubConnector()
        {
            m_Writer = new XSFWriter();
        }

        public override void DoRegist()
        {
            XSFCore.SetMessageExecutor((ushort)XsfPbid.SMSGID.HHcHandshake, new Executor_H_Hc_Handshake());
        }

        public override ModuleRunCode OnStartCheck()
        {
            if(IsHandshake)
            {
                return ModuleRunCode.OK;
            }

            return ModuleRunCode.Wait;
        }

        public override void SendHandshake()
        {
            Serilog.Log.Information("HubConnector SendHandshake");
            var message = XSFCore.GetMessage((ushort)XsfPbid.SMSGID.HcHHandshake) as XsfMsg.MSG_Hc_H_Handshake;
            message.mPB.ServerId = XSFCore.Server.ID;

            SendMessage(message);
        }

        public override void SendHeartbeat()
        {
            var message = XSFCore.GetMessage((ushort)XsfPbid.SMSGID.HcHHeartbeat);
            SendMessage(message);
        }

        public override void Send2Server(uint nServerID, IMessage message)
        {
            m_Writer.Clear();
            byte[] pbData = message.Export();
            //Serilog.Log.Information("pack pbData length=" + pbData.Length);
            uint pbLen = (uint)pbData.Length;
            // | 包长(uint 4字节) | 消息ID(ushort 2字节) | rawID(uint 4字节) | pb data |
            uint total = sizeof(ushort) + sizeof(uint) + pbLen;
            //Serilog.Log.Information("total = " + total);
            m_Writer.WriteUInt(total);
            m_Writer.WriteUShort(message.ID);
            m_Writer.WriteUInt(nServerID);
            m_Writer.WriteBuffer(pbData);

            byte[] totalData = new byte[m_Writer.Size];
            //Serilog.Log.Information("send total = " + totalData.Length);
            Array.Copy(m_Writer.Buffer, totalData, m_Writer.Size);

            SendData(message.ID, totalData);
        }
    }
}