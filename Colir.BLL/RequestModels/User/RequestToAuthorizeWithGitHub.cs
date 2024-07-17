namespace Colir.BLL.RequestModels.User;

public class RequestToAuthorizeWithGitHub
{
    public string GitHubId { get; set; } = default!;
    public long HexId { get; set; } = default!;
    public string Username { get; set; } = default!;
}