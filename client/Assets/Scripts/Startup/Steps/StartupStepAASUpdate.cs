//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\Scripts\Startup\Steps\StartupStepAASUpdate.cs
// 作者：Xoen Xie
// 时间：2023/06/24
// 描述：游戏启动 - AAS Update
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

using System.Collections;
using System.Collections.Generic;

public sealed class StartupStepAASUpdate : StartupStep
{
    public override void Start()
    {
#if UNITY_EDITOR
        if (XSFConfig.Instance.AASUpdateOpen)
        {
#endif
            XSF.Log("StartupStepAASUpdate Start ..");

            StartUpdate();
#if UNITY_EDITOR
        }
        else
        {
            XSF.Log("StartupStepAASUpdate skip ..");
            IsDone = true;
        }
#endif
    }

    private void StartUpdate()
    {
        XSFCoroutine.Instance.StartCoroutine((int)CoroutineID.C0, AASUpdate());
    }

    IEnumerator AASUpdate() {
        XSF.Log("AASUpdate start");
        List<string> catalogsToUpdate = new List<string>();
        AsyncOperationHandle<List<string>> checkForUpdateHandle = Addressables.CheckForCatalogUpdates();
        checkForUpdateHandle.Completed += op =>
        {
            catalogsToUpdate.AddRange(op.Result);
        };

        yield return checkForUpdateHandle;

        XSF.Log("AASUpdate start catalogsToUpdate.Count=" + catalogsToUpdate.Count);
        if (catalogsToUpdate.Count > 0) {
            AsyncOperationHandle<List<IResourceLocator>> updateHandle = Addressables.UpdateCatalogs(catalogsToUpdate, false);
            yield return updateHandle;

            XSF.Log($"updateHandle.Status = {updateHandle.Status}");
            if (updateHandle.Status == AsyncOperationStatus.Succeeded)
            {
                List<IResourceLocator> locators = updateHandle.Result;

                for(int i = 0; i < locators.Count; i ++)
                {
                    XSF.Log($"AASUpdate start download, keys={locators[i].Keys}");
                    AsyncOperationHandle<long> getDownloadSize = Addressables.GetDownloadSizeAsync(locators[i].Keys);
                    yield return getDownloadSize;

                    XSF.Log($"AASUpdate download size={getDownloadSize.Result}");

                    //If the download size is greater than 0, download all the dependencies.
                    if (getDownloadSize.Result > 0) {
                        AsyncOperationHandle downloadDependencies = Addressables.DownloadDependenciesAsync(locators[i].Keys);
                        yield return downloadDependencies;

                        XSF.Log($"AASUpdate download done, keys={locators[i].Keys}");
                        Addressables.Release(downloadDependencies);
                    }
                }
            }

            Addressables.Release(updateHandle);
        }

        Addressables.Release(checkForUpdateHandle);

        XSF.Log("AASUpdate end");

        IsDone = true;
    }
}