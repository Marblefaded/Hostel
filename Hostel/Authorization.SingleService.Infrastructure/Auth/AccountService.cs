using Suo.Autorization.Data.Models;
using Microsoft.AspNetCore.Identity;
using Ams.Pm.Wasm.Core.Auth.Identity;
using Suo.Autorization.SingleService.Infrastructure.Auth.Identity;
using Suo.Autorization.SingleService.Infrastructure.RequestModels;
using Suo.Autorization.SingleService.Infrastructure.ResponseModels;

namespace Suo.Autorization.SingleService.Infrastructure.Auth;

public class AccountService : IAccountService
{
    private readonly UserManager<AmsUser> _userManager;

    /*public AccountService(UserManager<AmsUser> userManager)
    {
        _userManager = userManager;
    }*/
    public AccountService(UserManager<AmsUser> userManager, ApplicationAuthDbContext context)
    {
        _userManager = userManager;
        this._context = context;
    }

    private ApplicationAuthDbContext _context;

    public async Task<ChangePasswordResponse> ChangePasswordAsync(ChangePasswordRequest model, int userId)
    {
        //var user = await this._userManager.FindByIdAsync(userId);
        var user =
            _context.Users.FirstOrDefault(x => x.UserId == userId);
        if (user == null)
        {

            return ChangePasswordResponse.Error("User Not Found!");
        }
        //string resetToken = await UserManager.GeneratePasswordResetTokenAsync(model.Id);
        //IdentityResult passwordChangeResult = await UserManager.ResetPasswordAsync(model.Id, resetToken, model.NewPassword);

        //var identityResul22t = await this._userManager.ResetPasswordAsync(user,);

        var identityResult = await this._userManager.ChangePasswordAsync(
            user,
            model.Password,
            model.NewPassword);
        var errors = identityResult.Errors.Select(e => e.Description).ToList();
        return identityResult.Succeeded ? ChangePasswordResponse.Succses() : ChangePasswordResponse.Error(string.Join("\n", errors));
    }

    public async Task<bool> ResetPasswordAsync(int userId,string newPassword)
    {
        var user =
            _context.Users.FirstOrDefault(x => x.UserId == userId);
        
        var userdb = await this._userManager.FindByIdAsync(user.Id);
        if (user == null)
        {
            
            return false;
        }
        string resetToken = await _userManager.GeneratePasswordResetTokenAsync(userdb);
        IdentityResult passwordChangeResult = await _userManager.ResetPasswordAsync(userdb, resetToken, newPassword);
        return passwordChangeResult.Succeeded;
    }
}