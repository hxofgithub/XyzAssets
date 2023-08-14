namespace XyzAssets.Runtime
{
    [System.Serializable]
    public class AssetInfo
    {
        public string AssetPath;

        public int MainId;
        /// <summary>
        /// 依赖
        /// </summary>
        public int[] Dependencies;

    }
}
