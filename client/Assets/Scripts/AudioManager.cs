//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Scripts/AudioManager.cs
// 作者：Xoen Xie
// 时间：2023/08/31
// 描述：声音管理
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using UnityEngine;

public enum BGMID
{
    Main = 0,
    Tetris,
    Snake,
    PacMan,
}

public enum ClipID
{
    Click = 0,
    Cycle,
    DropDown,
    HighScore,
    Link,
    LinkDone,
    Popup,
    SngleSelect,
    StartCreate,
    Rest,
    Wrong,
}

public sealed class AudioManager : MonoSingleton<AudioManager>
{
    public AudioSource Background;
    public AudioSource UI;

    public AudioSource FX;
    public AudioSource AudioDelay;

    public AudioClip[] BGM;

    public AudioClip[] Clips;

    public void PlayBGM(BGMID id)
    {
        Background.clip = BGM[(int)id];
        Background.Play();
    }

    public void SetBGMVolumn(float v)
    {
        Background.volume = v;
    }

    public void StopBGM()
    {
        Background.Stop();
    }

    public void PlayUIAudio(ClipID id)
    {
        UI.PlayOneShot(Clips[(int)id]);
    }

    public void PlayFXAudio(ClipID id)
    {
        FX.PlayOneShot(Clips[(int)id]);
    }

    public void PlayFXAudio(ClipID id, float timeDelay)
    {
        // 计划开始时间
        double scheduledTime = AudioSettings.dspTime + timeDelay; // 在当前时间后的 3 秒开始播放
        
        // 播放音频
        AudioDelay.clip = Clips[(int)id];
        AudioDelay.PlayScheduled(scheduledTime);
    }
}