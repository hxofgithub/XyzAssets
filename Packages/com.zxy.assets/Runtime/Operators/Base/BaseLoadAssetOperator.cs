using System;

namespace XyzAssets.Runtime
{
    public abstract class BaseLoadAssetOperator : ResourceBaseOperator
    {
        internal BaseLoadAssetOperator(AssetInfo assetInfo, System.Type type, bool asyncLoad)
        {
            m_AssetInfo = assetInfo;
            m_Type = type;
            m_AsyncLoad = asyncLoad;
        }

        public UnityEngine.Object AssetObject { get; protected set; }

        public event Action<BaseLoadAssetOperator> OnComplete;

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

        internal AssetInfo m_AssetInfo;

        protected Type m_Type;
        protected bool m_AsyncLoad;
    }

    public abstract class BaseLoadAllAssetsOperator : ResourceBaseOperator
    {
        public UnityEngine.Object[] AllAssetsObject { get; protected set; }

        public event Action<BaseLoadAllAssetsOperator> OnComplete;

        public override OperatorStatus Status
        {
            get => base.Status;
            protected set
            {
                base.Status = value;
                if (OnComplete != null && value == OperatorStatus.Success)
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
