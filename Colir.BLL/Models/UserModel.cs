using DAL.Enums;

namespace Colir.BLL.Models;

public class UserModel
{
    public int HexId { get; set; }
    public string Username { get; set; } = default!;
    public UserAuthType AuthType { get; set; }
}