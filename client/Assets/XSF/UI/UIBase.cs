//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\XSF\UI\UIBase.cs
// 作者：Xoen Xie
// 时间：2023/06/28
// 描述：UI 基类
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;
using YooAsset;

namespace XSF
{
    public class UIBase
    {
        enum UIStatus
        {
            None = 0,
            Loading,    // 正在加载中
            BeforeShow,
            Show,       // 已显示
            Hide,       // 隐藏，资源未销毁
            Close,      // 已关闭
        }

        struct UIRefreshData
        {
            public uint nRefreshID;
            public object data;
        }

        public Transform RootT { get; private set; }
        public GameObject RootGo { get; private set; }
        public Canvas Canvas { get; private set; }

        private Queue<UIRefreshData> m_FreshQueue;

        private UIStatus m_nStatus;

        public virtual string Name { get; }
        public virtual uint EventObjID { get; }
        public virtual string SortingLayerName { get; }

        private AssetHandle m_Handle;

        private const int MAX_REFRESH = 10;

        public bool IsShow { get { return m_nStatus == UIStatus.Show; } }

        public bool NeedRemove { get { return m_nStatus != UIStatus.Show && m_nStatus != UIStatus.BeforeShow; } }

        public UIBase()
        {

        }

        public virtual void OnInit() { }
        public virtual void OnHide() { }
        public virtual void OnShow() { }
        public virtual void OnClose() { }
        public virtual void OnRefresh(uint nFreshID, object data) { }
        public virtual void OnUpdate() { }

        public void Show()
        {
            switch (m_nStatus)
            {
                case UIStatus.None:
                case UIStatus.Close:
                    LoadUI();
                    m_nStatus = UIStatus.Loading;
                    break;

                case UIStatus.Hide:
                    if (RootGo == null)
                    {
                        LoadUI();
                        m_nStatus = UIStatus.Loading;
                    }
                    else
                    {
                        RootGo.SetActive(true);
                        RootT.SetAsLastSibling();

                        m_nStatus = UIStatus.BeforeShow;
                        XSFUI.Instance.AddUI(this);
                    }
                    break;
            }
        }

        private void LoadUI()
        {
            var package = YooAssets.GetPackage(XSFConfig.Instance.YooAssetPackage);
            m_Handle = package.LoadAssetAsync<GameObject>(Name);
            m_Handle.Completed += (AssetHandle handle) =>
            {
                GameObject go = handle.InstantiateSync();
                OnUILoadCall(go);
            };
        }

        private void OnUILoadCall(GameObject go)
        {
            RootGo = go;
            RootGo.name = Name;
            RootT = RootGo.transform;

            RootT.SetParent(XSFUI.Instance.UIRootT, false);
            RootT.localRotation = Quaternion.identity;
            RootT.localPosition = Vector3.zero;
            RootT.localScale = Vector3.one;

            var rt = RootT.GetComponent<RectTransform>();
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = Vector2.zero;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;

            RootT.SetAsLastSibling();

            Canvas = RootT.GetComponent<Canvas>();
            if (!string.IsNullOrEmpty(SortingLayerName))
            {
                Canvas.overrideSorting = true;
                Canvas.sortingLayerName = SortingLayerName;
            }

            OnInit();

            m_nStatus = UIStatus.BeforeShow;
            XSFUI.Instance.AddUI(this);
        }

        public void Refresh(uint nRefreshID, object data)
        {
            switch (m_nStatus)
            {
                case UIStatus.Hide:
                case UIStatus.Close:
                case UIStatus.None:
                    Debug.LogWarning("UIBase.Refresh can not fresh ui, please call Show() first, name=" + Name);
                    return;
            }

            if (m_FreshQueue == null)
                m_FreshQueue = new Queue<UIRefreshData>();

            UIRefreshData urd;
            urd.nRefreshID = nRefreshID;
            urd.data = data;
            m_FreshQueue.Enqueue(urd);
        }

        public void Hide()
        {
            switch (m_nStatus)
            {
                case UIStatus.Hide:
                case UIStatus.Close:
                case UIStatus.None:
                    return;
            }

            if (RootGo == null)
            {
                
            }
            else
            {
                OnHide();

                RootGo.SetActive(false);
            }

            if (m_FreshQueue != null)
                m_FreshQueue.Clear();

            m_nStatus = UIStatus.Hide;
        }

        public void Close()
        {
            switch (m_nStatus)
            {
                case UIStatus.Hide:
                case UIStatus.Close:
                case UIStatus.None:
                    return;
            }

            if (RootGo == null)
            {

            }
            else
            {
                OnClose();

                GameObject.Destroy(RootGo);
                RootGo = null;
                RootT = null;
            }

            if (m_Handle != null)
            {
                m_Handle.Release();
                m_Handle = null;
            }

            if (m_FreshQueue != null)
                m_FreshQueue.Clear();

            m_nStatus = UIStatus.Close;
        }

        public void Update()
        {
            switch (m_nStatus)
            {
                case UIStatus.BeforeShow:
                    {
                        OnShow();
                        m_nStatus = UIStatus.Show;

                        if (EventObjID > 0)
                        {
                            XSFEvent.Instance.Fire(XSFCore.UI_SHOW_EVENT_ID, EventObjID);
                        }
                    }
                    break;

                case UIStatus.Show:
                    {
                        if (m_FreshQueue != null && m_FreshQueue.Count > 0)
                        {
                            int nCount = m_FreshQueue.Count;
                            if (m_FreshQueue.Count > MAX_REFRESH)
                            {
                                nCount = MAX_REFRESH;
                            }

                            while (nCount > 0)
                            {
                                nCount--;
                                UIRefreshData urd = m_FreshQueue.Dequeue();
                                OnRefresh(urd.nRefreshID, urd.data);
                            }
                        }

                        OnUpdate();
                    }
                    break;
            }
        }

    }
}