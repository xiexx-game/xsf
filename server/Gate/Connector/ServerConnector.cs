//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/Gate/Connector/ServerConnector.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：消息处理
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8600, CS8602
using XSF;

public class ServerConnector : NetConnector
{
    private ConnectorManager m_Owner;

    public ServerConnector(ConnectorManager owner)
    {
        m_Owner = owner;
    }

    public override void SendHandshake()
    {
        var message = XSFUtil.GetMessage((ushort)XsfPb.SMSGID.GtGtAHandshake) as XsfMsg.MSG_Gt_GtA_Handshake;
        message.mPB.ServerId = XSFUtil.Server.ID;
        SendMessage(message);
    }

    public override void SendHeartbeat()
    {
        var message = XSFUtil.GetMessage((ushort)XsfPb.SMSGID.GtGtAHeartbeat);
        SendMessage(message);
    }

    public override void OnNetError()
    {
        m_Owner.DeleteConnector(m_nID);
    }
}