using UnityEngine;

namespace XyzAssets.Runtime
{
    internal sealed class OnlineLoadAssetOperator : BaseLoadAssetOperator
    {
        internal OnlineLoadAssetOperator(OnlineSystemImpl impl, AssetInfo assetInfo, System.Type type, bool async) : base(assetInfo, type, async)
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
                        m_AssetBundleRequest = BundleSystem.GetAssetBundle(m_LoadBundleOperator.MainBundleId).LoadAssetAsync(System.IO.Path.GetFileName(m_AssetInfo.AssetPath), m_Type);
                    else
                    {
                        Progress = (m_AssetBundleRequest.progress + 1) * 0.5f;
                        if (!m_AssetBundleRequest.isDone) return;
                        if (m_AssetBundleRequest.asset == null)
                        {
                            Error = StringUtility.Format("Failed Load name:{0} type:{1} from Bundle:{2}", m_AssetInfo.AssetPath, m_Type.Name, m_LoadBundleOperator.MainBundleId);
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
                    AssetObject = BundleSystem.GetAssetBundle(m_LoadBundleOperator.MainBundleId).LoadAsset(System.IO.Path.GetFileName(m_AssetInfo.AssetPath), m_Type);
                    Progress = 1;
                    if (AssetObject == null)
                    {
                        Error = StringUtility.Format("Failed Load name:{0} type:{1} from Bundle:{2}", m_AssetInfo.AssetPath, m_Type.Name, m_LoadBundleOperator.MainBundleId);
                        Status = EOperatorStatus.Failed;
                    }
                    else
                    {
                        Status = EOperatorStatus.Succeed;
                    }
                }
            }
        }
        protected override void OnStart()
        {
            m_LoadBundleOperator = m_Impl.LoadBundle(m_AssetInfo.MainId);
        }

        private LoadBundleOperator m_LoadBundleOperator;
        private AssetBundleRequest m_AssetBundleRequest;
        private OnlineSystemImpl m_Impl;
    }
}