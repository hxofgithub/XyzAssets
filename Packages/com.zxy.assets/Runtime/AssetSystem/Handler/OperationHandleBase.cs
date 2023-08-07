using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XyzAssets.Runtime
{

    public class OperationHandleBase : IEnumerator
    {
        public OperationHandleBase(ProviderBase provider)
        {
            Provider = provider;
        }

        public ProviderBase Provider { get; private set; }

        public bool IsDone
        {
            get
            {
                if (!IsProviderValidation) return false;
                return Provider.IsDone;
            }
        }

        public EOperatorStatus Status
        {
            get
            {
                if (IsProviderValidation == false)
                    return EOperatorStatus.None;
                if (Provider.Status == ProviderBase.EStatus.Failed)
                    return EOperatorStatus.None;
                else if (Provider.Status == ProviderBase.EStatus.Succeed)
                    return EOperatorStatus.Success;
                return EOperatorStatus.None;
            }
        }

        public string Error
        {
            get
            {
                if (IsProviderValidation == false) return string.Empty;
                return Provider.Error;
            }
        }

        public float Progress
        {
            get
            {
                if (IsProviderValidation == false) return 0;
                return Provider.Progress;
            }
        }


        internal bool IsProviderValidation
        {
            get
            {
                if(Provider != null && !Provider.IsDestroyed)
                {
                    return true;
                }
                else
                {
                    if (Provider == null)
                        XyzLogger.LogWarning("Operation handle is releaseed:");
                    else if (Provider.IsDestroyed)
                        XyzLogger.LogWarning("Provider is destroyed");
                    return false;
                }
            }
        }

        internal void ReleaseInternal()
        {
            if (IsProviderValidation == false) return;
            Provider.ReleaseHandle(this);
            Provider = null;
        }

        #region IEnumerator
        public System.Threading.Tasks.Task Task
        {
            get { return Provider.Task; }
        }
        bool IEnumerator.MoveNext()
        {
            return !Provider.IsDone;
        }
        void IEnumerator.Reset() { }

        object IEnumerator.Current
        {
            get { return Provider; }
        }

        #endregion
    }

}
