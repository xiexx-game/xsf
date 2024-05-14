//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets/Scripts/Startup/Steps/StartupStepYooAssetInit.cs
// 作者：Xoen Xie
// 时间：2023/06/24
// 描述：YooAsset初始化
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using UnityEngine;
using XSF;
using System.Collections;
using YooAsset;
using System.IO;

public sealed class StartupStepYooAssetInit : StartupStep
{
    public override void Start()
    {
        XSFCoroutine.Instance.StartCoroutine((int)CoroutineID.C0, InitPackage());
        XSFStartup.Instance.UI.SetInfoText("Ready in resources ...");
    }

    private string GetHostServerURL()
    {
        var hostServerIP = XSFConfig.Instance.YooAssetHostIp;
        var appVersion = Application.version;

#if UNITY_EDITOR
        if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.Android)
            return $"{hostServerIP}/{appVersion}/android";
        else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.iOS)
            return $"{hostServerIP}/{appVersion}/ios";
        else
            return $"{hostServerIP}/{appVersion}/editor";
#else
        if (Application.platform == RuntimePlatform.Android)
            return $"{hostServerIP}/{appVersion}/android";
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
            return $"{hostServerIP}/{appVersion}/ios";
        else
            return $"{hostServerIP}/{appVersion}/editor";
#endif
    }

    private float m_fProgress;
    public override float CurrentProgress { get { return m_fProgress; } }

    private IEnumerator InitPackage()
    {
        m_fProgress = 0.5f;
        // 创建资源包裹类
        var package = YooAssets.TryGetPackage(XSFConfig.Instance.YooAssetPackage);
        if (package == null)
            package = YooAssets.CreatePackage(XSFConfig.Instance.YooAssetPackage);

        var playMode = XSFConfig.Instance.YooAssetPlayMode;
        // 编辑器下的模拟模式
        InitializationOperation initializationOperation = null;
        if (playMode == EPlayMode.EditorSimulateMode)
        {
            var createParameters = new EditorSimulateModeParameters();
            createParameters.SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(EDefaultBuildPipeline.BuiltinBuildPipeline.ToString(), XSFConfig.Instance.YooAssetPackage);
            initializationOperation = package.InitializeAsync(createParameters);
        }
        // 联机运行模式
        else if (playMode == EPlayMode.HostPlayMode)
        {
            string defaultHostServer = GetHostServerURL();
            string fallbackHostServer = GetHostServerURL();
            var createParameters = new HostPlayModeParameters();

            if(XSFConfig.Instance.Decryption == YADecryption.FileStream)
            {
                createParameters.DecryptionServices = new FileStreamDecryption();
            }
            else
            {
                createParameters.DecryptionServices = new FileOffsetDecryption();
            }

            createParameters.BuildinQueryServices = new GameQueryServices();
            createParameters.RemoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
            initializationOperation = package.InitializeAsync(createParameters);
        }

        yield return initializationOperation;

        // 如果初始化失败弹出提示界面
        if (initializationOperation.Status != EOperationStatus.Succeed)
        {
            Debug.LogError($"{initializationOperation.Error}");
        }
        else
        {
            XSFConfig.Instance.YAOPackageVersion = initializationOperation.PackageVersion;
        }

        YooAssets.SetDefaultPackage(package);
        m_fProgress = 1.0f;
        IsDone = true;
    }

    /// <summary>
    /// 远端资源地址查询服务类
    /// </summary>
    class RemoteServices : IRemoteServices
    {
        private readonly string _defaultHostServer;
        private readonly string _fallbackHostServer;

        public RemoteServices(string defaultHostServer, string fallbackHostServer)
        {
            _defaultHostServer = defaultHostServer;
            _fallbackHostServer = fallbackHostServer;
        }
        string IRemoteServices.GetRemoteMainURL(string fileName)
        {
            return $"{_defaultHostServer}/{fileName}";
        }
        string IRemoteServices.GetRemoteFallbackURL(string fileName)
        {
            return $"{_fallbackHostServer}/{fileName}";
        }
    }

    /// <summary>
    /// 资源文件流加载解密类
    /// </summary>
    private class FileStreamDecryption : IDecryptionServices
    {
        /// <summary>
        /// 同步方式获取解密的资源包对象
        /// 注意：加载流对象在资源包对象释放的时候会自动释放
        /// </summary>
        AssetBundle IDecryptionServices.LoadAssetBundle(DecryptFileInfo fileInfo, out Stream managedStream)
        {
            BundleStream bundleStream = new BundleStream(fileInfo.FileLoadPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            managedStream = bundleStream;
            return AssetBundle.LoadFromStream(bundleStream, fileInfo.ConentCRC, GetManagedReadBufferSize());
        }

        /// <summary>
        /// 异步方式获取解密的资源包对象
        /// 注意：加载流对象在资源包对象释放的时候会自动释放
        /// </summary>
        AssetBundleCreateRequest IDecryptionServices.LoadAssetBundleAsync(DecryptFileInfo fileInfo, out Stream managedStream)
        {
            BundleStream bundleStream = new BundleStream(fileInfo.FileLoadPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            managedStream = bundleStream;
            return AssetBundle.LoadFromStreamAsync(bundleStream, fileInfo.ConentCRC, GetManagedReadBufferSize());
        }

        private static uint GetManagedReadBufferSize()
        {
            return 1024;
        }
    }

    /// <summary>
    /// 资源文件偏移加载解密类
    /// </summary>
    private class FileOffsetDecryption : IDecryptionServices
    {
        /// <summary>
        /// 同步方式获取解密的资源包对象
        /// 注意：加载流对象在资源包对象释放的时候会自动释放
        /// </summary>
        AssetBundle IDecryptionServices.LoadAssetBundle(DecryptFileInfo fileInfo, out Stream managedStream)
        {
            managedStream = null;
            return AssetBundle.LoadFromFile(fileInfo.FileLoadPath, fileInfo.ConentCRC, GetFileOffset());
        }

        /// <summary>
        /// 异步方式获取解密的资源包对象
        /// 注意：加载流对象在资源包对象释放的时候会自动释放
        /// </summary>
        AssetBundleCreateRequest IDecryptionServices.LoadAssetBundleAsync(DecryptFileInfo fileInfo, out Stream managedStream)
        {
            managedStream = null;
            return AssetBundle.LoadFromFileAsync(fileInfo.FileLoadPath, fileInfo.ConentCRC, GetFileOffset());
        }

        private static ulong GetFileOffset()
        {
            return XSFCore.YOO_OFFSET;
        }
    }
}





