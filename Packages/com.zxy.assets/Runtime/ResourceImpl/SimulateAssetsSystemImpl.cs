
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace XyzAssets.Runtime
{
    internal class SimulateAssetsSystemImpl : IAssetsSystemImpl
    {
        DownloadOperator IAssetsSystemImpl.CreateDownloaderByModes(string[] modeNames)
        {
            return new SimulateDownloadOperator();
        }

        void IAssetsSystemImpl.Dispose()
        {
            XyzLogger.Log("SimulateAssetsSystemImpl Dispose");
        }

        ExtractResourcesOperator IAssetsSystemImpl.ExtractResources(string extractPath, string[] modeNames)
        {
            return new SimulateExtractResourcesOperator();
        }

        InitializeOperator IAssetsSystemImpl.Initialize(InitializeParameters initializeParameters)
        {
            return new SimulateInitializeOperator();
        }


        BaseLoadAssetOperator IAssetsSystemImpl.LoadAssetAsync<T>(string assetPath)
        {
            var assetInfo = new AssetInfo() { AssetPath = assetPath };
            return new SimulateLoadAssetOperator(assetInfo, typeof(T), true);
        }

        BaseLoadAssetOperator IAssetsSystemImpl.LoadAssetAsync(string assetPath, Type type)
        {
            var assetInfo = new AssetInfo() { AssetPath = assetPath };
            return new SimulateLoadAssetOperator(assetInfo, type, true);
        }

        BaseLoadAllAssetsOperator IAssetsSystemImpl.LoadAssetsAsync(List<KeyValuePair<string, Type>> valuePairs)
        {
            return new SimulateLoadAllSubAssetsOperator(valuePairs);
        }

        BaseLoadAllAssetsOperator IAssetsSystemImpl.LoadAssetsAsync<T>(IEnumerable<string> assetPaths)
        {
            return new SimulateLoadAllSubAssetsOperator(assetPaths, typeof(T));
        }

        LoadSceneOperator IAssetsSystemImpl.LoadSceneOperator(string scenePath, LoadSceneMode sceneMode)
        {
            return new SimulateLoadSceneOperator(new AssetInfo() { AssetPath = scenePath }, sceneMode);
        }
        UpdateManifestOperator IAssetsSystemImpl.UpdateManifest()
        {
            return new SimulateUpdateManifestOperator();
        }
    }
}
