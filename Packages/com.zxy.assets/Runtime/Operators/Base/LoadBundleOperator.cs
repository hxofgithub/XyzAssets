using System;
using UnityEngine;

namespace XyzAssets.Runtime
{
    internal abstract class LoadBundleOperator : AsyncOperationBase
    {
        public LoadBundleOperator(BundleInfo bundleInfo) : base(false)
        {
            m_BundleInfo = bundleInfo;
            OperatorSystem.AddAssetOperator(this);
        }

        public event Action<LoadBundleOperator> OnComplete;
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
        public AssetBundle CachedBundle { get; protected set; }
        protected BundleInfo m_BundleInfo;
    }

}
