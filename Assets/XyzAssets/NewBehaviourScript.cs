using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XyzAssets.Runtime;

public class NewBehaviourScript : MonoBehaviour
{

    IEnumerator Start()
    {
        XyzAssets.Runtime.InitializeParameters initialize = new XyzAssets.Runtime.OnlineInitializeParameters();
        initialize.PlayModeService = new PlayModeService();
        using var op = XyzAssets.Runtime.XyzAsset.Initialize(initialize);
        yield return op;
        if (op.Status == EOperatorStatus.Succeed)
        {

        }
        else
            Debug.LogError(op.Error);
    }

    // Update is called once per frame
    void Update()
    {

    }
}


class PlayModeService : IPlayModeService
{
    public string[] ResUrls => new string[] { "https://malaysia-test-oss.oss-cn-hangzhou.aliyuncs.com/" };

    public int MaxRetryTimes => 4;

    public string GetCorrectBundleName(string bundleName)
    {
        return bundleName;
    }
}
