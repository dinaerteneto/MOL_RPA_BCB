namespace Core.Logging
{
    public interface ILogger
    {
        void LogInformation(string message);
        void LogError(string message, Exception? exception = null);
        void LogDebug(string message);
    }
}
