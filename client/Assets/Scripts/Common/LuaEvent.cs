//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\Scripts\Common\LuaEvent.cs
// 作者：Xoen Xie
// 时间：2023/06/21
// 描述：Lua 事件封装
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using XLua;
using System.Collections.Generic;

[CSharpCallLua]
public delegate bool OnLuaEventFunc( uint nEventID, uint nObjectID, object context );

// 一次性的事件注册
public sealed class LuaEventNode : IEventSink
{
    public int ID = 0;
    public OnLuaEventFunc m_Func;  

    public uint EventID = 0;
    public uint ObjectID = 0;

    public bool OnEvent( uint nEventID, uint nObjectID, object context )
    {
        bool bResult = m_Func(nEventID, nObjectID, context);
        if(!bResult)
        {
            ID = 0;
            m_Func = null;
            EventID = 0;
            ObjectID = 0;
        }

        return bResult;
    }

    public void Subscribe()
    {
        XSFEvent.Instance.Subscribe(this, EventID, ObjectID);
    }

    public void Unsubscribe()
    {
        if(ID > 0 && EventID > 0)
        {
            XSFEvent.Instance.Unsubscribe(this, EventID, ObjectID);
        }
        
        ID = 0;
        m_Func = null;
        EventID = 0;
        ObjectID = 0;
    }
}

public sealed class LuaEvent : Singleton<LuaEvent>
{
    private int m_nIDStart = 1;
    private List<LuaEventNode> m_NodeList;
    
    public LuaEvent()
    {
        m_NodeList = new List<LuaEventNode>();
    }

    public int Subscribe(uint nEventID, uint nObjectID, OnLuaEventFunc func )
    {
        LuaEventNode node = null;
        // 先找一个空闲的node来使用
        for(int i = 0; i < m_NodeList.Count; ++i)
        {
            if(m_NodeList[i].ID == 0)
            {
                node = m_NodeList[i];
                break;
            }
        }

        // 如果未找到，则新建一个
        if(node == null)
        {
            node = new LuaEventNode();
            m_NodeList.Add(node);
        }

        node.ID = m_nIDStart ++;
        node.EventID = nEventID;
        node.ObjectID = nObjectID;
        node.m_Func = func;
        node.Subscribe();

        return node.ID;
    }

    public void Unsubscribe(int nID)
    {
        for(int i = 0; i < m_NodeList.Count; ++i)
        {
            if(m_NodeList[i].ID == nID)
            {
                m_NodeList[i].Unsubscribe();
                break;
            }
        }
    }

}


