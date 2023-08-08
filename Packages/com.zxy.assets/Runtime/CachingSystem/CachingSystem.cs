
namespace XyzAssets.Runtime
{
    public enum EVerifyResult
    {
        /// <summary>
        /// 未找到缓存信息
        /// </summary>
        CacheNotFound = -4,

        /// <summary>
        /// 文件内容不足（小于正常大小）
        /// </summary>
        FileNotComplete = -3,

        /// <summary>
        /// 文件内容溢出（超过正常大小）
        /// </summary>
        FileOverflow = -2,

        /// <summary>
        /// 文件内容不匹配
        /// </summary>
        FileCrcError = -1,

        /// <summary>
        /// 默认状态（校验未完成）
        /// </summary>
        None = 0,

        /// <summary>
        /// 验证成功
        /// </summary>
        Succeed = 1,
    }

    public class CachingSystem
    {
        internal static EVerifyResult VerifyTempFile(string path, long fileSize, string version)
        {
            return EVerifyResult.Succeed;
        }

        internal static void TryDiscardTempFile(string path) { }

    }

}
