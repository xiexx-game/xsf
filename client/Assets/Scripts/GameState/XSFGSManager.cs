//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Scripts/GameState/XSFGSManager.cs
// 作者：Xoen
// 时间：2023/08/25
// 描述：游戏状态机
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;

public class XSFGSManager : Singleton<XSFGSManager>, IUpdateNode
{
    private XSFGameState[] m_States;
    private XSFGameState m_CurState;
    private XSFGSID m_nNextStateID;
    public XSFGSID mNextStateID
    {
        set
        {
            m_nNextStateID = value;
            Debug.Log($"XSFGSManager next state id={m_nNextStateID}");
        }
    }

    public XSFGSManager()
    {
        m_States = new XSFGameState[(int)XSFGSID.Max];
        m_States[(int)XSFGSID.None] = new XSFGameState();
        m_States[(int)XSFGSID.Home] = new XSFGameStateHome();
        m_States[(int)XSFGSID.Play] = new XSFGameStatePlay();

        m_CurState = m_States[(int)XSFGSID.None];
        m_nNextStateID = XSFGSID.None;
    }

    public void Init()
    {
        XSFUpdate.Instance.Add(this);
    }

    public void OnUpdate()
    {
        if (m_nNextStateID != m_CurState.mID)
        {
            m_CurState.Exit();
            Debug.Log($"XSFGSManager.Update {m_CurState.mID} Exit");

            m_CurState = m_States[(int)m_nNextStateID];

            Debug.Log($"XSFGSManager.Update {m_CurState.mID} Enter");
            m_CurState.Enter();         
        }

        m_CurState.OnUpdate();
    }

    public bool IsUpdateWroking { get { return true; } }

    public void  OnFixedUpdate() { }
}
