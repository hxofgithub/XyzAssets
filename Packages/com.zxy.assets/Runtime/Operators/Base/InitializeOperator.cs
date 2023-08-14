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

    }
}