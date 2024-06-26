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
using XsfPbid;

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
			m_MessagePool[(int)CMSGID.GtCltDisconnect] = new MSG_Gt_Clt_Disconnect();
			m_MessagePool[(int)CMSGID.CltGLogin] = new MSG_Clt_G_Login();
			m_MessagePool[(int)CMSGID.GCltLoginResult] = new MSG_G_Clt_LoginResult();
			m_MessagePool[(int)CMSGID.GCltTestData] = new MSG_G_Clt_TestData();
			m_MessagePool[(int)SMSGID.CcCHandshake] = new MSG_Cc_C_Handshake();
			m_MessagePool[(int)SMSGID.CCcHandshake] = new MSG_C_Cc_Handshake();
			m_MessagePool[(int)SMSGID.CcCHeartbeat] = new MSG_Cc_C_Heartbeat();
			m_MessagePool[(int)SMSGID.CcCServerInfo] = new MSG_Cc_C_ServerInfo();
			m_MessagePool[(int)SMSGID.CCcServerInfo] = new MSG_C_Cc_ServerInfo();
			m_MessagePool[(int)SMSGID.CcCServerLost] = new MSG_Cc_C_ServerLost();
			m_MessagePool[(int)SMSGID.CCcServerLost] = new MSG_C_Cc_ServerLost();
			m_MessagePool[(int)SMSGID.CcCServerOk] = new MSG_Cc_C_ServerOk();
			m_MessagePool[(int)SMSGID.CCcServerOk] = new MSG_C_Cc_ServerOk();
			m_MessagePool[(int)SMSGID.CCcStop] = new MSG_C_Cc_Stop();
			m_MessagePool[(int)SMSGID.GtGtAHandshake] = new MSG_Gt_GtA_Handshake();
			m_MessagePool[(int)SMSGID.GtAGtHandshake] = new MSG_GtA_Gt_Handshake();
			m_MessagePool[(int)SMSGID.GtGtAHeartbeat] = new MSG_Gt_GtA_Heartbeat();
			m_MessagePool[(int)SMSGID.GtGtAClientClose] = new MSG_Gt_GtA_ClientClose();
			m_MessagePool[(int)SMSGID.GtAGtClientDisconnect] = new MSG_GtA_Gt_ClientDisconnect();
			m_MessagePool[(int)SMSGID.GtAGtClientMessage] = new MSG_GtA_Gt_ClientMessage();
			m_MessagePool[(int)SMSGID.GtAGtBroadcast] = new MSG_GtA_Gt_Broadcast();
			m_MessagePool[(int)SMSGID.GtAGtSetServerId] = new MSG_GtA_Gt_SetServerId();
			m_MessagePool[(int)SMSGID.DbcDbHandshake] = new MSG_Dbc_Db_Handshake();
			m_MessagePool[(int)SMSGID.DbDbcHandshake] = new MSG_Db_Dbc_Handshake();
			m_MessagePool[(int)SMSGID.DbcDbHeartbeat] = new MSG_Dbc_Db_Heartbeat();
			m_MessagePool[(int)SMSGID.DbcDbRequest] = new MSG_Dbc_Db_Request();
			m_MessagePool[(int)SMSGID.DbDbcResponse] = new MSG_Db_Dbc_Response();
			m_MessagePool[(int)SMSGID.HcHHandshake] = new MSG_Hc_H_Handshake();
			m_MessagePool[(int)SMSGID.HHcHandshake] = new MSG_H_Hc_Handshake();
			m_MessagePool[(int)SMSGID.HcHHeartbeat] = new MSG_Hc_H_Heartbeat();
			m_MessagePool[(int)SMSGID.GGHubTest] = new MSG_G_G_HubTest();
            //MESSAGE_END
            // 上面的注释定义必须保留，用来自动生成代码

            XSFCore.messageHelper = this;
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

            XSFCore.Server.AddModule(module, init);
        }
    }
}
