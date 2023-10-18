//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/Center/TestModule.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：测试模块
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using XSF;

public class TestModule : IModule, ITimerHandler
{
    int m_nCount = 0;

    ulong m_nTimer1;
    ulong m_nTimer2;
    ulong m_nTimer3;

    public override bool Start()
    {
        Serilog.Log.Information("TestModule Start");
        m_nTimer1 = XSFUtil.AddTimer(0, this, 100, -1, "Test1");

        return true;
    }

    public override void OnClose()
    {
        XSFUtil.DelTimer(m_nTimer1);
        XSFUtil.DelTimer(m_nTimer2);
        XSFUtil.DelTimer(m_nTimer3);
    }

    public void OnTimer(byte nTimerID, bool bLastCall)
    {
        switch(nTimerID)
        {
        case 0:
            Serilog.Log.Information("TestModule OnTimer timer 0");
            m_nCount ++;
            if(m_nCount == 5)
            {
                m_nTimer2 = XSFUtil.AddTimer(1, this, 1000, 5, "Test2");
            }
            else if(m_nCount == 20)
            {
                m_nTimer3 = XSFUtil.AddTimer(2, this, 100, 1, "Test3");
            }
            break;

        case 1:
            Serilog.Log.Information("TestModule OnTimer timer 1");
            break;

        case 2:
            XSFUtil.DelTimer(m_nTimer1);
            XSFUtil.DelTimer(m_nTimer2);
            break;
        }
        
    }

    public static void CreateModule()
    {
        TestModule module = new TestModule();

        ModuleInit init = new ModuleInit();
        init.ID = (int)ModuleID.Test;
        init.Name = "TestModule";
        init.NoWaitStart = true;

        XSFUtil.Server.AddModule(module, init);
    }
}