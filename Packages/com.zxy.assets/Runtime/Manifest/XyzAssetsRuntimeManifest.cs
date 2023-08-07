namespace XyzAssets.Runtime
{
    public enum BundleFileNameType : byte
    {
        BundleName,
        Hash,
    }

    public enum BundleEncryptType : byte
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
