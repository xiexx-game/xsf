//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Scripts/XSFLogHelper.cs
// 作者：Xoen Xie
// 时间：2023/08/07
// 描述：XSF 日志助手
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using UnityEngine;


public class XSFLogHelper : MonoSingleton<XSFLogHelper>
{
    private struct LogData
    {
        public string log;
        public GUIStyle style;
    }


    private GUIStyle m_LogErrorStyle;
    private GUIStyle m_LogInfoStyle;
    private GUIStyle m_LogWarnStyle;

    [Header("是否显示帧率信息")] public bool ShowInfo = true;
    [Header("是否显示日志")] public bool ShowLog;

    private Vector2 scrollPosition = Vector2.zero;

    private Rect ShowRect;
    private Rect CloseRect;

    private GUIStyle m_FontStyle;
    private Rect m_Rect;
    private float m_fLastTime;

    private LogData[] m_LogDatas;

    private GUIStyle scrollViewStyle;

    // Use this for initialization
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


        m_LogErrorStyle = new GUIStyle();
        m_LogErrorStyle.fontSize = 14;
        m_LogErrorStyle.normal.textColor = new Color32(255, 0, 0, 255);

        m_LogInfoStyle = new GUIStyle();
        m_LogInfoStyle.fontSize = 14;
        m_LogInfoStyle.normal.textColor = new Color32(255, 255, 255, 255);

        m_LogWarnStyle = new GUIStyle();
        m_LogWarnStyle.fontSize = 14;
        m_LogWarnStyle.normal.textColor = new Color32(255, 255, 0, 255);

        ShowRect = new Rect(90, 5, 80, 40);
        CloseRect = new Rect(Screen.width - 130, Screen.height - 60, 80, 40);

        scrollViewStyle = new GUIStyle();
        scrollViewStyle.normal.background = MakeTex(2, 2, new Color32(80, 80, 80, 255));


    }

    private void OnGUI()
    {
        if (ShowLog)
        {
            ShowLogGUI();
        }

        if (ShowInfo)
        {
            ShowInfoGUI();

            if (GUI.Button(ShowRect, "Open Log"))
            {
                OpenLog();
            }
        }
    }

    public void OpenLog()
    {
        string[] logs = XSFLog.Instance.GetAllLogs();

        if (logs != null)
        {
            m_LogDatas = new LogData[logs.Length];
            for (int i = 0; i < logs.Length; ++i)
            {
                LogData d;
                d.log = logs[i];
                d.style = m_LogInfoStyle;

                if (logs[i].Contains("[ERROR]"))
                {
                    d.style = m_LogErrorStyle;
                }
                else if (logs[i].Contains("[WARN]"))
                {
                    d.style = m_LogWarnStyle;
                }

                m_LogDatas[i] = d;
            }

            ShowLog = true;
        }
    }

    private void ShowLogGUI()
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, scrollViewStyle, GUILayout.Width(Screen.width), GUILayout.Height(Screen.height));

        if (m_LogDatas != null)
        {
            foreach (var d in m_LogDatas)
            {
                GUILayout.Label(d.log, d.style); // 显示文本
            }
        }


        GUILayout.EndScrollView();

        if (GUI.Button(CloseRect, "Close Log"))
        {
            ShowLog = false;
        }
    }

    private int m_FPS;
    private int m_UpdateCount;

    private void ShowInfoGUI()
    {
        var content = $"FPS:{m_FPS}\nRTT:{XSFNet.Instance.RTT}";
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

    Texture2D MakeTex(int width, int height, Color color)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++)
        {
            pix[i] = color;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }

}
