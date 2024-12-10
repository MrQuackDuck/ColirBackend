using System.Collections.Concurrent;

namespace Colir.Logging;

public sealed class FileLoggerProvider : ILoggerProvider
{
    private readonly string _folderPath;
    private readonly ConcurrentDictionary<string, FileLogger> _loggers;

    public FileLoggerProvider(string folderPath)
    {
        _folderPath = folderPath ?? throw new ArgumentNullException(nameof(folderPath));
        _loggers = new ConcurrentDictionary<string, FileLogger>();

        try
        {
            Directory.CreateDirectory(_folderPath);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to create directory: {_folderPath}", ex);
        }
    }

    public ILogger CreateLogger(string categoryName)
    {
        return _loggers.GetOrAdd(categoryName, new FileLogger(_folderPath, categoryName));
    }

    public void Dispose()
    {
    }
}