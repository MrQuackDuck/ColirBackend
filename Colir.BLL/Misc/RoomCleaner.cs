using Colir.BLL.Interfaces;

namespace Colir.BLL.Misc;

/// <inheritdoc cref="IRoomCleaner"/>
public class RoomCleaner : IRoomCleaner
{
    public int FilesToDeleteCount { get; }
    public event Action? FileDeleted;

    private List<string> _filesToDelete;

    public RoomCleaner(string directoryPath)
    {
        _filesToDelete = Directory.GetFiles(directoryPath).ToList();
        FilesToDeleteCount = _filesToDelete.Count;
    }

    /// <inheritdoc cref="IRoomCleaner.Start"/>
    public void Start()
    {
        foreach (var file in _filesToDelete)
        {
            File.Delete(file);
            FileDeleted?.Invoke();
        }
    }
}