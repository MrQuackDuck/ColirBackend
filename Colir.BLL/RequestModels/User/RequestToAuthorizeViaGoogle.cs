namespace Colir.BLL.RequestModels.User;

public class RequestToAuthorizeViaGoogle
{
    public string GoogleId { get; set; } = default!;
    public int HexId { get; set; }
    public string Username { get; set; } = default!;
}