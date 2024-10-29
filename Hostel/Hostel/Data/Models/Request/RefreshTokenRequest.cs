namespace Suo.Admin.Data.Models.Request;

public class RefreshTokenRequest
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public string DeviceId { get; set; }
    
}