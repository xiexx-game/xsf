//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\XSF\XSFConfig.cs
// 作者：Xoen Xie
// 时间：2023/06/16
// 描述：XSF框架配置
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using UnityEngine;
using YooAsset;

namespace XSF
{
    public enum YADecryption
    {
        FileStream = 0,
        FileOffset
    }

    public sealed class XSFConfig : MonoSingleton<XSFConfig>
    {
        [Header("游戏默认帧率")][Range(30, 60)] public int TargetFrameRate = 60;

        [Header("从文件加载配置")] public bool LoadScpInFiles;

        [Header("网络心跳间隔（秒）")] public float HeartbeatInterval;

        [Header("YooAsset包名")] public string YooAssetPackage = "DefaultPackage";

        [Header("YooAsset模式")] public EPlayMode YooAssetPlayMode = EPlayMode.EditorSimulateMode;

        [Header("YooAsset热更服务器地址")] public string YooAssetHostIp;

        [Header("YooAsset加密方式")] public YADecryption Decryption;

        [Header("YooAsset本地包版本")]
        public string YAOPackageVersion;

        [Header("YooAsset最新包版本")]
        public string YANPackageVersion;


        protected override void Awake()
        {
            base.Awake();

#if UNITY_EDITOR
        
#else
            LoadScpInFiles = false;
#endif
            Application.targetFrameRate = TargetFrameRate;

            Application.runInBackground = true;
        }
    }
}