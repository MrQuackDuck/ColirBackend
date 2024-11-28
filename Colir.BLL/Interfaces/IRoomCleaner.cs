namespace Colir.BLL.Interfaces;

/// <summary>
/// Represents the room cleaning process
/// Each time a file is deleted, the "FileDeleted" event is triggered (in order to update the progress on the client)
/// </summary>
public interface IRoomCleaner
{
    public event Action FileDeleted;
    public event Action Finished;
    public int FilesToDeleteCount { get; }

    /// <summary>
    /// Starts the cleaning process.
    /// </summary>
    Task StartAsync();
}