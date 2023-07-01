//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\Scripts\Net\Messages\MSG_Clt_L_Login.cs
// 作者：xiexx
// 时间：2023/7/1
// 描述： client --> login 登录
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using XsfPb;

public sealed class MSG_Clt_L_Login : IMessage
{
    public override CMSGID ID { get { return CMSGID.CltLLogin; } }

    private Clt_L_Login m_PB;
    public Clt_L_Login mPB
    {
        get
        {
            if (m_PB == null)
                m_PB = new Clt_L_Login();

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
        m_PB = Clt_L_Login.Parser.ParseFrom(data, offset, length);
    }

    // 消息处理接口
    public override void Execute(NetClient client)
    {
        


    }
}