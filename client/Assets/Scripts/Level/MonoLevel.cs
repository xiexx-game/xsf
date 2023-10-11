//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Scripts/Level/MonoLevel.cs
// 作者：Xoen
// 时间：2023/08/25
// 描述：关卡脚本
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;

public interface XSFAnimHandler
{
    void OnAnimFinish(GameObject obj, string param);
}

public class MonoLevel : MonoBehaviour
{
    public Transform CamaraT;

    public Animator Anim;

    XSFAnimHandler m_Handler;

    public GameObject Map;
    public GameObject[] Objs;

    public Vector3 CharacterPos;

    public Vector3 CharacterTurnPos;

    public Vector3 CharacterEnterPos;
    public Vector3 CharacterExitPos;

    public void Init(XSFAnimHandler handler)
    {
        m_Handler = handler;
    }

    public void PlayAnim()
    {
        if(Anim != null)
        {
            Anim.SetFloat("Ins", 1.0f);
        }
    }

    public void PlayReverse()
    {
        if(Anim != null)
        {
            Anim.SetFloat("Ins", -1.0f);
        }
    }


    public void OnAnimFinish(string param)
    {
        if(m_Handler != null)
            m_Handler.OnAnimFinish(gameObject, param);
    }
}