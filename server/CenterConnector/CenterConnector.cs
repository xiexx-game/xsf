//////////////////////////////////////////////////////////////////////////
//
// 文件：server/CenterConnector/CenterConnector.cs
// 作者：Xoen Xie
// 时间：2023/06/15
// 描述：中心服连接器
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8602

using XSF;
using Google.Protobuf.Collections;

namespace CC
{
    public class CenterConnector : ICenterConnector
    {
        public override bool Start()
        {
            var config = XSFUtil.Config;

            return Connect(config.MainCenterIP, (int)config.CenterPort);
        }

        public override void SendHandshake()
        {
            Serilog.Log.Information("CenterConnector SendHandshake");
            var message = XSFUtil.GetMessage((ushort)XsfPb.SMSGID.CcCHandshake) as XsfMsg.MSG_Cc_C_Handshake;
            message.mPB.ServerId = XSFUtil.Server.ID;

            message.mPB.Ports.Clear();
            for(int i = 0; i < XSFUtil.Server.Ports.Length; i ++)
            {
                message.mPB.Ports.Add(XSFUtil.Server.Ports[i]);
            }

            SendMessage(message);
        }

        public override void SendHeartbeat()
        {
            Serilog.Log.Information("CenterConnector SendHeartbeat");
            var message = XSFUtil.GetMessage((ushort)XsfPb.SMSGID.CcCHeartbeat);
            SendMessage(message);
        }

        
    }
}