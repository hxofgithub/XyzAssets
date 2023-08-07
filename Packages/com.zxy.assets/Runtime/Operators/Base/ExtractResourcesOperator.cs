namespace XyzAssets.Runtime
{
    public abstract class ExtractResourcesOperator : AsyncOperationBase
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
