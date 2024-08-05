namespace Colir.Communication.Models;

public class VoiceChatUser
{
    public long HexId { get; set; }
    public string ConnectionId { get; set; } = default!;
    public string RoomGuid { get; set; } = default!;
    public bool IsMuted { get; set; }
    public bool IsDefeaned { get; set; }
    public bool IsVideoEnabled { get; set; }
    public bool IsStreamEnabled { get; set; }

    /// <summary>
    /// Stands for ids of other users which streams are being currently watched by the user
    /// </summary>
    public List<long> WatchedStreams { get; set; } = new();
}