//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\XSF\XSFQueue.cs
// 作者：Xoen Xie
// 时间：2023/06/15
// 描述：一个简单的无锁队列，同一时刻只允许一个线程读，一个线程写
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using System;
using UnityEngine;

public sealed class XSFQueue<T>
{
    class _Node
    {
        public T data;
        public _Node next;
    }

    private _Node m_Head;
    private _Node m_Tail;

    private ulong m_nPushCount;
    private ulong m_nPopCount;

    public XSFQueue()
    {
        m_Head = new _Node();
        m_Head.next = null;
        m_Tail = m_Head;
    }

    public void Push(T data)
    {
        _Node newNode = new _Node();
        newNode.data = data;
        newNode.next = null;

        m_Tail.next = newNode;
        m_Tail = newNode;

        ++m_nPushCount;
    }

    public bool Pop(out T data)
    {
        if (m_Head.next != null)
        {
            ++m_nPopCount;

            m_Head = m_Head.next;
            data = m_Head.data;

            return true;
        }

        data = default(T);

        return false;
    }

    public ulong Count
    {
        get
        {
            return m_nPushCount - m_nPopCount;
        }
    }
}