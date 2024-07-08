using System.ComponentModel.DataAnnotations.Schema;
using DAL.Enums;
using Microsoft.EntityFrameworkCore;

namespace DAL.Entities;

[Index(nameof(HexId), IsUnique = true)]
public class User : BaseEntity
{
    public string HexId { get; set; } = default!;
    public int? GitHubId { get; set; }
    public string Username { get; set; } = default!;
    public UserAuthType AuthType { get; set; }

    [ForeignKey(nameof(UserStatisticsId))]
    public long UserStatisticsId { get; set; }

    [ForeignKey(nameof(UserSettings))]
    public long UserSettingsId { get; set; }

    public UserStatistics UserStatistics { get; set; } = default!;
    public UserSettings UserSettings { get; set; } = default!;
    public ICollection<Room> JoinedRooms { get; set; } = default!;
}