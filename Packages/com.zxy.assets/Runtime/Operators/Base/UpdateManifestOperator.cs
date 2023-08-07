using System;

namespace XyzAssets.Runtime
{
    public abstract class UpdateManifestOperator : AsyncOperationBase
    {
        public event Action<UpdateManifestOperator> OnComplete;

        public override EOperatorStatus Status
        {
            get => base.Status;
            protected set
            {
                base.Status = value;
                if (OnComplete != null && value == EOperatorStatus.Success && !m_IsDisposed)
                {
                    OnComplete(this);
                    OnComplete = null;
                }
                if (value == EOperatorStatus.Failed)
                    XyzLogger.LogError(Error);
            }
        }

        public abstract void SaveManifest();
    }
}