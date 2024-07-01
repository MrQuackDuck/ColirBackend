using DAL.Enums;

namespace Colir.BLL.Models;

public class DetailedUserModel
{
    public long Id { get; set; }
    public string HexId { get; set; } = default!;
    public string Username { get; set; } = default!;
    public UserAuthType AuthType { get; set; }
    private UserSettingsModel UserSettingsModel { get; set; } = default!;
}