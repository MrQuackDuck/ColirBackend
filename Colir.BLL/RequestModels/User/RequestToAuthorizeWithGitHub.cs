using DAL.Enums;

namespace Colir.BLL.RequestModels.User;

public class RequestToAuthorizeWithGitHub
{
    public string GitHubId { get; set; } = default!;
    public string HexId { get; set; } = default!;
    public string Username { get; set; } = default!;
    public UserAuthType AuthType { get; set; }
}