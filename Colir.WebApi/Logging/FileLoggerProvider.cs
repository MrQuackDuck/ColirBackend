namespace Colir.Logging;

public sealed class FileLoggerProvider : ILoggerProvider
{
    private readonly string _folderPath;

    public FileLoggerProvider(string folderPath)
    {
        _folderPath = folderPath;
        Directory.CreateDirectory(folderPath);
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new FileLogger(_folderPath, categoryName);
    }

    public void Dispose()
    {
    }
}