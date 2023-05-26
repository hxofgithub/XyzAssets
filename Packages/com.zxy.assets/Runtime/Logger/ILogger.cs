namespace XyzAssets.Runtime
{
    public interface ILogger
    {
        bool enable { get; set; }
        void Log(string message);
        void LogWarning(string message);
        void LogError(string message);
        void Exception(System.Exception exception);
    }
}
