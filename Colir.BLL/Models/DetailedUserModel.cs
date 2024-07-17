using DAL.Enums;

namespace Colir.BLL.Models;

public class DetailedUserModel
{
    public long Id { get; set; }
    public long HexId { get; set; } = default!;
    public string Username { get; set; } = default!;
    public UserAuthType AuthType { get; set; }
    public UserSettingsModel UserSettings { get; set; } = default!;
    public UserStatisticsModel UserStatistics { get; set; } = default!;
    public ICollection<RoomModel> JoinedRooms { get; set; } = default!;
}