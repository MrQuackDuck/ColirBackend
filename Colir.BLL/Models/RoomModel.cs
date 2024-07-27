namespace Colir.BLL.Models;

public class RoomModel
{
    public string Guid { get; set; } = default!;
    public string Name { get; set; } = default!;
    public DateTime? ExpiryDate { get; set; }
    public UserModel Owner { get; set; } = default!;
    public long UsedMemoryInBytes { get; set; }
    public long FreeMemoryInBytes { get; set; }

    public List<UserModel> JoinedUsers { get; set; } = default!;
}