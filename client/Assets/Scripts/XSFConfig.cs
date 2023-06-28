//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\XSF\XSFConfig.cs
// 作者：Xoen Xie
// 时间：2023/06/16
// 描述：XSF框架配置
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using UnityEngine;

public sealed class XSFConfig : MonoSingleton<XSFConfig>
{
    [Header("游戏默认帧率")][Range(30, 60)] public int TargetFrameRate = 60;

    [Header("是否显示帧率信息")] public bool ShowInfo;

    [Header("从文件加载配置")] public bool LoadScpInFiles;

    [Header("网络心跳间隔（秒）")] public float HeartbeatInterval;

    [Header("是否开启资源更新")] public bool AASUpdateOpen;


    protected override void Awake()
    {
        base.Awake();

#if UNITY_EDITOR
        ShowInfo = true;
#else
        ShowInfo = false;
        LoadScpInFiles = false;
        AASUpdateOpen = true;
#endif
        Application.targetFrameRate = TargetFrameRate;
    }

    private GUIStyle m_FontStyle;
    private Rect m_Rect;
    private float m_fLastTime;

    void Start()
    {
        m_FontStyle = new GUIStyle();
        m_FontStyle.alignment = TextAnchor.MiddleCenter;
        m_FontStyle.fontSize = 20;
        m_FontStyle.fontStyle = FontStyle.Bold;
        m_FontStyle.alignment = TextAnchor.MiddleLeft;
        m_FontStyle.normal.textColor = new Color32(0, 255, 0, 255);

        m_Rect = new Rect(5, 5, 80, 80);

        m_fLastTime = Time.realtimeSinceStartup;
    }

    private void OnGUI()
    {
        if (ShowInfo)
        {
            ShowInfoGUI();
        }
    }

    private int m_FPS;
    private int m_UpdateCount;

    private void ShowInfoGUI()
    {
        var content = $"FPS:{m_FPS}";
        GUI.Label(m_Rect, content, m_FontStyle);
    }

    private void Update()
    {
        if (ShowInfo)
        {
            m_UpdateCount++;

            var fCurTime = Time.realtimeSinceStartup;
            if (fCurTime > m_fLastTime + 1)
            {
                var fTimePass = fCurTime - m_fLastTime;
                m_FPS = (int)(m_UpdateCount / fTimePass);

                m_fLastTime = fCurTime;
                m_UpdateCount = 0;
            }
        }
    }
}