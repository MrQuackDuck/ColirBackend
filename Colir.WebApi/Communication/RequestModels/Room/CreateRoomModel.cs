namespace Colir.Communication.RequestModels.Room;

public class CreateRoomModel
{
    public string Name { get; set; } = default!;
    public int OwnerHexId { get; set; } = default!;
    public DateTime? ExpiryDate { get; set; } = null;
}