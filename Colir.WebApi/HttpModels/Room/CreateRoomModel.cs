namespace Colir.HttpModels.Room;

public class CreateRoomModel
{
    public string Name { get; set; } = default!;
    public string OwnerHexId { get; set; } = default!;
    public DateTime? ExpiryDate { get; set; }
}