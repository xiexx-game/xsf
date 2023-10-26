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

public class ActorManager : IModule
{
    private Dictionary<uint, Actor> m_Actors;
    private Dictionary<uint, Actor> m_ClientActors;

    private uint m_GlobalActorID;

    public ActorManager()
    {
        m_Actors = new Dictionary<uint, Actor>();
        m_ClientActors = new Dictionary<uint, Actor>();

        m_GlobalActorID = 1;
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

    public override void DoRegist()
    {
        XSFUtil.SetMessageExecutor((ushort)XsfPb.CMSGID.CltGLogin, new Executor_Clt_G_Login());
    }

    public static ActorManager Instance { get; private set; }

    public static void CreateModule()
    {
        Instance = new ActorManager();

        ModuleInit init = new ModuleInit();
        init.ID = (int)ModuleID.Actor;
        init.Name = "ActorManager";

        XSFUtil.Server.AddModule(Instance, init);
    }
}