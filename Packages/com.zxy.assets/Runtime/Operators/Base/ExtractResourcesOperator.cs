namespace XyzAssets.Runtime
{
    public abstract class ExtractResourcesOperator : ResourceBaseOperator
    {
        internal ExtractResourcesOperator(string extractPath, BundleInfo[] bundleInfos)
        {
            m_BundleInfos = bundleInfos;
            m_ExtractRootPath = extractPath;
        }

        protected string m_ExtractRootPath;
        internal BundleInfo[] m_BundleInfos;
    }
}
