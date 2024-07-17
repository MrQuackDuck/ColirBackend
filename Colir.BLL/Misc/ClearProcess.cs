using Colir.BLL.Interfaces;

namespace Colir.BLL.Misc;

/// <summary>
/// Represents the room cleaning process
/// Each time a file deleted, the "FileDeleted" event is trigerred
/// </summary>
public class ClearProcess : IClearProcess
{
    public int FilesToDeleteCount { get; }
    public event Action? FileDeleted;

    private List<string> filesToDelete;
    
    public ClearProcess(string directoryPath)
    {
        filesToDelete = Directory.GetFiles(directoryPath).ToList();
        FilesToDeleteCount = filesToDelete.Count;
    }
    
    /// <summary>
    /// Starts the cleaning process
    /// </summary>
    public void Start()
    {
        foreach (var file in filesToDelete)
        {
            File.Delete(file);
            FileDeleted?.Invoke();
        }
    }
}