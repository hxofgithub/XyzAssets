using System;

namespace XyzAssets.Runtime
{
    internal abstract class LoadBundleOperator : AsyncOperationBase
    {
        internal int MainBundleId { get; }

        protected bool _mAsync;

        public LoadBundleOperator(int bundleId, bool async = true) : base(false)
        {
            MainBundleId = bundleId;
            _mAsync = async;
            OperatorSystem.AddAssetOperator(this);
        }

        public event Action<LoadBundleOperator> OnComplete;
        protected override void InvokeCompletion()
        {
            if (Status == EOperatorStatus.Succeed)
            {
                OnComplete?.Invoke(this);
                OnComplete = null;
            }
            else
                XyzLogger.LogError(Error);
        }

    }

}
