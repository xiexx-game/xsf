//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\XSF\UI\Component\UILocalizationTMP.cs
// 作者：Xoen Xie
// 时间：2023/06/20
// 描述：本地化脚本
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;
using TMPro;
using XLua;

[LuaCallCSharp]
[ExecuteInEditMode]
[RequireComponent(typeof(TextMeshProUGUI))]
public class UILocalizationTMP : MonoBehaviour
{
    TextMeshProUGUI m_TMPText;

    [Header("多语言key")]
    public string Key;

    void Awake()
    {
        Debug.Log("UILocalizationTMP Awake");
        m_TMPText = gameObject.GetComponent<TextMeshProUGUI>();
        UpdateText();
    }

    public void UpdateText()
    {
        m_TMPText.text = XSFLocalization.Instance.GetText(Key);
    }
}