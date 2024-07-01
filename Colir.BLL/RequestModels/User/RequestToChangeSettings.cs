using Colir.BLL.Models;

namespace Colir.BLL.RequestModels.User;

public class RequestToChangeSettings
{
    public long IssuerId { get; set; }
    public UserSettingsModel NewSettings { get; set; } = default!;
}