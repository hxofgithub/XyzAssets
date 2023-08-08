#if UNITY_EDITOR

#endif
namespace XyzAssets.Runtime
{
    internal sealed class SimulateExtractResourcesOperator : ExtractResourcesOperator
    {
        internal SimulateExtractResourcesOperator() : base(null, null)
        {

        }
        protected override void OnStart()
        {

        }

        protected override void OnExecute()
        {
            Status = EOperatorStatus.Succeed;
        }

        protected override void OnDispose()
        {

        }
    }

}
