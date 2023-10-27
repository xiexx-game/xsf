//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\XSF\XSFUpdate.cs
// 作者：Xoen Xie
// 时间：2023/06/15
// 描述：更新模块
// 说明：核心点是，需要逐帧调用时才调用，不需要时关闭
//
//////////////////////////////////////////////////////////////////////////
using System.Collections.Generic;

namespace XSF
{
    public interface IUpdateNode
    {
        // 是否工作，需要注意的是，如果该属性为false时，会从Update列表中移除，并且只会在Update时移除
        bool IsUpdateWroking { get; }

        void OnUpdate();
        void OnFixedUpdate() { }
    }

    public sealed class XSFUpdate : Singleton<XSFUpdate>
    {
        List<IUpdateNode> m_UpdateList;

        public void Init()
        {
            m_UpdateList = new List<IUpdateNode>();
        }

        public void Release()
        {
            m_UpdateList = null;
        }

        public void Add(IUpdateNode node)
        {
            for (int i = 0; i < m_UpdateList.Count; ++i)
            {
                if (m_UpdateList[i] == node)
                    return;
            }

            m_UpdateList.Add(node);
        }

        public void Update()
        {
            for (int i = 0; i < m_UpdateList.Count; ++i)
            {
                if (m_UpdateList[i].IsUpdateWroking)
                {
                    m_UpdateList[i].OnUpdate();
                }
                else
                {
                    m_UpdateList.RemoveAt(i);
                    i--;
                }
            }
        }

        public void FixedUpdate()
        {
            for (int i = 0; i < m_UpdateList.Count; ++i)
            {
                if (m_UpdateList[i].IsUpdateWroking)
                {
                    m_UpdateList[i].OnFixedUpdate();
                }
            }
        }
    }
}