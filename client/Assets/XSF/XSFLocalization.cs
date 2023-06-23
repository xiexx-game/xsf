//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\Scripts\Localization\XSFLocalization.cs
// 作者：Xoen Xie
// 时间：2023/6/23
// 描述：本地化模块
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using System;
using System.IO;
#endif
public enum XSFLanguage 
{
    Chinese = 0,
    English,
}

public class XSFLocalization : Singleton<XSFLocalization>
{
    public readonly string LOCAL_PREFS = "XSFLocalization";
    public string LanSchemaExt { get; private set; }

    public XSFLanguage mCurLanguage { get; private set; }

#if UNITY_EDITOR
    private Dictionary<string, string> m_LocalTexts;
    private DateTime m_lastModifiedTime;
#endif

    public void Init()
    {
        string str = PlayerPrefs.GetString(LOCAL_PREFS);
        InitLocalization(str);
    }

    public void SetLanguage(int language)
    {
        switch((XSFLanguage)language)
        {
        case XSFLanguage.English:
            LanSchemaExt = "English";
            PlayerPrefs.SetString(LOCAL_PREFS, LanSchemaExt);
            mCurLanguage = XSFLanguage.English;
            break;

        default:
            LanSchemaExt = "";
            PlayerPrefs.SetString(LOCAL_PREFS, "Chinese");
            mCurLanguage = XSFLanguage.Chinese;
            break;
        }
    }

    private void InitLocalization(string str)
    {
        if(string.IsNullOrEmpty(str)) {
            switch(Application.systemLanguage)
            {
            case SystemLanguage.Chinese:
            case SystemLanguage.ChineseSimplified:
                SetLanguage((int)XSFLanguage.Chinese);
                break;

            default:  
                SetLanguage((int)XSFLanguage.English);
                break;
            }
            
            return;
        }

        if(str.Equals("English"))
        {
            SetLanguage((int)XSFLanguage.English);
        }
        else
        {
            SetLanguage((int)XSFLanguage.Chinese);
        }
    }

    public string GetText(string key)
    {
#if UNITY_EDITOR
        if(string.IsNullOrEmpty(key))
            key = "Empty Key";

        if(UnityEngine.Application.isPlaying)
        {
            return XSFSchema.Instance.Helper.GetLocalText(key);
        }
        else
        {
            string lanCSV = Application.dataPath + $"/Scp/Language.csv";
            Init();

            if(!string.IsNullOrEmpty(LanSchemaExt))
            {
                lanCSV = Application.dataPath + $"/Scp/Language_{LanSchemaExt}.csv";
            }

            if(m_LocalTexts == null)
            {
                m_lastModifiedTime = File.GetLastWriteTime(lanCSV);
                ReadLanCSV(lanCSV);
            }
            else 
            {
                var curTime = File.GetLastWriteTime(lanCSV);
                if(curTime != m_lastModifiedTime)
                {
                    Debug.Log("language csv has changed, reload local text");
                    m_lastModifiedTime = curTime;
                    ReadLanCSV(lanCSV);
                }
            }

            string text = null;
            if(m_LocalTexts.TryGetValue(key, out text))
                return text;
            else
            {
                Debug.LogError("XSFLocalization can not find local text, key=" + key);
                return key;
            }
                
        }
#else
        return XSFSchema.Instance.Helper.GetLocalText(key);
#endif
        
    }

#if UNITY_EDITOR
    private void ReadLanCSV(string path)
    {
        m_LocalTexts = new Dictionary<string, string>();

        string[] lines = File.ReadAllLines(path);
        for(int i = 3; i < lines.Length; i++)
        {
            string [] values = lines[i].Split(',');
            m_LocalTexts.Add(values[0], values[1].Replace("[comma]",","));
        }
    } 
#endif
}