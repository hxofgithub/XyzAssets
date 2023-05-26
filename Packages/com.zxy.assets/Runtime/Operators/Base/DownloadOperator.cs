using System;

namespace XyzAssets.Runtime
{
    public abstract class DownloadOperator : ResourceBaseOperator
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

        public override OperatorStatus Status
        {
            get => base.Status;
            protected set
            {
                base.Status = value;
                if (OnComplete != null && value == OperatorStatus.Success && !m_IsDisposed)
                {
                    OnComplete(this);
                    OnComplete = null;
                }
                if (value == OperatorStatus.Failed)
                    XyzLogger.LogError(Error);
            }
        }
        public void BeginDownload()
        {
            XyzOperatorSystem.AddAssetOperator(this);
        }
    }
}
