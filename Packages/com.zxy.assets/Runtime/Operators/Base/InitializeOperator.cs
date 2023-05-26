using System;

namespace XyzAssets.Runtime
{
    public abstract class InitializeParameters
    {
        public IPlayModeService PlayModeService;
        public IGameDecryptService DecryptService;
    }
    public abstract class InitializeOperator : ResourceBaseOperator
    {
        public event Action<InitializeOperator> OnComplete;

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
    }
}