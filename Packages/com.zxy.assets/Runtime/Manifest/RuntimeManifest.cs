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
    internal class RuntimeManifest
    {
        internal BundleInfo[] BundleList;
        internal AssetInfo[] AssetInfoList;
    }
}
