using System;

namespace XyzAssets.Runtime
{
    public abstract class InitializeParameters
    {

    }
    public abstract class InitializeOperator : AsyncOperationBase
    {
        public event Action OnComplete;

        protected override void InvokeCompletion()
        {
            if (Status == EOperatorStatus.Succeed)
            {
                OnComplete?.Invoke();
                OnComplete = null;
            }
            else
                XyzLogger.LogError(Error);
        }
    }
}