namespace Colir.Communication.Models;

public class RefreshToken
{
    /// <summary>
    /// The JWT token
    /// </summary>
    public string Content { get; set; } = default!;

    /// <summary>
    /// Expiry date of a refresh token
    /// </summary>
    public DateTime ExpiryDate { get; set; }
}