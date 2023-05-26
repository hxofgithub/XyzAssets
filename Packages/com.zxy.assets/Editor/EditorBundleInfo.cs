

using XyzAssets.Runtime;

namespace XyzAssets.Editor
{
    [System.Serializable]
    public class EditorBundleInfo
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
        /// 包裹名字
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
    public class EditorAssetInfo
    {
        public string AssetPath;

        public string MainBundle;

        /// <summary>
        /// 依赖
        /// </summary>
        public string[] Dependencies;
    }

}