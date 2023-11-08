//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Scripts/AudioMgr.cs
// 作者：Xoen Xie
// 时间：2023/06/16
// 描述：XSF框架配置
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using UnityEngine;

public enum BGMID
{
    Lobby = 0,
    Level = 1,
}

public enum AudioID
{
    Click = 0,
    OK,
    Move,
    Finish,
    Pop,
}

public sealed class AudioMgr : MonoSingleton<AudioMgr>
{
    public AudioClip [] BGM;

    public AudioClip [] FX;

    public AudioSource m_ASBGM;
    public AudioSource m_Character;

    public AudioSource m_FX;

    public void PlayBGM(BGMID nID)
    {
        m_ASBGM.clip = BGM[(int)nID];
        m_ASBGM.Play();
    }

    public void StopBGM()
    {
        m_ASBGM.Stop();
    }

    public void PlayWalk(bool bStop)
    {
        if(bStop)
        {
            m_Character.Play();
        }
        else
        {
            m_Character.Stop();
        }
    }

    public void PlayFX(AudioID nID)
    {
        m_FX.PlayOneShot(FX[(int)nID]);
    }
}