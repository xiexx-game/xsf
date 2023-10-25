//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\Scripts\Net\Messages\MSG_L_Clt_LoginResult.cs
// 作者：Xoen Xie
// 时间：10/25/2023
// 描述：login --> client 登录结果
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using XsfPb;

public sealed class MSG_L_Clt_LoginResult : IMessage
{
    public override CMSGID ID { get { return CMSGID.LCltLoginResult; } }

    private L_Clt_LoginResult m_PB;
    public L_Clt_LoginResult mPB
    {
        get
        {
            if (m_PB == null)
                m_PB = new L_Clt_LoginResult();

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
        m_PB = L_Clt_LoginResult.Parser.ParseFrom(data, offset, length);
    }

    // 消息处理接口
    public override void Execute(NetClient client)
    {
        


    }
}