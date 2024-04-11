//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/Game/Actor/ActorManager.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：角色管理
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8602, CS8600, CS8618

using XSF;
using DBC;
using DBUtil;

public class ActorManager : IModule, ITimerHandler, IDBHandler
{
    private Dictionary<uint, Actor> m_Actors;
    private Dictionary<uint, Actor> m_ClientActors;

    private uint m_GlobalActorID;

    private TimersManager m_Timer;

    public ActorManager()
    {
        m_Actors = new Dictionary<uint, Actor>();
        m_ClientActors = new Dictionary<uint, Actor>();

        m_GlobalActorID = 1;

        m_Timer = new TimersManager(2);
    }

    public Actor DoLogin(uint nClientID)
    {
        Actor a = null;
        if(m_ClientActors.TryGetValue(nClientID, out a))
        {
            return a;
        }

        a = new Actor(m_GlobalActorID++, nClientID);

        m_Actors.Add(a.ID, a);
        m_ClientActors.Add(a.ClientID, a);

        return a;
    }

    public void OnActorLogout(uint nClientID)
    {
        Actor a = null;
        if(m_ClientActors.TryGetValue(nClientID, out a))
        {
            m_ClientActors.Remove(nClientID);

            m_Actors.Remove(a.ID);

            Serilog.Log.Information("Actor Logout id={0}, client id={1}", a.ID, a.ClientID);
        }
        else
        {
            Serilog.Log.Error("Actor logout not exist, client id={0}", nClientID);
        }
    }

    private uint m_nGetStart = 0;
    private readonly uint m_nGetCount = 50;

    private uint m_nTotal = 0;

    public override void OnOK()
    {
        if(XSFCore.Server.SID.Index == 1)
        {
            var sql = DBUtil.DBSqlBase.GetAllAccounts;
            sql.SetData(m_nGetStart, m_nGetCount);

            DBC.IDBConnector.Get((int)ModuleID.DBConnector).SendRequest(this, sql, 0, 0);

            
        }
    }

    public override void OnStartClose()
    {
        m_Timer.CloseAllTimer();
    }

    public void OnMysqlResult(DBSqlBase sql, uint nCode, uint nKeyID, uint nTotal, byte[] data)
    {
        if(nCode != (uint)XsfPb.OpResult.Ok)
        {
            Serilog.Log.Error($"ActorManager OnMysqlResult error, code={nCode}, sql={sql.ID}");
            return;
        }

        switch((DBSqlID)sql.ID)
        {
        case DBSqlID.GetAllAccounts:
            {
                if(nTotal > 0)
                {
                    // XSFReader reader = new XSFReader(data);
                    // var localSql = sql as DBSqlGetAllAccounts;
                    // for(uint i = 0; i < nTotal; i ++)
                    // {
                    //      AccountData ad = localSql.Read(reader);
                    //      Serilog.Log.Information($"ActorManager OnMysqlResult read data, email={ad.email}, pwd={ad.pwd}, actor={ad.actorID}");
                    // }

                    m_nTotal += nTotal;
                }

                if(nTotal >= m_nGetCount)
                {
                    m_nGetStart += nTotal;
                    var nextSql = DBUtil.DBSqlBase.GetAllAccounts;
                    nextSql.SetData(m_nGetStart, m_nGetCount);

                    DBC.IDBConnector.Get((int)ModuleID.DBConnector).SendRequest(this, nextSql, 0, 0);
                }
                else
                {
                    Serilog.Log.Information("all data get done ...m_nTotal=" + m_nTotal);
                    m_Timer.StartTimer(1, this, 100, 1, "DBTestTimer");
                }
            }
            break;

        case DBSqlID.NewAccount:
            {
                Serilog.Log.Information("NewAccount down, nKeyID=" + nKeyID);
            }
            break;

        case DBSqlID.SetAccountActorID:
            {
                Serilog.Log.Information("SetAccountActorID down, nKeyID=" + nKeyID);
            }
            break;
        }
    }

    private uint emailID = 100;

    public void OnTimer(byte nTimerID, bool bLastCall)
    {
        switch(nTimerID)
        {
        case 0:
            {
                if(ServerInfoHandler.TargetServerID > 0)
                {
                    var message = XSFCore.GetMessage((ushort)XsfPbid.SMSGID.GGHubTest) as XsfMsg.MSG_G_G_HubTest;
                    message.mPB.Code = 1;
                    HubC.IHubConnector.Get((int)ModuleID.HubConnector).Send2Server(ServerInfoHandler.TargetServerID, message);
                    Serilog.Log.Information("发送GGHubTest， Code 1, local id=" + XSFCore.Server.ID + ", target=" + ServerInfoHandler.TargetServerID);

                    m_Timer.DelTimer(0);
                }
            }
            break;

        case 1:
            {

                /*
                // 新建账号
                for(int i = 0; i < 1000; i ++)
                {
                    var newSql = DBSqlBase.NewAccount;
                    newSql.SetData($"add-{emailID}@test.com", "123456", emailID);
                    DBC.IDBConnector.Get((int)ModuleID.DBConnector).SendRequest(this, newSql, emailID, emailID);

                    emailID ++;
                }
                */

                // 更新账号
                for(int i = 0; i < 50; i ++)
                {
                    var newSql = DBSqlBase.SetAccountActorID;
                    newSql.SetData(emailID + 900000000,  $"add-{emailID}@test.com");
                    DBC.IDBConnector.Get((int)ModuleID.DBConnector).SendRequest(this, newSql, emailID, emailID);

                    emailID ++;
                }

                m_Timer.StartTimer(0, this, 100, -1, "GGHubTest");
            }
            break;
        }    
    }

    public override void DoRegist()
    {
        XSFCore.SetMessageExecutor((ushort)XsfPbid.CMSGID.CltGLogin, new Executor_Clt_G_Login());
        XSFCore.SetMessageExecutor((ushort)XsfPbid.SMSGID.GGHubTest, new Executor_G_G_HubTest());
    }

    public static ActorManager Instance { get; private set; }

    public static void CreateModule()
    {
        Instance = new ActorManager();

        ModuleInit init = new ModuleInit();
        init.ID = (int)ModuleID.Actor;
        init.Name = "ActorManager";

        XSFCore.Server.AddModule(Instance, init);
    }
}