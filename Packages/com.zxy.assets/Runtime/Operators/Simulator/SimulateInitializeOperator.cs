namespace XyzAssets.Runtime
{
    public sealed class SimulateInitializeParameters : InitializeParameters
    {

    }
    internal sealed class SimulateInitializeOperator : InitializeOperator
    {
        protected override void OnExecute()
        {
            Progress = 1;
            Status = EOperatorStatus.Succeed;
        }

        protected override void OnStart()
        {

        }

        protected override void OnDispose()
        {

        }

    }
}