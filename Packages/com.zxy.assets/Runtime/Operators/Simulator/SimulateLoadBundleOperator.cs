
namespace XyzAssets.Runtime
{
    internal sealed class SimulateLoadBundleOperator : LoadBundleOperator
    {
        internal SimulateLoadBundleOperator(int bundleInfo) : base(bundleInfo)
        {

        }
        protected override void OnExecute()
        {
            Status = EOperatorStatus.Succeed;
        }

        protected override void OnStart()
        {

        }
    }
}