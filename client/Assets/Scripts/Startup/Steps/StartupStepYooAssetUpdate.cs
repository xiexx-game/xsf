//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets/Scripts/Startup/Steps/StartupStepYooAssetUpdate.cs
// 作者：Xoen Xie
// 时间：2023/06/24
// 描述：YooAsset 更新
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using UnityEngine;
using XSF;
using System.Collections;
using YooAsset;
using System.IO;
using UnityEngine.Rendering;
using Unity.VisualScripting.Dependencies.NCalc;

public sealed class StartupStepYooAssetUpdate : StartupStep
{
    internal enum DownloadStep
    {
        None = 0,
        UpdateVersion,
        UpdateManifest,
        CreateDownloader,
        DownloadFiles,
        ClearCache,
    }

    private DownloadStep m_nStep;
    private ResourceDownloaderOperation m_Downloader;

    internal DownloadStep Step 
    {
        set 
        {
            m_nStep = value;
        }
    }

    public override void Start()
    {
        m_nStep = DownloadStep.UpdateVersion;
    }

    public override void End()
    {
        XSFCore.OnContentUpdateDone();
    }

    private float m_fProgress;
    public override float CurrentProgress { get { return m_fProgress; } }

    public override void Update()
    {
        switch(m_nStep)
        {
        case DownloadStep.UpdateVersion:
            XSFStartup.Instance.UI.SetInfoText("starts to detect the resource version ...");
            XSFCoroutine.Instance.StartCoroutine((int)CoroutineID.C0, UpdatePackageVersion());
            break;

        case DownloadStep.UpdateManifest:
            XSFCoroutine.Instance.StartCoroutine((int)CoroutineID.C0, UpdateManifest());
            break;

        case DownloadStep.CreateDownloader:
            XSFCoroutine.Instance.StartCoroutine((int)CoroutineID.C0, CreateDownloader());
            break;

        case DownloadStep.DownloadFiles:
            XSFStartup.Instance.UI.SetInfoText("Start downloading new resource files ...");
            XSFCoroutine.Instance.StartCoroutine((int)CoroutineID.C0, BeginDownload());
            break;

        case DownloadStep.ClearCache:
            {
                var package = YooAssets.GetPackage(XSFConfig.Instance.YooAssetPackage);
                var operation = package.ClearUnusedCacheFilesAsync();
                operation.Completed += op => 
                {
                    IsDone = true;
                };
            }
            break;

        default:
            break;
        }
        
        m_nStep = DownloadStep.None;
    }

    private IEnumerator UpdatePackageVersion()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        var package = YooAssets.GetPackage(XSFConfig.Instance.YooAssetPackage);
        var operation = package.UpdatePackageVersionAsync();
        yield return operation;

        if (operation.Status != EOperationStatus.Succeed)
        {
            Debug.LogError(operation.Error);
            //PatchEventDefine.PackageVersionUpdateFailed.SendEventMessage();
        }
        else
        {   
            XSFConfig.Instance.YANPackageVersion = operation.PackageVersion;
            m_nStep = DownloadStep.UpdateManifest;
        }
    }

    private IEnumerator UpdateManifest()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        var package = YooAssets.GetPackage(XSFConfig.Instance.YooAssetPackage);
        bool savePackageVersion = true;
        var operation = package.UpdatePackageManifestAsync(XSFConfig.Instance.YANPackageVersion, savePackageVersion);
        yield return operation;

        if (operation.Status != EOperationStatus.Succeed)
        {
            Debug.LogWarning(operation.Error);
            yield break;
        }
        else
        {
            m_nStep = DownloadStep.CreateDownloader;
        }
    }

    IEnumerator CreateDownloader()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        var package = YooAssets.GetPackage(XSFConfig.Instance.YooAssetPackage);
        int downloadingMaxNum = 1;
        int failedTryAgain = 3;
        m_Downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);


        if (m_Downloader.TotalDownloadCount == 0)
        {
            Debug.Log("Not found any download files !");
            IsDone = true;
        }
        else
        {
            // 发现新更新文件后，挂起流程系统
            // 注意：开发者需要在下载前检测磁盘空间不足
            int totalDownloadCount = m_Downloader.TotalDownloadCount;
            long totalDownloadBytes = m_Downloader.TotalDownloadBytes;

            string totalSizeMB = Bytes2String(totalDownloadBytes);
            Debug.Log($"Found update patch files, Total count {totalDownloadCount} Total szie {totalSizeMB}MB");

            XSFStartup.Instance.UI.ShowMessageBox(new MessageBoxDownload(this, 
                $"Currently, {totalDownloadCount} files need to be updated, with a total of {totalSizeMB}MB. Do you need to update them? "));
        }
    }

    private IEnumerator BeginDownload()
    {
        m_Downloader.OnDownloadErrorCallback = OnDownloadError;
        m_Downloader.OnDownloadProgressCallback = OnDownloadProgress;
        m_Downloader.BeginDownload();
        yield return m_Downloader;

        // 检测下载结果
        if (m_Downloader.Status != EOperationStatus.Succeed)
            yield break;

        m_nStep = DownloadStep.ClearCache;
    }

    string Bytes2String(long length)
    {
        float sizeMB = length / 1048576f;
        sizeMB = Mathf.Clamp(sizeMB, 0.1f, float.MaxValue);
        return sizeMB.ToString("f1");
    }

    void OnDownloadProgress(int totalDownloadCount, int currentDownloadCount, long totalDownloadBytes, long currentDownloadBytes)
    {
        m_fProgress = (float)currentDownloadCount/totalDownloadCount;

        Debug.LogWarning($"OnDownloadProgress {Bytes2String(currentDownloadBytes)}MB/{Bytes2String(totalDownloadBytes)}MB m_fProgress={m_fProgress}");

        XSFStartup.Instance.UI.SetInfoText($"Downloading ...  {currentDownloadCount}/{totalDownloadCount}  {Bytes2String(currentDownloadBytes)}MB/{Bytes2String(totalDownloadBytes)}MB");
    }

    void OnDownloadError(string fileName, string error)
    {
        XSFStartup.Instance.UI.ShowMessageBox(new MessageBoxDownloadError(this, 
                $"Download {fileName} error, message:{error}. Try again? "));
    }

    internal class MessageBoxDownload : IMessageBoxHandler
    {
        private StartupStepYooAssetUpdate StartupStep;

        private string Info;

        public MessageBoxDownload(StartupStepYooAssetUpdate ss, string info)
        {
            StartupStep = ss;
            Info = info;
        }

        public void OnOK()
        {
            StartupStep.Step = DownloadStep.DownloadFiles;
        }

        public void OnCancel()
        {

        }

        public string InfoText 
        { 
            get {
                return Info;
            } 
        }
    }

    internal class MessageBoxDownloadError : IMessageBoxHandler
    {
        private StartupStepYooAssetUpdate StartupStep;

        private string Info;

        public MessageBoxDownloadError(StartupStepYooAssetUpdate ss, string info)
        {
            StartupStep = ss;
            Info = info;
        }

        public void OnOK()
        {
            StartupStep.Step = DownloadStep.CreateDownloader;
        }

        public void OnCancel()
        {

        }

        public string InfoText 
        { 
            get {
                return Info;
            } 
        }
    }
}





