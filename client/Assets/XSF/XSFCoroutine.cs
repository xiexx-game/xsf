//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\XSF\XSFCoroutine.cs
// 作者：Xoen Xie
// 时间：2022/06/16
// 描述：Unity协程封装
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using System.Collections;
using UnityEngine;

public sealed class XSFCoroutine : Singleton<XSFCoroutine>
{
    private GameObject m_MainObj;
    private MonoCoroutine[] m_Monos;

    public bool Init()
    {
        m_MainObj = new GameObject();
        m_MainObj.name = "XSFCoroutine";
        GameObject.DontDestroyOnLoad(m_MainObj);
        m_Monos = new MonoCoroutine[(int)CoroutineID.Max];

        return true;
    }

    public void Release()
    {
        m_Monos = null;
        GameObject.Destroy(m_MainObj);
        m_MainObj = null;
    }

    public Coroutine StartCoroutine(int nID, IEnumerator routine)
    {
        if(nID >= (int)CoroutineID.Max)
        {
            XSF.LogError($"CoroutineMgr.StartCoroutine nID:{nID} >= CoroutineID.Max:{(int)CoroutineID.Max}");
            return null;
        }

        if(m_Monos[nID] == null)
        {
            GameObject obj = new GameObject();
            obj.name = nID.ToString();
            obj.transform.SetParent(m_MainObj.transform);
            m_Monos[nID] = obj.AddComponent<MonoCoroutine>();
            m_Monos[nID].ID = (CoroutineID)nID;
        }

        return m_Monos[nID].StartCoroutine(routine);
    }

    public void StopCoroutine(int nID, IEnumerator routine)
    {
        if(m_Monos[nID] != null)
            m_Monos[nID].StopCoroutine(routine);
    }

    public void StopCoroutine(int nID, Coroutine routine)
    {
        if(m_Monos[nID] != null)
            m_Monos[nID].StopCoroutine(routine);
    }

    public void StopAllCoroutines(int nID = 0)
    {
        if (nID == 0)
        {
            for (int i = 0; i < (int)CoroutineID.Max; ++i)
            {
                if(m_Monos[i] != null)
                    m_Monos[i].StopAllCoroutines();
            }
                
        }
        else
        {
            if(m_Monos[nID] != null)
                m_Monos[nID].StopAllCoroutines();
        }
    }
}