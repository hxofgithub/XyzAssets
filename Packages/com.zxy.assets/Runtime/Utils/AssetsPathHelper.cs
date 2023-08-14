using System.IO;
namespace XyzAssets.Runtime
{
    internal static class AssetsPathHelper
    {
        #region 临时文件路径
        public static string ExternalTempPath
        {
            get
            {
                if (string.IsNullOrEmpty(m_ExternalTempPath))
                {
                    string path = "Temp";

                    if (UnityEngine.Application.isMobilePlatform)
                    {
                        path = Path.Combine(UnityEngine.Application.persistentDataPath, path);
#if UNITY_IOS
                        UnityEngine.iOS.Device.SetNoBackupFlag(path);
#endif
                        m_ExternalTempPath = path;
                    }
                    else
                        m_ExternalTempPath = $"SandBox/{path}";
                }
                return m_ExternalTempPath;
            }
        }
        private static string m_ExternalTempPath;
        public static string GetTempFilePath(string fileName)
        {
            return Path.Combine(ExternalTempPath, fileName);
        }
        #endregion

        #region 外部路径
        public static string ExternalPath
        {
            get
            {
                if (string.IsNullOrEmpty(m_ExternalPath))
                {
                    string path = "Data";

                    if (UnityEngine.Application.isMobilePlatform)
                    {
                        path = Path.Combine(UnityEngine.Application.persistentDataPath, path);
#if UNITY_IOS
                        UnityEngine.iOS.Device.SetNoBackupFlag(path);
#endif
                        m_ExternalPath = path;
                    }
                    else
                        m_ExternalPath = $"SandBox/{path}";
                }
                return m_ExternalPath;
            }
        }
        private static string m_ExternalPath;

        public static string GetFileExternalPath(string fileName)
        {
            return Path.Combine(ExternalPath, fileName);
        }
        #endregion

    }
}