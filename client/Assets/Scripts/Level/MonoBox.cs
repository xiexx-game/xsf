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
        m_MoveDir = m_Target - transform.position;
        m_MoveDir.Normalize();
        m_bMoving = true;
    }

    void Update()
    {
        if(m_bMoving)
        {
            float dis = GameConfig.Instance.RunSpeed * Time.deltaTime;
            float length = Vector3.Distance(transform.position, m_Target);
            if(dis > length)
            {
                transform.position = m_Target;
                m_bMoving = false;
            }
            else
            {
                transform.position += m_MoveDir * dis;
            }
        }
        
    }
}