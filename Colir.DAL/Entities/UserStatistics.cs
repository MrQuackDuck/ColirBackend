using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities;

public class UserStatistics : BaseEntity
{
    [ForeignKey(nameof(User))]
    public long UserId { get; set; }
    public long SecondsSpentInVoice { get; set; }
    public long ReactionsSet { get; set; }
    public long MessagesSent { get; set; }
    public long RoomsJoined { get; set; }
    public long RoomsCreated { get; set; }

    public User User { get; set; } = default!;
}