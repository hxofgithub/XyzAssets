namespace XyzAssets.Runtime
{
    internal sealed class SimulateUpdateManifestOperator : UpdateManifestOperator
    {
        protected override void OnDispose()
        {

        }

        protected override void OnExecute()
        {
            Progress = 1;
            Status = OperatorStatus.Success;
        }

        protected override void OnStart()
        {

        }

        public override void SaveManifest()
        {

        }

    }
}