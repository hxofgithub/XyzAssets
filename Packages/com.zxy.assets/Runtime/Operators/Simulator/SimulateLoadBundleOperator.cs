
namespace XyzAssets.Runtime
{
    internal sealed class SimulateLoadBundleOperator : LoadBundleOperator
    {
        internal SimulateLoadBundleOperator(BundleInfo bundleInfo) : base(bundleInfo)
        {

        }
        protected override void OnExecute()
        {
            Status = EOperatorStatus.Succeed;
        }
        protected override void OnDispose()
        {
        }

        protected override void OnStart()
        {

        }
    }
}