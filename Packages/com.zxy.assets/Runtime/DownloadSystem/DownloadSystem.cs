using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XyzAssets.Runtime
{
    public class DownloadSystem
    {
        /// <summary>
		/// 下载失败后清理文件的HTTP错误码
		/// </summary>
		public static List<long> ClearFileResponseCodes { set; get; }

        public static string[] ResUrls { get; set; }

    }
}
