using DAL.Enums;

namespace Colir.BLL.Models;

public class UserModel
{
    public long HexId { get; set; }  = default!;
    public string Username { get; set; } = default!;
    public UserAuthType AuthType { get; set; }
}