
using UnityEngine;

namespace XyzAssets.Runtime
{
    internal abstract class DownloaderBase : AsyncOperationBase
    {
        public DownloaderBase(BundleInfo info, int retryTimes, int timeout)
        {
            _requestRetryTimes = retryTimes;
            _requestTimeout = timeout;
            BundleInfo = info;

            _tempFilePath = AssetsPathHelper.GetTempFilePath(info.BundleName);
        }
        internal BundleInfo BundleInfo { get; private set; }
        internal ulong DownloadedBytes { get; set; } = 0;
        internal long ResponseCode { get; set; }

        protected abstract void Abort();
        internal abstract void SendRequest();

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


        protected int _requestRetryTimes = 0;
        protected bool _isAbort = false;
        protected float _lastRequestRealtime = 0;
        protected int _requestTimeout = 0;
        protected ulong _lastDownloadedBytes;
        protected string _tempFilePath;
        protected int _requestIndex;

    }
}
