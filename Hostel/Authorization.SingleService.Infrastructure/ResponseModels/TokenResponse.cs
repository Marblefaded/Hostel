namespace Suo.Autorization.SingleService.Infrastructure.ResponseModels;

public class TokenResponse
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }

    public bool IsSuccsesdTokenLogin { get; set; } = false;
    public string? FailMessage { get; set; }
    public string DeviceId { get; set; }
    public static TokenResponse FailTokenResponse(string Message)
    {
        TokenResponse token = new TokenResponse();
        token.FailMessage = Message;
        token.IsSuccsesdTokenLogin = false;
        return token;
    }

    public static TokenResponse SuccessTokenResponse(string Token, string RefreshToken, DateTime RefreshTokenExpiryTime,
        string tokenDbDeviceId)
    {
        TokenResponse token = new TokenResponse();
        token.Token = Token;
        token.RefreshToken = RefreshToken;
        token.RefreshTokenExpiryTime = RefreshTokenExpiryTime;
        token.IsSuccsesdTokenLogin = true;
        token.DeviceId = tokenDbDeviceId;
        return token;
    }
}