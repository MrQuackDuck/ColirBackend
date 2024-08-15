namespace Colir.Communication.RequestModels.Auth;

public class RefreshTokenRequestModel
{
    public string AccessToken { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
}