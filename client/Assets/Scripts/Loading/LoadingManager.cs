//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Scripts/Loading/LoadingManager.cs
// 作者：Xoen
// 时间：2023/08/25
// 描述：加载管理
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections.Generic;

public class LoadingManager : Singleton<LoadingManager>, IUpdateNode, IEventSink
{
    private List<LoadingBase> m_LoadingList;
    private int m_LoadingIndex;
    private bool m_bNeedLoading;

    enum LoadingStatus
    {
        None = 0,
        Load,
        Wait,
    }

    private LoadingStatus m_nStatus;
    private float m_SingleProgress;
    private float m_ProgressExt;

    private UIBase m_UI;

    public void AddLoading(LoadingBase loading)
    {
        if (m_LoadingList == null)
            m_LoadingList = new List<LoadingBase>();

        m_LoadingList.Add(loading);
    }

    public void Start()
    {
        XSFUpdate.Instance.Add(this);
        m_UI = XSFUI.Instance.Get((int)UIID.UILoading);
        m_UI.Show();
        XSFEvent.Instance.Subscribe(this, XSF.UI_SHOW_EVENT_ID, (uint)EventObjectID.UILoadingShow);
    }

    private void End()
    {
        m_LoadingList.Clear();
        m_bNeedLoading = false;
    }

    private void InnerStart()
    {
        m_LoadingIndex = 0;
        m_bNeedLoading = true;
        m_nStatus = LoadingStatus.Load;
        XSFUpdate.Instance.Add(this);

        m_SingleProgress = 1.0f / m_LoadingList.Count;

        m_ProgressExt = 1.001f - (m_SingleProgress * m_LoadingList.Count); 
    }

    public void OnUpdate()
    {
        switch (m_nStatus)
        {
        case LoadingStatus.Wait:
            //Debug.LogError($"LoadingStatus.Load, m_LoadingIndex={m_LoadingIndex}, m_LoadingList[m_LoadingIndex].IsDone={m_LoadingList[m_LoadingIndex].IsDone}");
            if(m_LoadingList[m_LoadingIndex].IsDone)
            {
                //Debug.LogError($"LoadingStatus.Load, m_LoadingIndex={m_LoadingIndex}, m_LoadingList[m_LoadingIndex].IsDone={m_LoadingList[m_LoadingIndex].IsDone}");
                m_LoadingList[m_LoadingIndex].End();
                m_LoadingIndex ++;
                m_UI.Refresh((uint)UIRefreshID.AddProgress, m_SingleProgress);
                if(m_LoadingIndex >= m_LoadingList.Count)
                {
                    m_UI.Refresh((uint)UIRefreshID.AddProgress, m_ProgressExt);
                    m_nStatus = LoadingStatus.None;
                    End();
                }
                else
                {
                    m_nStatus = LoadingStatus.Load;
                }
            }
            break;

        case LoadingStatus.Load:
            m_LoadingList[m_LoadingIndex].Start();
            m_nStatus = LoadingStatus.Wait;
            //Debug.LogError($"LoadingStatus.Load, m_LoadingIndex={m_LoadingIndex}");
            break;
        }
    }

    public bool IsUpdateWroking { get { return m_bNeedLoading; } }

    public void OnFixedUpdate() { }

    public bool OnEvent(uint nEventID, uint nObjectID, object context)
    {
        InnerStart();
        return false;
    }

}