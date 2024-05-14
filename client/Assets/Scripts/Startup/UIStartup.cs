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
using XSF;
using TMPro;
using System.Collections.Generic;

public sealed class UIStartup
{
    private GameObject m_UIRoot;
    private Transform m_UIRootT;

    private Slider m_Progress;

    private float m_fTotal;
    private float m_fCurProgress;

    private bool m_bUpdate;

    private UIMessageBox MessageBox;

    private TextMeshProUGUI m_Info;

    private int m_nBGIndex;
    private List<GameObject> m_Backgrounds;

    public void Init()
    {
        m_UIRoot = XSFMain.Instance.UIStartup;
        m_UIRootT = m_UIRoot.transform;

        m_Progress = m_UIRootT.Find("progress").GetComponent<Slider>();
        m_Progress.value = 0;
        m_fTotal = 0;
        m_fCurProgress = 0;
        m_bUpdate = false;

        m_Info = m_UIRootT.Find("Info").GetComponent<TextMeshProUGUI>();

        MessageBox = new UIMessageBox();
        MessageBox.Init(m_UIRootT.Find("MessageBox"));

        m_Backgrounds = new List<GameObject>();
        m_Backgrounds.Add(m_UIRootT.Find("background1").gameObject);
        m_Backgrounds.Add(m_UIRootT.Find("background2").gameObject);
    }

    public void SetInfoText(string text)
    {
        m_Info.text = text;
    }

    public void ShowMessageBox(IMessageBoxHandler handler)
    {
        MessageBox.Show(handler);
    }

    public void SetProgress(float value)
    {
        m_fTotal = value;
        m_bUpdate = true;
    }

    public void ShowNextBg()
    {
        m_Backgrounds[m_nBGIndex].SetActive(false);
        m_nBGIndex++;
        m_Backgrounds[m_nBGIndex].SetActive(true);
    }

    public void ResetProgress()
    {
        m_fTotal = 0.0f;
        m_fCurProgress = 0.0f;
        m_Progress.value = 0.0f;
    }

    public void Update()
    {
        if(!m_bUpdate)
        {
            return;
        }

        m_fCurProgress += 10 * Time.deltaTime;

        //Debug.Log($"m_fCurProgress={m_fCurProgress}, m_fTotal={m_fTotal}");

        if(m_fCurProgress >= m_fTotal)
        {
            //Debug.Log("m_fCurProgress >= m_fTotal");
            m_Progress.value = m_fTotal;
            m_bUpdate = false;

            if(m_fTotal > 1.0f)
            {
                XSFEvent.Instance.Fire((uint)EventID.StartupStepDone);
            } 
        }
        else
        {
            m_Progress.value = m_fCurProgress;
        }
    }

    public void Destory()
    {
        GameObject.Destroy(m_UIRoot);
        m_UIRoot = null;
        m_UIRootT = null;
        m_Progress = null;
    }
}