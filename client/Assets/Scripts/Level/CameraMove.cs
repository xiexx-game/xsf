//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Scripts/Level/CameraMove.cs
// 作者：Xoen
// 时间：2023/08/25
// 描述：关卡脚本
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    private enum Status
    {
        None = 0,
        Moving,

    }

    public float Speed;

    private GameObject Start;
    private Transform Target;

    private Status m_nStatus;

    private float m_fLerp;

    public void MoveTo(Transform t)
    {
        Target = t;
        m_nStatus = Status.Moving;
        m_fLerp = 0;
        Start = new GameObject();
        Start.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Start.transform.rotation = Quaternion.LookRotation(transform.forward);
    }

    void Update()
    {
        if(m_nStatus == Status.Moving)
        {
            m_fLerp += Speed * Time.deltaTime;
            // 使用Lerp函数平滑过渡相机位置
            Vector3 smoothedPosition = Vector3.Lerp(Start.transform.position, Target.position, m_fLerp);
            // 设置相机位置
            transform.position = smoothedPosition;

            // 使用Slerp函数平滑过渡相机旋转
            Quaternion smoothedRotation = Quaternion.Lerp(Start.transform.rotation, Target.rotation, m_fLerp);
            // 设置相机旋转
            transform.rotation = smoothedRotation;
            if(m_fLerp >= 1.0f)
            {
                m_nStatus =  Status.None;
            }
        }
    }
}