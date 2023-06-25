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
        XSF.LogWarning("StartupStepAASUpdate Start");
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

    List<string> catalogsToUpdate;

    private void StartUpdate()
    {
        XSF.Log("StartUpdate Start ..");
        catalogsToUpdate = new List<string>();
        
        checkForUpdateHandle = Addressables.CheckForCatalogUpdates(false);
        checkForUpdateHandle.Completed += op =>
        {
            catalogsToUpdate.AddRange(checkForUpdateHandle.Result);
            XSF.Log("AASUpdate checkForUpdateHandle.Completed count=" + checkForUpdateHandle.Result.Count);

            XSFCoroutine.Instance.StartCoroutine((int)CoroutineID.C0, AASUpdate());
        };
    }

    private AsyncOperationHandle<List<string>> checkForUpdateHandle;
    IEnumerator AASUpdate() {
        XSF.Log("AASUpdate start catalogsToUpdate.Count=" + catalogsToUpdate.Count + ", result=" + checkForUpdateHandle.Result);
        if (catalogsToUpdate.Count > 0) {
            AsyncOperationHandle<List<IResourceLocator>> updateHandle = Addressables.UpdateCatalogs(catalogsToUpdate, false);

            yield return updateHandle;

            XSF.Log($"AASUpdate updateHandle.Status = {updateHandle.Status}");
            if (updateHandle.Status == AsyncOperationStatus.Succeeded)
            {
                List<IResourceLocator> locators = updateHandle.Result;

                XSF.Log($"AASUpdate start locators count={locators.Count}");

                for(int i = 0; i < locators.Count; i ++)
                {
                    XSF.Log($"AASUpdate start download, keys={locators[i].LocatorId}");
                    AsyncOperationHandle<long> getDownloadSize = Addressables.GetDownloadSizeAsync(locators[i].Keys);
                    while(!getDownloadSize.IsDone)
                    {
                        XSF.Log("AASUpdate wait getDownloadSize");
                        yield return new WaitForSeconds(0.3f);
                    }

                    XSF.Log($"AASUpdate download size={getDownloadSize.Result}, status={getDownloadSize.Status}");

                    //If the download size is greater than 0, download all the dependencies.
                    if (getDownloadSize.Result > 0) {
                        AsyncOperationHandle downloadDependencies = Addressables.DownloadDependenciesAsync(locators[i].Keys);
                        while(!downloadDependencies.IsDone)
                        {
                            XSF.Log("AASUpdate wait downloadDependencies");
                            yield return new WaitForSeconds(0.3f);
                        }

                        XSF.Log($"AASUpdate download done, keys={locators[i].Keys}");
                        Addressables.Release(downloadDependencies);
                    }

                    Addressables.Release(getDownloadSize);
                }
            }

            Addressables.Release(updateHandle);
        }

        Addressables.Release(checkForUpdateHandle);

        XSF.Log("AASUpdate end");

        IsDone = true;
    }
}