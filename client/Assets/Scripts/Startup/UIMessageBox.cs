//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets/Scripts/Startup/UIMessageBox.cs
// 作者：Xoen Xie
// 时间：2023/06/24
// 描述：弹窗
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;
using XSF;
using TMPro;

public interface IMessageBoxHandler
{
    void OnOK();

    void OnCancel();

    string InfoText { get; }
}

public class UIMessageBox
{
    GameObject m_RootObj;

    TextMeshProUGUI m_Info;

    private IMessageBoxHandler m_Handler;

    public void Init(Transform t)
    {
        m_RootObj = t.gameObject;

        m_Info = t.Find("Info").GetComponent<TextMeshProUGUI>();

        var BtnOK = t.Find("BtnOK").gameObject;
		UIEventClick.Set(BtnOK, OnBtnOKClick);

        var BtnCancel = t.Find("BtnCancel").gameObject;
		UIEventClick.Set(BtnCancel, OnBtnCancelClick);
    }

    private void OnBtnOKClick(GameObject go)
    {
        if(m_Handler != null)
        {
            m_Handler.OnOK();
        }

        m_RootObj.SetActive(false);
    }

    private void OnBtnCancelClick(GameObject go)
    {
        if(m_Handler != null)
        {
            m_Handler.OnCancel();
        }

        m_RootObj.SetActive(false);
    }

    public void Show(IMessageBoxHandler handler)
    {
        m_Handler = handler;

        m_Info.text = m_Handler.InfoText;

        m_RootObj.SetActive(true);
    }
}