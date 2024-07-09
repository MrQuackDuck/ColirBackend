using DAL.Enums;

namespace Colir.BLL.Models;

public class DetailedUserModel
{
    public long Id { get; set; }
    public string HexId { get; set; } = default!;
    public string Username { get; set; } = default!;
    public UserAuthType AuthType { get; set; }
    public UserSettingsModel UserSettingsModel { get; set; } = default!;
    public UserStatisticsModel UserStatisticsModel { get; set; } = default!;
}