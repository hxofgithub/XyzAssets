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

        protected override void InvokeCompletion()
        {
            if (Status == EOperatorStatus.Succeed)
            {
                OnComplete?.Invoke(this);
                OnComplete = null;
            }
            else
                XyzLogger.LogError(Error);
        }

        internal AssetInfo m_AssetInfo;

        protected Type m_Type;
        protected bool m_AsyncLoad;
    }

    public abstract class BaseLoadAllAssetsOperator : AsyncOperationBase
    {
        public UnityEngine.Object[] AllAssetsObject { get; protected set; }

        public event Action<BaseLoadAllAssetsOperator> OnComplete;
        protected override void InvokeCompletion()
        {
            if (Status == EOperatorStatus.Succeed)
            {
                OnComplete?.Invoke(this);
                OnComplete = null;
            }
            else
                XyzLogger.LogError(Error);
        }
    }

}
