using System;

namespace XyzAssets.Runtime
{
    public abstract class DownloadOperator : AsyncOperationBase
    {
        public DownloadOperator() : base(false)
        {

        }

        public delegate void OnDownloadProgressChanged(int currentDownloadedCnt, long currentDownloadedBytes, int totalDownloadCnt, long totalDownloadBytes);

        public event OnDownloadProgressChanged DownloadProgressChanged
        {
            add => m_OnDownloadProgressChanged += value;
            remove => m_OnDownloadProgressChanged -= value;
        }

        public int TotalDownloadCnt { get; protected set; }
        public long TotalDownloadBytes { get; protected set; }

        protected OnDownloadProgressChanged m_OnDownloadProgressChanged;

        public event Action<DownloadOperator> OnComplete;

        protected override void InvokeCompletion()
        {
            if (Status == EOperatorStatus.Succeed)
            {
                OnComplete(this);
                OnComplete = null;
            }
            else
                XyzLogger.LogError(Error);
        }

        public void BeginDownload()
        {
            OperatorSystem.AddAssetOperator(this);
        }
    }
}
