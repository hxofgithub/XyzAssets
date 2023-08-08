using UnityEngine;
using UnityEngine.Networking;

namespace XyzAssets.Runtime
{
    internal class FileResumeDownloader : DownloaderBase
    {
        private enum EStep
        {
            None,
            CheckTempFile,
            WaitingCheckTempFile,
            PrepareDownload,
            CreateDownloader,
            CheckDownload,
            VerifyTempFile,
            WaitingVerifyTempFile,
            CachingFile,
            TryAgain,
            Done,
        }

        private EStep _step = EStep.None;
        public FileResumeDownloader(BundleInfo info, int retryTimes, int timeout) : base(info, retryTimes, timeout)
        {

        }

        internal override void SendRequest()
        {
            _step = EStep.CheckTempFile;
        }

        internal override void Update()
        {
            if (_step == EStep.None)
                return;

            if (IsDone) return;


            if (_step == EStep.CheckTempFile)
            {
                //检测已下载的文件
                _verifyOpera = VerifyTempOpeation.Create(_tempFilePath, BundleInfo.Version, BundleInfo.FileSize);
                _step = EStep.WaitingCheckTempFile;
            }
            else if (_step == EStep.WaitingCheckTempFile)
            {
                //等待检测完成
                if (!_verifyOpera.IsDone) return;


                if (_verifyOpera.Status == EOperatorStatus.Succeed)
                {
                    //如果缓存文件是正确的
                    _fileOrgLength = _verifyOpera.FileSize;

                    //设置下载长度
                    DownloadedBytes = (ulong)_fileOrgLength;
                    //设置进度
                    Progress = 1f;

                    ResponseCode = 0;


                    _step = EStep.CachingFile;
                }
                else if (_verifyOpera.Status == EOperatorStatus.Failed)
                {
                    //如果缓存文件过大或者错误， 应该删除文件
                    if (_verifyOpera.Result == EVerifyResult.FileOverflow || _verifyOpera.Result == EVerifyResult.FileCrcError)
                        CachingSystem.TryDiscardTempFile(_tempFilePath);

                    _fileOrgLength = _verifyOpera.FileSize;

                    _step = EStep.PrepareDownload;
                }
                _verifyOpera = null;
            }
            else if (_step == EStep.PrepareDownload)
            {
                //重置数据

                _isAbort = false;
                DownloadedBytes = 0;
                Progress = 0;
                ResponseCode = 0;
                _tryAgainTimer = 0;
                _lastRequestRealtime = Time.realtimeSinceStartup;

                _step = EStep.CreateDownloader;

            }
            else if (_step == EStep.CreateDownloader)
            {
                _webRequest = UnityWebRequest.Get(GetRequestURL());
                var handler = new DownloadHandlerFile(_tempFilePath, true);
                handler.removeFileOnAbort = true;

                _webRequest.downloadHandler = handler;
                _webRequest.disposeDownloadHandlerOnDispose = true;
                if (_fileOrgLength > 0)
                    _webRequest.SetRequestHeader("Range", $"bytes={_fileOrgLength}-");
                _webRequest.SendWebRequest();
                _step = EStep.CheckDownload;
            }
            else if (_step == EStep.CheckDownload)
            {
                Progress = _webRequest.downloadProgress;
                DownloadedBytes = _webRequest.downloadedBytes + (ulong)_fileOrgLength;
                if (!_webRequest.isDone)
                {
                    CheckTimeout();
                    return;
                }

                bool hasError = false;

                // 检查网络错误
#if UNITY_2020_3_OR_NEWER
                if (_webRequest.result != UnityWebRequest.Result.Success)
                {
                    hasError = true;
                    Error = _webRequest.error;
                    ResponseCode = _webRequest.responseCode;
                }
#else
				if (_webRequest.isNetworkError || _webRequest.isHttpError)
				{
					hasError = true;
					_lastError = _webRequest.error;
					_lastCode = _webRequest.responseCode;
				}
#endif
                if (hasError)
                {
                    if (DownloadSystem.ClearFileResponseCodes != null)
                    {
                        if (DownloadSystem.ClearFileResponseCodes.Contains(ResponseCode))
                            CachingSystem.TryDiscardTempFile(_tempFilePath);
                    }
                    _step = EStep.TryAgain;
                }
                else
                {
                    _step = EStep.VerifyTempFile;
                }

            }
            else if (_step == EStep.VerifyTempFile)
            {
                _verifyOpera = VerifyTempOpeation.Create(_tempFilePath, BundleInfo.Version, BundleInfo.FileSize);
                _step = EStep.WaitingVerifyTempFile;
            }
            else if (_step == EStep.WaitingVerifyTempFile)
            {
                if (!_verifyOpera.IsDone) return;
                if (_verifyOpera.Status == EOperatorStatus.Succeed)
                {
                    _step = EStep.CachingFile;
                    _verifyOpera = null;
                }
                else
                {
                    CachingSystem.TryDiscardTempFile(_tempFilePath);
                    Error = _verifyOpera.Error;
                    _step = EStep.TryAgain;
                }
            }
            else if (_step == EStep.CachingFile)
            {
                try
                {
                    CachingFile(_tempFilePath);
                    Status = EStatus.Succeed;
                    _step = EStep.Done;
                    Error = "";
                    ResponseCode = 0;
                }
                catch (System.Exception e)
                {
                    _step = EStep.TryAgain;
                    Error = e.Message;
                }
            }
            else if (_step == EStep.TryAgain)
            {
                if (_requestRetryTimes <= 0)
                {
                    Status = EStatus.Failed;
                    return;
                }
                _tryAgainTimer += Time.unscaledDeltaTime;
                if (_tryAgainTimer > 1f)
                {
                    _requestRetryTimes--;
                    _step = EStep.CheckTempFile;
                }
            }
        }

        internal override void Abort()
        {
            if (IsDone) return;
            _isAbort = true;
            if (_webRequest != null)
                _webRequest.Abort();
            _webRequest = null;
        }

        internal override void Dispose()
        {
            if (_webRequest != null)
            {
                _webRequest.Dispose();
                _webRequest = null;
            }
        }

        private VerifyTempOpeation _verifyOpera;
        private UnityWebRequest _webRequest;
        private long _fileOrgLength;
        private float _tryAgainTimer = 0;
    }

}
