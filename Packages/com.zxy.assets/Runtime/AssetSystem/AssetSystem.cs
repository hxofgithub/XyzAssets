using System;
using System.Collections.Generic;
using UnityEngine;

namespace XyzAssets.Runtime
{
    internal static class AssetSystem
    {
        #region Load assets
        internal static void LoadAssetAsync<T>(string assetPath) where T : UnityEngine.Object
        {

        }

        internal static void LoadAssetAsync(string assetPath, Type type)
        {

        }

        internal static void LoadSceneAsync(string assetPath)
        {
        }

        internal static void LoadAssetsAsync<T>(IEnumerable<string> collection) where T : UnityEngine.Object
        {

        }

        internal static void LoadAssetsAsync(List<KeyValuePair<string, Type>> valuePairs)
        {

        }
        #endregion

        internal static AssetInfo GetAssetInfo(string assetPath) { return null; }
        internal static BundleInfo GetBundleInfo(int bundleId) { return null; }
        internal static BundleInfo[] GetBundleInfos(string[] modeNames) { return null; }

    }
}