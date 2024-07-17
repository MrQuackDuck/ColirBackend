using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DAL.Enums;
using Microsoft.EntityFrameworkCore;

namespace DAL.Entities;

[Index(nameof(HexId), IsUnique = true)]
public class User : BaseEntity
{
    [Range(0, 16_777_216)]
    public long HexId { get; set; }
    public int? GitHubId { get; set; }
    public string Username { get; set; } = default!;
    public UserAuthType AuthType { get; set; }

    [ForeignKey(nameof(UserStatisticsId))]
    public long UserStatisticsId { get; set; }

    [ForeignKey(nameof(UserSettings))]
    public long UserSettingsId { get; set; }

    public UserStatistics UserStatistics { get; set; } = new UserStatistics();
    public UserSettings UserSettings { get; set; } = new UserSettings();
    public ICollection<Room> JoinedRooms { get; set; } = default!;
}