using DAL.Enums;

namespace Colir.BLL.Models;

public class DetailedUserModel
{
    public long Id { get; set; }
    public int HexId { get; set; }
    public string Username { get; set; } = default!;
    public DateTime RegistrationDate { get; set; }
    public UserAuthType AuthType { get; set; }
    public UserStatisticsModel UserStatistics { get; set; } = default!;
    public UserSettingsModel UserSettings { get; set; } = default!;
    public IList<RoomModel> JoinedRooms { get; set; } = default!;
}