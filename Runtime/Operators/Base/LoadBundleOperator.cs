using System;
using UnityEngine;

namespace XyzAssets.Runtime
{
    internal abstract class LoadBundleOperator : ResourceBaseOperator
    {
        public LoadBundleOperator(BundleInfo bundleInfo) : base(false)
        {
            m_BundleInfo = bundleInfo;
            XyzOperatorSystem.AddAssetOperator(this);
        }

        public event Action<LoadBundleOperator> OnComplete;

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

        public AssetBundle CachedBundle { get; protected set; }
        protected BundleInfo m_BundleInfo;
    }

}
