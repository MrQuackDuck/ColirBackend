using Colir.Communication.ResponseModels;

namespace Colir.Interfaces.Hubs;

public interface IClearRoomHub
{
    /// <summary>
    /// Starts a room cleaning process
    /// Each time a file got deleted, the "FileDeleted" signal is fired
    /// When the cleaning finished, the "CleaningFinished" signal is fired
    /// </summary>
    Task<SignalRHubResult> Clear();
}