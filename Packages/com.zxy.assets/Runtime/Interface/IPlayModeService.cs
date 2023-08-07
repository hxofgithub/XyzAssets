using System;
using System.IO;

namespace XyzAssets.Runtime
{
    public interface IPlayModeService
    {
        /// <summary>
        /// 存在多个语言版本时，获取当前正确的本地化包名
        /// </summary>
        string GetCorrectBundleName(string bundleName);

        string[] ResUrls { get; }

        int MaxRetryTimes { get; }

    }
}