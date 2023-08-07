using System;

namespace XyzAssets.Runtime
{
    public abstract class BaseLoadAssetOperator : AsyncOperationBase
    {
        internal BaseLoadAssetOperator(AssetInfo assetInfo, System.Type type, bool asyncLoad)
        {
            m_AssetInfo = assetInfo;
            m_Type = type;
            m_AsyncLoad = asyncLoad;
        }

        public UnityEngine.Object AssetObject { get; protected set; }

        public event Action<BaseLoadAssetOperator> OnComplete;

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

        internal AssetInfo m_AssetInfo;

        protected Type m_Type;
        protected bool m_AsyncLoad;
    }

    public abstract class BaseLoadAllAssetsOperator : AsyncOperationBase
    {
        public UnityEngine.Object[] AllAssetsObject { get; protected set; }

        public event Action<BaseLoadAllAssetsOperator> OnComplete;

        public override EOperatorStatus Status
        {
            get => base.Status;
            protected set
            {
                base.Status = value;
                if (OnComplete != null && value == EOperatorStatus.Success)
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
