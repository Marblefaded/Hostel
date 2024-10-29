using System.ComponentModel.DataAnnotations;

namespace Suo.Autorization.Data.Models;

public class AmsRefreshTokens
{
    [Key]
    public int RefreshTokenId { get; set; }
    public string UserId { get; set; }

    public string RefreshToken { get; set; }
    public string UserInfo { get; set; }
    public string DeviceId { get; set; }

    public DateTime RefreshTokenExpiration { get; set; }
    public virtual AmsUser User { get; set; }
}