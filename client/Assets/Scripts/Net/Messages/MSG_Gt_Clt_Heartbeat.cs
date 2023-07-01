//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\Scripts\Net\Messages\MSG_Gt_Clt_Heartbeat.cs
// 作者：Xoen Xie
// 时间：2023/7/1
// 描述： gate --> client 心跳 反馈
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using XsfPb;

public sealed class MSG_Gt_Clt_Heartbeat : IMessage
{
    public override CMSGID ID { get { return CMSGID.GtCltHeartbeat; } }

    private Gt_Clt_Heartbeat m_PB;
    public Gt_Clt_Heartbeat mPB
    {
        get
        {
            if (m_PB == null)
                m_PB = new Gt_Clt_Heartbeat();

            return m_PB;
        }
    }

    public override void Export(XSFWriter writer)
    {
        // 写入PB数据
        byte[] bytes = new byte[mPB.CalculateSize()];
        mPB.WriteTo(new Google.Protobuf.CodedOutputStream(bytes));
        writer.WriteBuffer(bytes);
    }

    public override void Import(byte[] data, int offset, int length)
    {
        m_PB = Gt_Clt_Heartbeat.Parser.ParseFrom(data, offset, length);
    }

    // 消息处理接口
    public override void Execute(NetClient client)
    {
        XSFNet.Instance.SetTime(m_PB.ClientTime, m_PB.ServerTime);
    }
}