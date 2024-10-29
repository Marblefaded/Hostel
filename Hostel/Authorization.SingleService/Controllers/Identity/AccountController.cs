using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Suo.Autorization.SingleService.Infrastructure.Auth;
using Suo.Autorization.SingleService.Infrastructure.Auth.Identity;
using Suo.Autorization.SingleService.Infrastructure.Interfaces.IdentityServices;
using Suo.Autorization.SingleService.Infrastructure.RequestModels;
using Suo.Autorization.SingleService.Infrastructure.ResponseModels;

namespace Suo.Autorization.SingleService.Controllers.Identity;



[Route("api/identity/account")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly ICurrentUserService _currentUser;
    private readonly IUserManagmentService _userManagmentService;
    private readonly ITokenService _identityService;

    public AccountController(IAccountService accountService, ICurrentUserService currentUser,
        IUserManagmentService userManagmentService, ITokenService identityService)
    {
        _accountService = accountService;
        _currentUser = currentUser;
        _userManagmentService = userManagmentService;
        _identityService = identityService;
    }

    [HttpPost]
    [Route("ChangePassword")]
    public async Task<ActionResult> ChangePassword(ChangePasswordRequest model)
    {
        var response = await _accountService.ChangePasswordAsync(model, int.Parse(_currentUser.UserId));
        return Ok(response);
    }

    [HttpPost]
    [Route("Logout")]

    public async Task Logout(string deviceId)
    {
        var id = _currentUser.UserId;
        await _identityService.Logout(id, deviceId);
    }

    [HttpPost]
    [Route("GetDevices")]

    public async Task<List<UserDeviceInfoResponse>> GetDevices()
    {
        var id = _currentUser.UserId;
        var response = await _identityService.GetDevices(id);

        return response;
    }

    [HttpPost]
    [Route("RemoveDevice")]

    public async Task<bool> RemoveDevice(string deviceId)
    {
        var id = _currentUser.UserId;
        //var response =
        var result = await _identityService.RemoveDevice(id, deviceId);
        return result;

    }

    [HttpPost]
    [Route("RemoveAllOthersDevice")]

    public async Task<bool> RemoveAllOthersDevice(string currentDeviceId)
    {
        var id = _currentUser.UserId;
        //var response =
        var result = await _identityService.RemoveAllOtherDevice(id, currentDeviceId);
        return result;

    }
}