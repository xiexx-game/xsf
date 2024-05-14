//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\Scripts\Startup\XSFStartup.cs
// 作者：Xoen Xie
// 时间：2023/06/24
// 描述：游戏启动流程
// 说明：
//
//////////////////////////////////////////////////////////////////////////


using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;
using XSF;

public sealed class XSFStartup : Singleton<XSFStartup>, IXSFModule, IUpdateNode, IEventSink
{
    struct StepData
    {
        public StartupStep Step;
        public float fProgress;
    }

    class StepInfo
    {
        enum RunStatus
        {
            None = 0,
            Start,
            Check,
            Wait,
        }

        private RunStatus m_nCurStatus;

        public int m_CurrentIndex;

        List<StepData> m_Steps;

        public StepInfo()
        {
            m_Steps = new List<StepData>();
            m_nCurStatus = RunStatus.Start;
            m_CurrentIndex = 0;
        }

        public void Update()
        {
            switch(m_nCurStatus)
            {
            case RunStatus.Start:
                m_Steps[m_CurrentIndex].Step.Start();
                m_nCurStatus = RunStatus.Check;
                break;    

            case RunStatus.Check:
                m_Steps[m_CurrentIndex].Step.Update();

                if(m_Steps[m_CurrentIndex].Step.IsDone)
                {
                    m_Steps[m_CurrentIndex].Step.End();

                    m_CurrentIndex ++;
                    if(m_CurrentIndex >= m_Steps.Count)
                    {
                        m_nCurStatus = RunStatus.Wait;
                    }
                    else
                    {
                        m_nCurStatus = RunStatus.Start;
                    }
                }

                Instance.UI.SetProgress(Progress);
                break;

            case RunStatus.Wait:
                break;

            default:
                return;
            }

            Instance.UI.Update();
        }

        public void AddStep(StartupStep s, float progress)
        {
            StepData sd;
            sd.Step = s;
            sd.fProgress = progress;
            m_Steps.Add(sd);
        }

        public float Progress  
        {
            get
            {
                if(m_CurrentIndex >= m_Steps.Count)
                {
                    return 1.01f;
                }

                float progress = 0.0f;
                for(int i = 0; i < m_Steps.Count; i++)
                {
                    if(i < m_CurrentIndex)
                    {
                        progress += m_Steps[i].fProgress;
                    }
                    else
                    {
                        progress += m_Steps[i].Step.CurrentProgress * m_Steps[i].fProgress;
                    }
                }

                return progress;
            }
        }
    }

    int m_nCurrentStep;
    List<StepInfo> m_AllSteps;


    public UIStartup UI { get; private set; }
    private bool m_bWorking;

    public void Init()
    {
        UI = new UIStartup();
        UI.Init();

        InitSteps();

        m_bWorking = true;
        XSFUpdate.Instance.Add(this);

        XSFEvent.Instance.Subscribe(this, (uint)EventID.StartupStepDone, 0);
    }

    // 在所有模块Init后调用
    public void Start() 
    {

    }

    // 释放操作
    public void Release() {}

    // 热更完成后调用
    public void OnContentUpdateDone() {}

    // 登录完成后调用
    public void OnLogin() {}

    // 登出时调用
    public void OnLogout() {}

    private void InitSteps()
    {
        m_AllSteps = new List<StepInfo>();
        StepInfo si = new StepInfo();
        si.AddStep(new StartupStepYooAssetInit(), 0.2f);
        si.AddStep(new StartupStepYooAssetUpdate(), 0.79f);
        m_AllSteps.Add(si);

        si = new StepInfo();
        si.AddStep(new StartupStepSchema(), 0.79f);
        si.AddStep(new StartupStepShowFirstUI(), 0.2f);
        m_AllSteps.Add(si);
    }

    public void OnAllStepDone()
    {
        m_bWorking = false;
        Debug.Log("XSFStartup OnAllStepDone");
        XSFEvent.Instance.Fire((uint)EventID.AllStartupStepDone);
    }

    public bool OnEvent( uint nEventID, uint nObjectID, object context )
    {
        if(nEventID == (uint)EventID.StartupStepDone)
        {
            m_nCurrentStep ++;
            if(m_nCurrentStep >= m_AllSteps.Count)
            {
                UI.Destory();
                UI = null;
                OnAllStepDone();
                return false;
            }
            else
            {
                UI.ShowNextBg();
                UI.ResetProgress();
            }
        }

        return true;
    }

    public bool IsUpdateWroking { get { return m_bWorking; } }

    public void OnUpdate()
    {
        m_AllSteps[m_nCurrentStep].Update();
    }

    public void OnFixedUpdate() 
    {

    }
}