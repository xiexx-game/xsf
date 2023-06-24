//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\Scripts\Startup\UIStartup.cs
// 作者：Xoen Xie
// 时间：2023/06/24
// 描述：游戏启动UI
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;

public sealed class UIStartup
{
    private GameObject m_UIRoot;
    private Transform m_UIRootT;

    private Slider m_Progress;

    private float m_fTotal;
    private float m_fCurProgress;

    private bool m_bUpdate;

    public void Init()
    {
        m_UIRoot = XSFMain.Instance.UIStartup;
        m_UIRootT = m_UIRoot.transform;

        m_Progress = m_UIRootT.Find("progress").GetComponent<Slider>();
        m_Progress.value = 0;
        m_fTotal = 0;
        m_fCurProgress = 0;
        m_bUpdate = false;
    }

    public void SetProgress(float value)
    {
        m_fTotal += value;
        m_bUpdate = true;
    }

    public void Update()
    {
        if(!m_bUpdate)
        {
            return;
        }

        m_fCurProgress += Time.deltaTime;

        if(m_fCurProgress >= m_fTotal)
        {
            m_Progress.value = m_fTotal;
            m_bUpdate = false;
            if(m_fTotal > 1.0f)
            {
                GameObject.Destroy(m_UIRoot);
                m_UIRoot = null;
                m_UIRootT = null;
                m_Progress = null;

                XSFStartup.Instance.OnAllStepDone();
            } 
        }
        else
        {
            m_Progress.value = m_fCurProgress;
        }
    }
}