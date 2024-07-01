namespace Colir.BLL.Models;

public class UserStatisticsModel
{
    public long UserId { get; set; }
    public decimal SecondsSpentInVoice { get; set; }
    public long ReactionsSet { get; set; }
    public long MessagesSent { get; set; }
    public long RoomsJoined { get; set; }
    public long RoomsCreated { get; set; }
}