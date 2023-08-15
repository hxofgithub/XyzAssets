namespace XyzAssets.Runtime
{
    internal sealed class SimulateUpdateManifestOperator : UpdateManifestOperator
    {

        protected override void OnExecute()
        {
            Progress = 1;
            Status = EOperatorStatus.Succeed;
        }

        protected override void OnStart()
        {

        }

        public override void SaveManifest()
        {

        }

    }
}