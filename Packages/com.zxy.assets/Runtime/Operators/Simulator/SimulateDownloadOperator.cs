namespace XyzAssets.Runtime
{
    internal sealed class SimulateDownloadOperator : DownloadOperator
    {
        protected override void OnDispose()
        {

        }

        protected override void OnExecute()
        {
            Status = OperatorStatus.Success;
        }

        protected override void OnStart()
        {

        }
    }
}