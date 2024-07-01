namespace Colir.BLL.RequestModels.Message;

public class RequestToEditMessage
{
    public long IssuerId { get; set; }
    public long MessageId { get; set; }
    public string NewContent { get; set; } = default!;
}