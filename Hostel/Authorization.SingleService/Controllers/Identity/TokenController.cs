using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Suo.Autorization.SingleService.Infrastructure.Auth.Identity;
using Suo.Autorization.SingleService.Infrastructure.RequestModels;
using Suo.Autorization.SingleService.Infrastructure.ResponseModels;

namespace Suo.Autorization.SingleService.Controllers.Identity;

[Route("api/identity/token")]
[ApiController]
[AllowAnonymous]
public class TokenController : ControllerBase
{
    private readonly ITokenService _identityService;

    public TokenController(ITokenService identityService)
    {
        _identityService = identityService;
    }



    [HttpPost]
    //[HttpPost("Get")]
    [AllowAnonymous]
    public async Task<ActionResult> Get(TokenRequest model)
    {
        var response = await _identityService.LoginAsync(model);
        return Ok(response);
    }


    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<TokenResponse> Refresh(RefreshTokenRequest model)
    {
        try
        {
            //if (this.Response.HasStarted)
            //{
            //    return BadRequest();
            //}
            
            var response = await _identityService.GetRefreshTokenAsync(model);//ошибка
            
            //return Ok(response);
            return response;
        }
        catch (Exception e)
        {
            throw e;
        }
    }



    
}