//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/Message/MessageModule.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：消息协议模块
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using XSF;
using Serilog;
using XsfPb;

namespace XsfMsg
{
    public partial class MessageModule : IModule, IMessageHelper
    {
        private IMessage[] m_MessagePool;

        public MessageModule()
        {
            m_MessagePool = new IMessage[10000];

            // 下面的注释定义必须保留，用来自动生成代码
            //MESSAGE_START
            m_MessagePool[(int)CMSGID.CltGtHandshake] = new MSG_Clt_Gt_Handshake();
            m_MessagePool[(int)CMSGID.GtCltHandshake] = new MSG_Gt_Clt_Handshake();
            m_MessagePool[(int)CMSGID.CltGtHeartbeat] = new MSG_Clt_Gt_Heartbeat();
            m_MessagePool[(int)CMSGID.GtCltHeartbeat] = new MSG_Gt_Clt_Heartbeat();
            m_MessagePool[(int)CMSGID.CltLLogin] = new MSG_Clt_L_Login();
            //MESSAGE_END
            // 上面的注释定义必须保留，用来自动生成代码

            XSFUtil.messageHelper = this;
        }

        public IMessage GetMessage(ushort nID)
        {
            return m_MessagePool[nID];
        }

        public static void CreateModule()
        {
            MessageModule module = new MessageModule();

            ModuleInit init = new ModuleInit();
            init.ID = 1;
            init.Name = "Message";
            init.NoWaitStart = true;

            XSFUtil.Server.AddModule(module, init);
        }
    }
}