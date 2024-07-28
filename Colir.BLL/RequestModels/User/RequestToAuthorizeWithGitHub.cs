namespace Colir.BLL.RequestModels.User;

public class RequestToAuthorizeViaGitHub
{
    public string GitHubId { get; set; } = default!;
    public int HexId { get; set; }
    public string Username { get; set; } = default!;
}