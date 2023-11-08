
using UnityEngine;



public class SceneAudio : MonoBehaviour
{
    public GameObject [] Objs;
    public float Interval;
    private bool [] m_Check;
    private float m_fLastTime;

    void Awake()
    {
        m_Check = new bool[Objs.Length];
        for(int i = 0; i < Objs.Length; i ++)
        {
            m_Check[i] = Objs[i].activeSelf;
        }
    }

    void Update()
    {
        for(int i = 0; i < Objs.Length; i ++)
        {
            if(m_Check[i] != Objs[i].activeSelf)
            {
                m_Check[i] = Objs[i].activeSelf;
                float fCurrent = Time.realtimeSinceStartup;
                if(fCurrent >= m_fLastTime + Interval )
                {
                    AudioMgr.Instance.PlayFX(AudioID.Click);
                }
            }
        }
    }
}