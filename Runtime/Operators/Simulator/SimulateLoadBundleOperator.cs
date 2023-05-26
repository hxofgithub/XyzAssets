
namespace XyzAssets.Runtime
{
    internal sealed class SimulateLoadBundleOperator : LoadBundleOperator
    {
        internal SimulateLoadBundleOperator(BundleInfo bundleInfo) : base(bundleInfo)
        {

        }
        protected override void OnExecute()
        {
            Status = OperatorStatus.Success;
        }
        protected override void OnDispose()
        {
        }

        protected override void OnStart()
        {

        }
    }
}