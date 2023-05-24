namespace XyzAssets.Runtime
{
    public enum BundleFileNameType
    {
        BundleName,
        Hash,
    }

    public enum BundleEncryptType
    {
        FileOffset,
        Stream,
        None
    }


    [System.Serializable]
    internal class XyzAssetsRuntimeManifest
    {
        internal BundleInfo[] BundleList;
        internal AssetInfo[] AssetInfoList;
    }
}
