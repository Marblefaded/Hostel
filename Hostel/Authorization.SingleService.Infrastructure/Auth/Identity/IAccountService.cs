using Suo.Autorization.SingleService.Infrastructure.RequestModels;
using Suo.Autorization.SingleService.Infrastructure.ResponseModels;

namespace Suo.Autorization.SingleService.Infrastructure.Auth.Identity;

public interface IAccountService
{
    Task<ChangePasswordResponse> ChangePasswordAsync(ChangePasswordRequest model, int userId);

    Task<bool> ResetPasswordAsync(int userId, string newPassword);
}