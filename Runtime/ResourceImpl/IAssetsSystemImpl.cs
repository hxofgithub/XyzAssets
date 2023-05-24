using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
namespace XyzAssets.Runtime
{
    internal interface IAssetsSystemImpl
    {
        InitializeOperator Initialize(InitializeParameters initializeParameters);
        UpdateManifestOperator UpdateManifest();
        DownloadOperator CreateDownloaderByModes(string[] modeNames);
        LoadSceneOperator LoadSceneOperator(string scenePath, LoadSceneMode sceneMode);
        BaseLoadAssetOperator LoadAssetAsync<T>(string assetPath);
        BaseLoadAssetOperator LoadAssetAsync(string assetPath, Type type);
        BaseLoadAllAssetsOperator LoadAssetsAsync(List<KeyValuePair<string, Type>> valuePairs);
        BaseLoadAllAssetsOperator LoadAssetsAsync<T>(IEnumerable<string> assetPaths);

        ExtractResourcesOperator ExtractResources(string extractPath, string[] modeNames);

        void SetBundleWrapFunc(Func<string, string> wrapFunc);

        void Dispose();
    }
}
