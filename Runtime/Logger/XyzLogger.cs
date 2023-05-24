
using System.Diagnostics;

namespace XyzAssets.Runtime
{
    internal static class XyzLogger
    {
        public static void Log(string message)
        {
            if (m_Logger != null)
                m_Logger.Log(message);
        }

        public static void LogError(string message)
        {
            if (m_Logger != null)
                m_Logger.LogError(message);
        }

        public static void LogWarning(string message)
        {
            if (m_Logger != null)
                m_Logger.LogWarning(message);
        }

        public static void Exception(System.Exception exception)
        {
            if (m_Logger != null)
                m_Logger.Exception(exception);
        }

        public static void SetLogger(ILogger logger)
        {
            m_Logger = logger;
        }

        public static void SetEnable(bool value)
        {
            if (m_Logger != null)
                m_Logger.enable = value;
        }

        private static ILogger m_Logger;
    }
}