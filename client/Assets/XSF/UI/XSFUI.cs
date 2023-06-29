//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\XSF\UI\XSFUI.cs
// 作者：Xoen Xie
// 时间：2023/06/15
// 描述：UI 模块
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections.Generic;

public interface IUIHelper
{
    public int MaxID { get; }
    UIBase GetUI(int nID);
}

public sealed class XSFUI : Singleton<XSFUI>, IUpdateNode
{
    public GameObject Root { get; private set; }
    public Transform UIRootT { get; private set; }

    private List<UIBase> m_ShowList;

    private UIBase[] m_UIs;

    public IUIHelper Helper { get; private set; }

    public void Init(IUIHelper helper)
    {
        Helper = helper;

        Root = XSFMain.Instance.UIRoot;
        UIRootT = Root.transform.Find("Root");

        XSFUpdate.Instance.Add(this);

        m_ShowList = new List<UIBase>();
        m_UIs = new UIBase[Helper.MaxID];
    }


    public void AddUI(UIBase ui)
    {
        m_ShowList.Add(ui);
    }

    public T Get<T>(int id) where T : UIBase
    {
        if (id >= Helper.MaxID)
            return null;

        if(m_UIs[id] == null)
            m_UIs[id] = Helper.GetUI(id);

        return m_UIs[id] as T;
    }


    public bool IsUpdateWroking { get { return true; } }

    public void OnUpdate() 
    {
        for(int i = 0; i < m_ShowList.Count;)
        {
            if(!m_ShowList[i].IsShow)
            {
                m_ShowList.RemoveAt(i);
            }
            else
            {
                m_ShowList[i].Update();
                i ++;
            }
        }
    }

    public void OnFixedUpdate() 
    {


    }

}
