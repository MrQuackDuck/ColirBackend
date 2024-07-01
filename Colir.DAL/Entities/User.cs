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
    public long UserSettingsId { get; set; }

    public UserSettings UserSettings { get; set; } = default!;
    public ICollection<Room> JoinedRooms { get; set; } = default!;
}