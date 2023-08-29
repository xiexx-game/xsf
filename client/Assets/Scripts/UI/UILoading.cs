//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\Scripts\UI\UILoading.cs
// 作者：Xoen Xie
// 时间：8/24/2023
// 描述：
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;

public sealed class UILoading : UIBase
{
// UI_PROP_START
	public Slider Progress { get; private set; }	// 
// UI_PROP_END

    public override string Name { get { return "UILoading"; } }

    public override string SortingLayerName { get { return "UILoading";} }

    public override uint EventObjID { get { return (uint)EventObjectID.UILoadingShow; } }

    private float m_fTargetProgress;

    enum ProgressStatus
    {
        None = 0,
        Add,
    }

    private ProgressStatus m_nStatus;

    public override void OnInit()
    {
        // UI_INIT_START
		// 
		Progress = RootT.Find("Progress").GetComponent<Slider>();
        // UI_INIT_END
    }

    public override void OnShow()
    {
        Progress.value = 0;
    }

    public override void OnRefresh(uint nFreshID, object data)
    {
        if(nFreshID == (uint)UIRefreshID.AddProgress)
        {
            m_fTargetProgress += (float)data;
            m_nStatus = ProgressStatus.Add;
        }
    }

    public override void OnUpdate()
    {
        switch (m_nStatus)
        {
        case ProgressStatus.Add:
            {
                float current = Progress.value + Time.deltaTime;
                //Debug.LogError($"current={current}, m_fTargetProgress={m_fTargetProgress}");
                if(current >= m_fTargetProgress)
                {
                    Progress.value = m_fTargetProgress;
                    m_nStatus = ProgressStatus.None;
                    //Debug.LogError($"1111 current={current}, m_fTargetProgress={m_fTargetProgress}");
                    if(current >= 1.0f)
                    {
                        Close();
                        //Debug.LogError($"2222 current={current}, m_fTargetProgress={m_fTargetProgress}");
                        XSFEvent.Instance.Fire((uint)EventID.LoadingDone);
                    }
                }
                else 
                {
                    Progress.value = current;
                }
            }
            break;
        }
    }


    // UI_FUNC_APPEND
}
