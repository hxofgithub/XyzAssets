
using UnityEngine;

namespace XyzAssets.Runtime
{
    internal abstract class DownloaderBase
    {
        public enum EStatus
        {
            None = 0,
            Succeed,
            Failed
        }
        public DownloaderBase(BundleInfo info, int retryTimes, int timeout)
        {
            _requestRetryTimes = retryTimes;
            _requestTimeout = timeout;
            BundleInfo = info;

            _tempFilePath = XyzAssetPathHelper.GetTempFilePath(info.BundleName);
        }
        internal BundleInfo BundleInfo { get; private set; }
        internal float Progress { get; set; } = 0;
        internal ulong DownloadedBytes { get; set; } = 0;

        internal string Error { get; set; }
        internal long ResponseCode { get; set; }

        public EStatus Status { get; set; }

        public bool IsDone { get { return Status == EStatus.Succeed || Status == EStatus.Failed; } }


        internal abstract void SendRequest();
        internal abstract void Update();
        internal abstract void Abort();

        internal abstract void Dispose();
        protected void CheckTimeout()
        {
            if (_isAbort)
                return;
            if (_lastDownloadedBytes != DownloadedBytes)
            {
                _lastDownloadedBytes = DownloadedBytes;
                _lastRequestRealtime = Time.realtimeSinceStartup;
            }

            if (Time.realtimeSinceStartup - _lastRequestRealtime > _requestTimeout)
            {
                Abort();
            }
        }

        /// <summary>
        /// 获取网络请求地址
        /// </summary>
        protected string GetRequestURL()
        {
            // 轮流返回请求地址
            _requestIndex++;
            var index = _requestIndex % DownloadSystem.ResUrls.Length;
            return string.Format(DownloadSystem.ResUrls[index], BundleInfo.BundleName);
        }

        protected void CachingFile(string tempFilePath)
        {

        }

        protected int _requestRetryTimes = 0;
        protected bool _isAbort = false;
        protected float _lastRequestRealtime = 0;
        protected int _requestTimeout = 0;
        protected ulong _lastDownloadedBytes;
        protected string _tempFilePath;
        protected int _requestIndex;

    }
}
