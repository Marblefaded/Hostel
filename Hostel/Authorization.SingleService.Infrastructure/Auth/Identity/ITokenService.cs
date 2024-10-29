using Suo.Autorization.SingleService.Infrastructure.Interfaces;
using Suo.Autorization.SingleService.Infrastructure.RequestModels;
using Suo.Autorization.SingleService.Infrastructure.ResponseModels;

namespace Suo.Autorization.SingleService.Infrastructure.Auth.Identity;

public interface ITokenService : IServerSideService
{
    Task<TokenResponse> LoginAsync(TokenRequest model);

    Task<TokenResponse> GetRefreshTokenAsync(RefreshTokenRequest model);

    Task Logout(string userId, string deviceId);

    Task<List<UserDeviceInfoResponse>> GetDevices(string id);
    Task<bool> RemoveDevice(string id, string deviceId);
    Task<bool> RemoveAllOtherDevice(string id, string currentDeviceId);
}