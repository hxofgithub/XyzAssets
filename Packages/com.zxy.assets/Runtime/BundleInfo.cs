

namespace XyzAssets.Runtime
{
    [System.Serializable]
    internal class BundleInfo
    {
        /// <summary>
        /// Bundle名字
        /// </summary>
        public string BundleName;

        /// <summary>
        /// 当前版本
        /// </summary>
        public string Version;
        /// <summary>
        /// Crc
        /// </summary>
        public uint Crc;
        /// <summary>
        /// 文件大小
        /// </summary>
        public long FileSize;
        /// <summary>
        /// 模块名字
        /// </summary>
        public string ModeName;

        /// <summary>
        /// 加密方式
        /// </summary>
        public BundleEncryptType EncryptType;

        /// <summary>
        /// 名字格式
        /// </summary>
        public BundleFileNameType NameType;

    }

    [System.Serializable]
    internal class AssetInfo
    {
        public string AssetPath;

        public int MainId;
        /// <summary>
        /// 依赖
        /// </summary>
        public int[] Dependencies;

    }

}