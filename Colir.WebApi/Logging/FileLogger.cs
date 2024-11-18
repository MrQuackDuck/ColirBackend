namespace Colir.Logging;

public class FileLogger : ILogger
{
    private readonly string _folderPath;
    private readonly string _category;
    private static readonly object Lock = new();

    public FileLogger(string folderPath, string category)
    {
        _folderPath = folderPath;
        _category = category;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
        Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var logFile = Path.Combine(_folderPath, $"colir-{DateTime.UtcNow:yyyy-MM-dd}.log");
        var message = formatter(state, exception);
        var logEntry = $"{Environment.NewLine}[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} {logLevel}] - {_category}: \n{message}";

        // Ensure only one thread writes at a time
        lock (Lock)
        {
            File.AppendAllLines(logFile, new[] { logEntry, });
        }
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
}
