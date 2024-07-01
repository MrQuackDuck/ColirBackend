namespace Colir.BLL.RequestModels.User;

public class RequestToChangeUsername
{
    public long IssuerId { get; set; }
    public string DesiredUsername { get; set; } = default!;
}