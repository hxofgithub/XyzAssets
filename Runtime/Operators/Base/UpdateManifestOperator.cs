using System;

namespace XyzAssets.Runtime
{
    public abstract class UpdateManifestOperator : ResourceBaseOperator
    {
        public event Action<UpdateManifestOperator> OnComplete;

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

        public abstract void SaveManifest();
    }
}