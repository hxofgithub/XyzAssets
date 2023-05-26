using System.IO;
namespace XyzAssets.Runtime
{
    public class XyzAssetPathHelper
    {
        public static string ExternalPath
        {
            get
            {
                if (string.IsNullOrEmpty(m_ExternalPath))
                {
                    string path = XyzConfiguration.ResourceRootName;

                    if (UnityEngine.Application.isMobilePlatform)
                    {
                        if (string.IsNullOrEmpty(path))
                            path = UnityEngine.Application.persistentDataPath;
                        else
                            path = Path.Combine(UnityEngine.Application.persistentDataPath, path);
#if UNITY_IOS
                        UnityEngine.iOS.Device.SetNoBackupFlag(path);
#endif
                        m_ExternalPath = path;
                    }
                    else
                        m_ExternalPath = $"SandBox/{XyzConfiguration.ResourceRootName}";
                }
                return m_ExternalPath;
            }
            set
            {
                if (string.IsNullOrEmpty(value) || m_ExternalPath == value)
                    return;
                m_ExternalPath = value;
                if (UnityEngine.Application.isMobilePlatform)
                {
#if UNITY_IOS
                    UnityEngine.iOS.Device.SetNoBackupFlag(m_ExternalPath);
#endif
                }

            }
        }
        private static string m_ExternalPath;

        public static string GetFileExternalPath(string fileName)
        {
            return Path.Combine(ExternalPath, fileName);
        }

    }
}