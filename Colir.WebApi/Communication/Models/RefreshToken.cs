namespace Colir.Communication.Models;

public class RefreshToken
{
    /// <summary>
    /// The JWT token
    /// </summary>
    public string Content { get; set; } = default!;

    /// <summary>
    /// Expiry date of the refresh token
    /// </summary>
    public DateTime ExpiryDate { get; set; }
}