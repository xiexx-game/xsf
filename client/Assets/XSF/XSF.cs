//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\XSF\XSF.cs
// 作者：Xoen Xie
// 时间：2023/06/15
// 描述：框架接口
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using System;
using UnityEngine;
using System.Diagnostics;

public static class XSF
{
    public static void Init()
    {
        XSFLog.Instance.Init();
        Log("XSF.Init start");

        XSFUpdate.Instance.Init();
        XSFCoroutine.Instance.Init();
        XSFUpdate.Instance.Init();
        XSFLua.Instance.Init();
    }

    public static void Release()
    {
        Log("XSF.Release start");

        try
        {
            XSFUpdate.Instance.Release();
        }
        catch (Exception e)
        {
            LogError($"XSF.Release catch exception, message={e.Message}, stack={e.StackTrace}");
        }

        XSFLog.Instance.Release();
    }

    public static void Update()
    {
        XSFUpdate.Instance.Update();
    }

    public static void FixedUpdate()
    {
        XSFUpdate.Instance.FixedUpdate();
    }

    public static void LateUpdate()
    {
        XSFUpdate.Instance.LateUpdate();
    }

    [Conditional("XSF_DEBUG")]
    public static void DebugLog(string log)
    {
        XSFLog.Instance.Push(LogType.Log, log);
        UnityEngine.Debug.Log(log);
    }

    [Conditional("XSF_DEBUG")]
    public static void DebugWarning(string log)
    {
        XSFLog.Instance.Push(LogType.Warning, log);
        UnityEngine.Debug.LogWarning(log);
    }

    [Conditional("XSF_DEBUG")]
    public static void DebugError(string log)
    {
        XSFLog.Instance.Push(LogType.Error, log);
        UnityEngine.Debug.LogError(log);
    }


    public static void Log(string format, params object[] args)
    {
        var s = string.Format(format, args);

        XSFLog.Instance.Push(LogType.Log, s);

#if UNITY_EDITOR
        UnityEngine.Debug.Log(s);
#endif
    }

    public static void Log(string log)
    {
        XSFLog.Instance.Push(LogType.Log, log);

#if UNITY_EDITOR
        UnityEngine.Debug.Log(log);
#endif
    }

    public static void LogWarning(string format, params object[] args)
    {
        var s = string.Format(format, args);
        XSFLog.Instance.Push(LogType.Warning, s);

#if UNITY_EDITOR
        UnityEngine.Debug.LogWarning(s);
#endif
    }

    public static void LogWarning(string log)
    {
        XSFLog.Instance.Push(LogType.Warning, log);

#if UNITY_EDITOR
        UnityEngine.Debug.LogWarning(log);
#endif
    }

    public static void LogError(string format, params object[] args)
    {
        var s = string.Format(format, args);

        XSFLog.Instance.Push(LogType.Error, s);

#if UNITY_EDITOR
        UnityEngine.Debug.LogError(s);
#endif
    }

    public static void LogError(string log)
    {
        XSFLog.Instance.Push(LogType.Error, log);

#if UNITY_EDITOR
        UnityEngine.Debug.LogError(log);
#endif
    }

    /// <summary>
    /// 64位混合ID
    /// </summary>
    public static ulong UINT64_ID(uint k1, uint k2)
    {
        return (((ulong)k1) << 32 | ((ulong)k2));
    }

    public static uint FIRST_UINT64_ID(ulong k)
    {
        return (uint)(k >> 32);
    }

    public static uint SECOND_UINT64_ID(ulong k)
    {
        return ((uint)(k & 0xFFFFFFFF));
    }


    /// <summary>
    /// 32位混合KEY
    /// </summary>
    public static uint UINT_ID(ushort k1, ushort k2)
    {
        return (uint)(k1 << 16 | k2);
    }

    public static ushort FIRST_UINT_ID(uint k)
    {
        return (ushort)(k >> 16);
    }

    public static ushort SECOND_UINT_ID(uint k)
    {
        return (ushort)(k & 0x0000FFFF);
    }


    /// <summary>
    /// 16位混合ID
    /// </summary>
    public static ushort USHORT_ID(byte k1, byte k2)
    {
        return (ushort)(k1 << 8 | k2);
    }

    public static byte FIRST_USHORT_ID(ushort k)
    {
        return (byte)(k >> 8);
    }

    public static byte SECOND_USHORT_ID(ushort k)
    {
        return (byte)(k & 0x000000FF);
    }

    /// <summary>
    /// 时间戳计算时间
    /// </summary>
    public static DateTime DateFrom = new(1970, 1, 1, 0, 0, 0);

    // 当前UTC时间，单位毫秒
    public static ulong CurrentMS => (ulong)(DateTime.Now - DateFrom).TotalMilliseconds;
}