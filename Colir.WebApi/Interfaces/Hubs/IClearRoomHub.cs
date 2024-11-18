using Colir.Communication.ResponseModels;

namespace Colir.Interfaces.Hubs;

public interface IClearRoomHub
{
    /// <summary>
    /// Starts a room cleaning process
    /// The "ReceiveFilesToDeleteCount" signal is fired with the number of files to delete
    /// Each time a file got deleted, the "FileDeleted" signal is fired
    /// When the cleaning finished, the "CleaningFinished" signal is fired
    /// </summary>
    Task<SignalRHubResult> Clear();
}