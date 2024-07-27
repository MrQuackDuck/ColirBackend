using Colir.BLL.Interfaces;

namespace Colir.BLL.Misc;

/// <summary>
/// Represents the room cleaning process
/// Each time a file deleted, the "FileDeleted" event is trigerred (in order to move the progress on the client)
/// </summary>
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
    
    /// <summary>
    /// Starts the cleaning process
    /// </summary>
    public void Start()
    {
        foreach (var file in _filesToDelete)
        {
            File.Delete(file);
            FileDeleted?.Invoke();
        }
    }
}