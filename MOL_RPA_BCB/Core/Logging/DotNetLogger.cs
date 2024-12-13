namespace Core.Logging
{
    public class DotNetLogger<T> : ILogger
    {
        private readonly Microsoft.Extensions.Logging.ILogger<T> _logger;

        public DotNetLogger(Microsoft.Extensions.Logging.ILogger<T> logger)
        {
            _logger = logger;
        }

        public void LogInformation(string message)
        {
            _logger.LogInformation(message);
        }

        public void LogError(string message, Exception? exception = null)
        {
            _logger.LogError(exception, message);
        }

        public void LogDebug(string message)
        {
            _logger.LogDebug(message);
        }
    }
}
