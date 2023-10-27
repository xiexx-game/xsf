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
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace XSF
{
    public static class XSFCore
    {
        private static Queue<GameObject> AASGoList;

        public static void Init()
        {
            int seed = DateTime.Now.GetHashCode();
            UnityEngine.Random.InitState(seed);

            XSFLog.Instance.Init();
            UnityEngine.Debug.Log("XSF.Init start");

            XSFLocalization.Instance.Init();
            XSFUpdate.Instance.Init();
            XSFCoroutine.Instance.Init();
            XSFEvent.Instance.Init();

            AASGoList = new Queue<GameObject>();
        }

        public static void ReleaseAASGo(GameObject gameObject)
        {
            AASGoList.Enqueue(gameObject);
        }

        public static void Release()
        {
            UnityEngine.Debug.Log("XSF.Release start");

            try
            {
                XSFUpdate.Instance.Release();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"XSF.Release catch exception, message={e.Message}, stack={e.StackTrace}");
            }

            XSFLog.Instance.Release();
        }

        public static void Update()
        {
            XSFUpdate.Instance.Update();

            if (AASGoList.Count > 0)
            {
                GameObject go = AASGoList.Dequeue();
                Addressables.ReleaseInstance(go);
            }
        }

        public static void FixedUpdate()
        {
            XSFUpdate.Instance.FixedUpdate();
        }

        [Conditional("XSF_DEBUG")]
        public static void DebugLog(string log)
        {
            UnityEngine.Debug.Log(log);
        }

        [Conditional("XSF_DEBUG")]
        public static void DebugWarning(string log)
        {
            UnityEngine.Debug.LogWarning(log);
        }

        [Conditional("XSF_DEBUG")]
        public static void DebugError(string log)
        {
            UnityEngine.Debug.LogError(log);
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

        public const uint SCHEMA_EVENT_ID = 1;
        public const uint UI_SHOW_EVENT_ID = 2;
    }
}