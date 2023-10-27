//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Scripts/Level/MonoBox.cs
// 作者：Xoen
// 时间：2023/08/25
// 描述：箱子脚本
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;

public class MonoBox : MonoBehaviour
{

    private Vector3 m_Target;

    private Vector3 m_MoveDir;

    private bool m_bMoving;

    public MonoSelect select;

    public void Move(Vector3 target)
    {
        m_Target = target;
        m_MoveDir = m_Target - transform.localPosition;
        m_MoveDir.Normalize();
        m_bMoving = true;
        //Debug.Log("MonoBox move target=" + target + ", local=" + transform.localPosition);
    }

    void Update()
    {
        if(m_bMoving)
        {
            float dis = GameConfig.Instance.RunSpeed * Time.deltaTime;
            float length = Vector3.Distance(transform.localPosition, m_Target);
            //Debug.Log("MonoBox move dis=" + dis + ", length=" + length + ", transform.localPosition" + transform.localPosition);
            if(dis > length)
            {
                transform.localPosition = m_Target;
                m_bMoving = false;
                //Debug.Log("transform.localPosition 1=" + transform.localPosition);
            }
            else
            {
                transform.localPosition += m_MoveDir * dis;
                //Debug.Log("transform.localPosition 2=" + transform.localPosition);
            }
        }
        
    }
}