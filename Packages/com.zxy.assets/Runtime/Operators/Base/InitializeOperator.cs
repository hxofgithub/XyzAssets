using System;

namespace XyzAssets.Runtime
{
    public abstract class InitializeParameters
    {
        public IPlayModeService PlayModeService;
        public IGameDecryptService DecryptService;
    }
    public abstract class InitializeOperator : AsyncOperationBase
    {
        public event Action<InitializeOperator> OnComplete;

        public override EOperatorStatus Status
        {
            get => base.Status;
            protected set
            {
                base.Status = value;
                if (OnComplete != null && value == EOperatorStatus.Succeed && !m_IsDisposed)
                {
                    OnComplete(this);
                    OnComplete = null;
                }
                if (value == EOperatorStatus.Failed)
                    XyzLogger.LogError(Error);
            }
        }
    }
}