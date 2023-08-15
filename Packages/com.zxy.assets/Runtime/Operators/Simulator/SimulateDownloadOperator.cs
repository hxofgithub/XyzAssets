﻿namespace XyzAssets.Runtime
{
    internal sealed class SimulateDownloadOperator : DownloadOperator
    {
        protected override void OnExecute()
        {
            Status = EOperatorStatus.Succeed;
        }

        protected override void OnStart()
        {

        }
    }
}