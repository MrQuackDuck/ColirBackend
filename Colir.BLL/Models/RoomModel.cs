namespace Colir.BLL.Models;

public class RoomModel
{
    public string Guid { get; set; } = default!;
    public string Name { get; set; } = default!;
    public DateTime? ExpiryDate { get; set; } = default!;
    public UserModel Owner { get; set; } = default!;
    public long UsedMemoryInKb { get; set; }
    public long FreeMemoryInKb { get; set; }

    public List<UserModel> JoinedUsers { get; set; } = default!;
}