using DAL.Enums;

namespace Colir.BLL.Models;

public class DetailedUserModel
{
    public long Id { get; set; }
    public long HexId { get; set; }
    public string Username { get; set; } = default!;
    public UserAuthType AuthType { get; set; }
    public UserStatisticsModel UserStatistics { get; set; } = default!;
    public UserSettingsModel UserSettings { get; set; } = default!;
    public IList<RoomModel> JoinedRooms { get; set; } = default!;
}