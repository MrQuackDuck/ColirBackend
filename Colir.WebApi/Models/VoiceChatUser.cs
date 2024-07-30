namespace Colir.Models;

public class VoiceChatUser
{
    public int Id { get; set; }
    public bool IsMuted { get; set; }
    public bool IsDefeaned { get; set; }
    public bool IsVideoEnabled { get; set; }
    public bool IsStreamEnabled { get; set; }
}