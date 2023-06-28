//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\Scripts\Startup\XSFStartup.cs
// 作者：Xoen Xie
// 时间：2023/06/24
// 描述：游戏启动流程
// 说明：
//
//////////////////////////////////////////////////////////////////////////

public sealed class XSFStartup : Singleton<XSFStartup>, IUpdateNode
{
    public UIStartup UI { get; private set; }
    private bool m_bWorking;

    private StartupStep[] m_Steps;

    private float m_fProgressValue;
    private float m_fOffset;

    private StartupStepID m_CurStepID;

    enum RunStatus
    {
        None = 0,
        Start,
        Check,
    }

    private RunStatus m_nCurStatus;

    public void Init()
    {
        UI = new UIStartup();
        UI.Init();

        InitSteps();

        m_bWorking = true;
        XSFUpdate.Instance.Add(this);
    }

    private void InitSteps()
    {
        m_Steps = new StartupStep[(int)StartupStepID.Max];

        int nTotalCount = (int)StartupStepID.Max - 1;
        m_fProgressValue = 1.0f/(float)nTotalCount;
        m_fOffset = 0.01f;
        float total = m_fProgressValue * (float)nTotalCount;
        if(total < 1.0f) 
        {
            m_fOffset = 1.0f - total + 0.01f;
        }

        m_Steps[(int)StartupStepID.AASUpdate] = new StartupStepAASUpdate();
        m_Steps[(int)StartupStepID.Schema] = new StartupStepSchema();

        m_CurStepID = StartupStepID.AASUpdate;
        m_nCurStatus = RunStatus.Start;
    }

    public void OnAllStepDone()
    {
        m_bWorking = false;
        XSF.Log("XSFStartup OnAllStepDone");
        XSFEvent.Instance.Fire((uint)EventID.AllStartupStepDone);
    }


    public bool IsUpdateWroking { get { return m_bWorking; } }

    public void OnUpdate()
    {
        switch(m_nCurStatus)
        {
        case RunStatus.Start:
            m_Steps[(int)m_CurStepID].Start();
            m_nCurStatus = RunStatus.Check;
            break;    

        case RunStatus.Check:
            if(m_Steps[(int)m_CurStepID].IsDone)
            {
                m_Steps[(int)m_CurStepID].End();

                UI.SetProgress(m_fProgressValue);
                m_CurStepID ++;
                if(m_CurStepID >= StartupStepID.Max)
                {
                    UI.SetProgress(m_fOffset);
                    m_nCurStatus = RunStatus.None;
                }
                else
                {
                    m_nCurStatus = RunStatus.Start;
                }
            }
            break;
        }
        UI.Update();
    }

    public void OnFixedUpdate() 
    {

    }
}