using System.IO;
using UnityEngine.Networking;
namespace XyzAssets.Runtime
{
    internal sealed class OnlineDownloadOperator : DownloadOperator
    {
        private enum DownloadStep
        {
            None,
            Begin,
            Downloading,
            Verify,
            Completed,
        }

        internal OnlineDownloadOperator(OnlineAssetsSystemImpl impl, BundleInfo[] downloadInfos)
        {
            m_BundleInfos = downloadInfos;
            m_Impl = impl;

            if (downloadInfos != null)
            {
                TotalDownloadCnt = m_BundleInfos.Length;
                m_CurrentDownloadedBytes = 0;
                for (int i = 0; i < m_BundleInfos.Length; i++)
                {
                    TotalDownloadBytes += m_BundleInfos[i].FileSize;
                }
            }
        }
        protected override void OnStart()
        {
            m_CurrentResUrlIndex = m_CurrentBundleInfoIndex = 0;
            m_CurrentDownloadedBytes = 0;
            if (m_BundleInfos == null || m_BundleInfos.Length == 0)
            {
                Status = EOperatorStatus.Succeed;
            }
            else
            {
                m_ResUrls = m_Impl.GetModeService().ResUrls;
                m_MaxRetryTimes = m_Impl.GetModeService().MaxRetryTimes;
                m_Step = DownloadStep.Begin;
            }
        }
        protected override void OnExecute()
        {

            if (m_Step == DownloadStep.Begin)
            {
                var bundleInfo = m_BundleInfos[m_CurrentBundleInfoIndex];
                var bundleName = bundleInfo.NameType == BundleFileNameType.Hash ? bundleInfo.Version : bundleInfo.BundleName;

                m_WebRequest = UnityWebRequest.Get($"{System.IO.Path.Combine(m_ResUrls[m_CurrentResUrlIndex], bundleName)}?v={System.DateTime.UtcNow.ToString("yyyyHHmmhhMMss")}");
                m_WebRequest.disposeDownloadHandlerOnDispose = true;
                m_WebRequest.SendWebRequest();

                m_Step = DownloadStep.Downloading;

            }
            else if (m_Step == DownloadStep.Downloading)
            {
                Progress = (m_WebRequest.downloadProgress + m_CurrentBundleInfoIndex) / TotalDownloadCnt;
                m_OnDownloadProgressChanged?.Invoke(m_CurrentBundleInfoIndex, (long)(m_CurrentDownloadedBytes + m_WebRequest.downloadedBytes), TotalDownloadCnt, TotalDownloadBytes);

                if (!m_WebRequest.isDone) return;
                m_CurrentDownloadedBytes += m_WebRequest.downloadedBytes;
                m_Step = DownloadStep.Verify;
            }
            else if (m_Step == DownloadStep.Verify)
            {
                if (string.IsNullOrEmpty(m_WebRequest.error))
                {
                    var bundleInfo = m_BundleInfos[m_CurrentBundleInfoIndex];
                    var hash = XyzAssetUtils.CalculateMD5(m_WebRequest.downloadHandler.data);
                    var bundleName = bundleInfo.NameType == BundleFileNameType.Hash ? bundleInfo.Version : bundleInfo.BundleName;
                    if (bundleInfo.Version != hash)
                    {
                        m_WebRequest.Dispose();
                        m_WebRequest = null;

                        Error = StringUtility.Format("Downloaded file:{0} hash is error.", bundleName);
                        Status = EOperatorStatus.Failed;
                    }
                    else
                    {
                        m_RetryTimes = 0;
                        var extenalPath = XyzAssetPathHelper.GetFileExternalPath(bundleName);
                        var direName = Path.GetDirectoryName(extenalPath);

                        if (!Directory.Exists(direName))
                            Directory.CreateDirectory(direName);
                        File.WriteAllBytes(extenalPath, m_WebRequest.downloadHandler.data);
                        m_CurrentBundleInfoIndex++;

                        m_WebRequest.Dispose();
                        m_WebRequest = null;

                        if (m_CurrentBundleInfoIndex >= TotalDownloadCnt)
                            m_Step = DownloadStep.Completed;
                        else
                            m_Step = DownloadStep.Begin;
                    }

                }
                else
                {
                    if (++m_RetryTimes >= m_MaxRetryTimes)
                    {
                        Error = StringUtility.Format("Download Bundle Error: {0}", m_WebRequest.error);
                        Status = EOperatorStatus.Failed;
                        return;
                    }

                    m_CurrentResUrlIndex++;
                    m_CurrentResUrlIndex %= m_ResUrls.Length;

                    m_WebRequest.Dispose();
                    m_WebRequest = null;
                    m_Step = DownloadStep.Begin;
                }
            }
            else if (m_Step == DownloadStep.Completed)
            {
                Status = EOperatorStatus.Succeed;
            }

        }
        protected override void OnDispose()
        {
            if (m_WebRequest != null)
            {
                m_WebRequest.Abort();
                m_WebRequest.Dispose();
                m_WebRequest = null;
            }
            m_ResUrls = null;
            m_BundleInfos = null;
            m_Impl = null;
            m_Step = DownloadStep.None;
        }

        private BundleInfo[] m_BundleInfos;
        private int m_CurrentResUrlIndex, m_RetryTimes;
        private int m_CurrentBundleInfoIndex;
        private UnityWebRequest m_WebRequest;
        private OnlineAssetsSystemImpl m_Impl;
        private ulong m_CurrentDownloadedBytes;
        private string[] m_ResUrls;
        private int m_MaxRetryTimes;
        private DownloadStep m_Step;
    }
}