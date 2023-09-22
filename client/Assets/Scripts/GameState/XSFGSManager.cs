//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\XSF\GameState\XSFGSManager.cs
// 作者：Xiexx
// 时间：2022/03/05
// 描述：游戏状态机
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using System;

public class XSFGSManager : Singleton<XSFGSManager>
{
    private XSFGameState[] m_States;
    private XSFGameState m_CurState;
    private XSFGSID m_nNextStateID;
    public XSFGSID mNextStateID
    {
        set
        {
            m_nNextStateID = value;
        }
    }

    public XSFGSManager()
    {
        m_States = new XSFGameState[(int)XSFGSID.Max];
        m_States[(int)XSFGSID.None] = new XSFGameState();
        m_States[(int)XSFGSID.Main] = new XSFGameStateMain();
        m_States[(int)XSFGSID.Play] = new XSFGameStatePlay();

        m_CurState = m_States[(int)XSFGSID.None];
        m_nNextStateID = XSFGSID.None;
    }

    public void Update()
    {
        if (m_nNextStateID != m_CurState.mID)
        {
            m_CurState.End();

            m_CurState = m_States[(int)m_nNextStateID];

            m_CurState.Enter();         
        }

        m_CurState.Update();
    }
}
