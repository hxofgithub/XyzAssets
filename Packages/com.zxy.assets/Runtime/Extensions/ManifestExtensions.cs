using System.Collections.Generic;
namespace XyzAssets.Runtime
{
    public static class ManifestExtensions
    {
        public static void ConnectHandlerToObject<T>(this AsyncOperationBase handler, T target) where T : UnityEngine.Object
        {
            ThrowIfNull(handler);
            ThrowIfNull(target);
            //WeakRefSystem.AddWeakReference(target, handler);
        }

        internal static AssetInfo GetAssetInfo(this RuntimeManifest manifest, string assetPath)
        {
            ThrowIfNull(manifest);
            ThrowIfNull(manifest.AssetInfoList);
            for (int i = 0; i < manifest.AssetInfoList.Length; i++)
            {
                var assetInfo = manifest.AssetInfoList[i];
                if (assetInfo.AssetPath == assetPath)
                    return assetInfo;
            }
            return null;
        }
        internal static BundleInfo GetBundleInfo(this RuntimeManifest manifest, string bundleName, out int bundleId)
        {
            ThrowIfNull(manifest);
            ThrowIfNull(manifest.BundleList);
            bundleId = -1;
            for (int i = 0; i < manifest.BundleList.Length; i++)
            {
                var assetInfo = manifest.BundleList[i];
                if (assetInfo.BundleName == bundleName)
                {
                    bundleId = i;
                    return assetInfo;
                }
            }
            return null;
        }
        internal static BundleInfo GetBundleInfo(this RuntimeManifest manifest, int bundleId)
        {
            ThrowIfNull(manifest);
            ThrowIfNull(manifest.BundleList);
            return manifest.BundleList[bundleId];
        }
        internal static BundleInfo[] GetModeBundleInfos(this RuntimeManifest manifest, string[] modeNames)
        {
            ThrowIfNull(manifest);
            ThrowIfNull(manifest.BundleList);
            ThrowIfNull(modeNames);
            if (modeNames.Length == 0) return null;
            List<BundleInfo> result = new List<BundleInfo>();
            for (int i = 0; i < manifest.BundleList.Length; i++)
            {
                var bundleInfo = manifest.BundleList[i];
                if (System.Array.IndexOf(modeNames, bundleInfo.ModeName) != -1)
                    result.Add(bundleInfo);
            }
            return result.ToArray();
        }

        internal static int GetBundleId(this RuntimeManifest manifest, BundleInfo bundleInfo)
        {
            ThrowIfNull(manifest);
            ThrowIfNull(manifest.BundleList);
            ThrowIfNull(bundleInfo);
            for (int i = 0; i < manifest.BundleList.Length; i++)
            {
                var assetInfo = manifest.BundleList[i];
                if (assetInfo == bundleInfo)
                {
                    return i;
                }
            }
            return -1;
        }

        private static void ThrowIfNull<T>(T arg)
        {
            if (arg == null)
                throw new System.ArgumentNullException(typeof(T).Name);
        }
    }

}
