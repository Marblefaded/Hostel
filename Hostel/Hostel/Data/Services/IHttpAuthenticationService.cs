using Suo.Admin.Data.Models.Request;
using Suo.Admin.Data.Models.Responce;

namespace Suo.Admin.Data.Services;

public interface IHttpAuthenticationService
{
    public Task<TokenResponse> Login(TokenRequest tokenModel);

    public Task Loguot();

    public Task RefreshToken();
}