using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities;

public class UserSettings : BaseEntity
{
    [ForeignKey(nameof(User))]
    public long UserId { get; set; }
    public bool StatisticsEnabled { get; set; }

    public User User { get; set; } = default!;
}