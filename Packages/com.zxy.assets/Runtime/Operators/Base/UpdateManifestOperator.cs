using System;

namespace XyzAssets.Runtime
{
    public abstract class UpdateManifestOperator : AsyncOperationBase
    {
        public event Action<UpdateManifestOperator> OnComplete;

        protected override void InvokeCompletion()
        {
            if(Status == EOperatorStatus.Succeed)
            {
                OnComplete(this);
                OnComplete = null;
            }
            else
                XyzLogger.LogError(Error);
        }

        public abstract void SaveManifest();
    }
}