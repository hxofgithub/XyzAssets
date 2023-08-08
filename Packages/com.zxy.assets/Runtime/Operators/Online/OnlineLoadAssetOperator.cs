using UnityEngine;

namespace XyzAssets.Runtime
{
    internal sealed class OnlineLoadAssetOperator : BaseLoadAssetOperator
    {
        internal OnlineLoadAssetOperator(OnlineAssetsSystemImpl impl, AssetInfo assetInfo, System.Type type, bool async) : base(assetInfo, type, async)
        {
            m_Impl = impl;
        }

        protected override void OnExecute()
        {
            if (m_LoadBundleOperator == null) return;

            Progress = m_LoadBundleOperator.Progress * 0.5f;

            if (!m_LoadBundleOperator.IsDone) return;

            if (m_LoadBundleOperator.Status == EOperatorStatus.Failed)
            {
                Error = m_LoadBundleOperator.Error;
                Status = m_LoadBundleOperator.Status;
            }
            else
            {
                if (m_AsyncLoad)
                {
                    if (m_AssetBundleRequest == null)
                        m_AssetBundleRequest = m_LoadBundleOperator.CachedBundle.LoadAssetAsync(System.IO.Path.GetFileName(m_AssetInfo.AssetPath), m_Type);
                    else
                    {
                        Progress = (m_AssetBundleRequest.progress + 1) * 0.5f;
                        if (!m_AssetBundleRequest.isDone) return;
                        if (m_AssetBundleRequest.asset == null)
                        {
                            Error = StringUtility.Format("Failed Load name:{0} type:{1} from Bundle:{2}", m_AssetInfo.AssetPath, m_Type.Name, m_LoadBundleOperator.CachedBundle.name);
                            Status = EOperatorStatus.Failed;
                        }
                        else
                        {
                            AssetObject = m_AssetBundleRequest.asset;
                            Status = EOperatorStatus.Succeed;
                        }
                    }
                }
                else
                {
                    AssetObject = m_LoadBundleOperator.CachedBundle.LoadAsset(System.IO.Path.GetFileName(m_AssetInfo.AssetPath), m_Type);
                    Progress = 1;
                    if (AssetObject == null)
                    {
                        Error = StringUtility.Format("Failed Load name:{0} type:{1} from Bundle:{2}", m_AssetInfo.AssetPath, m_Type.Name, m_LoadBundleOperator.CachedBundle.name);
                        Status = EOperatorStatus.Failed;
                    }
                    else
                    {
                        Status = EOperatorStatus.Succeed;
                    }
                }
            }
        }

        protected override void OnDispose()
        {
            if (m_LoadBundleOperator != null)
            {
                m_LoadBundleOperator = null;
            }
            m_AssetBundleRequest = null;


            AssetObject = null;

            if (m_AssetInfo != null)
            {
                m_Impl.UnLoadBundle(m_AssetInfo);
                m_AssetInfo = null;
            }


            m_Impl = null;
        }

        protected override void OnStart()
        {
            m_LoadBundleOperator = m_Impl.LoadBundle(m_AssetInfo);
        }

        private LoadBundleOperator m_LoadBundleOperator;
        private AssetBundleRequest m_AssetBundleRequest;
        private OnlineAssetsSystemImpl m_Impl;
    }
}