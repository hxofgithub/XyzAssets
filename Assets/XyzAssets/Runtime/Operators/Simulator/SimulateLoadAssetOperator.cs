
namespace XyzAssets.Runtime
{
    internal sealed class SimulateLoadAssetOperator : BaseLoadAssetOperator
    {
        internal SimulateLoadAssetOperator(AssetInfo assetInfo, System.Type type, bool async) : base(assetInfo, type, async)
        {

        }

        protected override void OnExecute()
        {
#if UNITY_EDITOR
            AssetObject = UnityEditor.AssetDatabase.LoadAssetAtPath(m_AssetInfo.AssetPath, m_Type);
            if (AssetObject == null)
            {
                Error = StringUtility.Format("Asset Load Error: {0}", m_AssetInfo.AssetPath);
                Status = OperatorStatus.Failed;
            }
            else
                Status = OperatorStatus.Success;

#endif
        }

        protected override void OnDispose()
        {
            AssetObject = null;
        }

        protected override void OnStart()
        {

        }
    }
}