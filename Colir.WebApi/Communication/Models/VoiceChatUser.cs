namespace Colir.Communication.Models;

public class VoiceChatUser
{
    public long UserId { get; set; }
    public string RoomGuid { get; set; }
    public bool IsMuted { get; set; }
    public bool IsDefeaned { get; set; }
    public bool IsVideoEnabled { get; set; }
    public bool IsStreamEnabled { get; set; }
}