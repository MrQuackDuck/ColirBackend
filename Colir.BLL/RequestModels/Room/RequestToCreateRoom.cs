namespace Colir.BLL.RequestModels.Room;

public class RequestToCreateRoom
{
    public long IssuerId { get; set; }
    public string Name { get; set; } = default!;
    public DateTime? ExpiryDate { get; set; }
}