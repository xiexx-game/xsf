//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\XSF\XSFEvent.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：事件模块
// 说明：需要注意的是，事件是异步通知
//
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using UnityEngine;

namespace XSF
{
    public interface IEventSink
    {
        // return false表示不在关心这个事件
        bool OnEvent(uint nEventID, uint nObjectID, object context);
    }

    public sealed class XSFEvent : Singleton<XSFEvent>, IUpdateNode
    {
        private struct SEventInfo
        {
            public uint nEventID;
            public uint nObjectID;
            public object context;
            public ulong nIndex;
        }

        private Dictionary<ulong, List<IEventSink>> m_SubList;
        private Queue<SEventInfo> m_EventList;
        private float m_fTimeSkip;

        public XSFEvent()
        {
            m_SubList = new Dictionary<ulong, List<IEventSink>>();
            m_EventList = new Queue<SEventInfo>();

            m_fTimeSkip = Time.fixedDeltaTime * 2;
        }

        public void Init()
        {
            XSFUpdate.Instance.Add(this);
        }

        public void Fire(uint nEventID, uint nObjectID = 0, object context = null, bool bFireAll = false)
        {
            XSFCore.DebugLog($"XSFEvent.Fire nEventID={nEventID}, obj id={nObjectID}");

            var info = new SEventInfo
            {
                nEventID = nEventID,
                nObjectID = nObjectID,
                context = context,
                nIndex = XSFCore.UINT64_ID(nEventID, nObjectID)
            };

            m_EventList.Enqueue(info);

            if (!bFireAll || nObjectID == 0)
                return;

            var info0 = new SEventInfo
            {
                nEventID = nEventID,
                nObjectID = nObjectID,
                context = context,
                nIndex = XSFCore.UINT64_ID(nEventID, 0)
            };

            m_EventList.Enqueue(info0);
        }

        public bool Subscribe(IEventSink sink, uint nEventID, uint nObjectID)
        {
            if (null == sink)
            {
                Debug.LogError($"XSFEvent::Subscribe sink is null, nEventID={nEventID}, nObjectID={nObjectID}");
                return false;
            }

            ulong nIndex = XSFCore.UINT64_ID(nEventID, nObjectID);

            List<IEventSink> sinkList = null;
            if (m_SubList.TryGetValue(nIndex, out sinkList))
            {
                if (sinkList.Contains(sink))
                {
                    XSFCore.DebugWarning($"XSFEvent::Subscribe sink exists, nEventID={nEventID}, nObjectID={nObjectID}");
                    return false;
                }
            }
            else
            {
                sinkList = new List<IEventSink>();
                m_SubList.Add(nIndex, sinkList);
            }

            XSFCore.DebugLog($"XSFEvent.Subscribe nEventID={nEventID}, obj id={nObjectID}");

            sinkList.Add(sink);

            return true;
        }

        public bool Unsubscribe(IEventSink sink, uint nEventID, uint nObjectID)
        {
            if (null == sink || m_SubList == null)
            {
                return false;
            }

            ulong nIndex = XSFCore.UINT64_ID(nEventID, nObjectID);

            List<IEventSink> sinkList = null;
            if (!m_SubList.TryGetValue(nIndex, out sinkList))
            {
                return false;
            }

            sinkList.Remove(sink);

            if (sinkList.Count <= 0)
            {
                m_SubList.Remove(nIndex);
            }

            return true;
        }

        public bool IsUpdateWroking
        {
            get { return true; }
        }

        public void OnUpdate()
        {
            var fTimeStart = Time.realtimeSinceStartup;

            while (true)
            {
                if (m_EventList.Count <= 0)
                    break;

                var info = m_EventList.Dequeue();

                if (m_SubList.TryGetValue(info.nIndex, out var sinkList))
                {
                    for (var i = 0; i < sinkList.Count;)
                    {
                        XSFCore.DebugLog($"XSFEvent.HandleEvent OnEvent nEventID={info.nEventID}, obj id={info.nObjectID}");
                        if (!sinkList[i].OnEvent(info.nEventID, info.nObjectID, info.context))
                        {
                            sinkList.RemoveAt(i);
                        }
                        else
                        {
                            i++;
                        }
                    }
                }

                var fCurTime = Time.realtimeSinceStartup;
                if (fCurTime - fTimeStart >= m_fTimeSkip)
                {
                    Debug.LogWarning("XSFEvent.Dispatch cost too much time, cost=" + (fCurTime - fTimeStart));
                    break;
                }
            }
        }

        public void OnFixedUpdate() {}
    }
}




