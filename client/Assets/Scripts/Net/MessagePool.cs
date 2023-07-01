//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\Scripts\Net\MessagePool.cs
// 作者：Xiexx
// 时间：2023/03/23
// 描述：消息池
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using XsfPb;

public sealed class MessagePool : Singleton<MessagePool>
{
    private IMessage[] m_MessagePool;

    public MessagePool()
    {
        m_MessagePool = new IMessage[(int)CMSGID.Max];
    }

    public bool Init()
    {
        // 下面的注释定义必须保留，用来自动生成代码
        //MESSAGE_START
		m_MessagePool[(int)CMSGID.CltGtHandshake] = new MSG_Clt_Gt_Handshake();
		m_MessagePool[(int)CMSGID.GtCltHandshake] = new MSG_Gt_Clt_Handshake();
		m_MessagePool[(int)CMSGID.CltGtHeartbeat] = new MSG_Clt_Gt_Heartbeat();
		m_MessagePool[(int)CMSGID.GtCltHeartbeat] = new MSG_Gt_Clt_Heartbeat();
		m_MessagePool[(int)CMSGID.CltLLogin] = new MSG_Clt_L_Login();
        //MESSAGE_END
        // 上面的注释定义必须保留，用来自动生成代码
        return true;
    }

    public IMessage Get(CMSGID nID)
    {
        if(nID >= CMSGID.Max) {
            return null;
        }

        return m_MessagePool[(int)nID];
    }
}
