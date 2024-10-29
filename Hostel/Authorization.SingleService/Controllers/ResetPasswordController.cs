using Suo.Autorization.SingleService.Infrastructure.Auth;
using Suo.Autorization.Extentions;
using JWTCoding;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Suo.Autorization.SingleService.Infrastructure.Auth.Identity;
using Suo.Autorization.SingleService.Infrastructure.Interfaces.IdentityServices;
using Suo.Autorization.SingleService.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Suo.Autorization.SingleService.Controllers
{

    [Route("reset/[controller]/[action]")]
    [AllowAnonymous]
    public class ResetPasswordController : Controller
    {
        private readonly IAccountService accountService;
        private readonly AppConfiguration _appConfiguration;
        public UserData userData = new UserData();
        public static string key = "8asdq728das412zxcq14asd";
        //private readonly IUserManagmentService _managmentService;


        public ResetPasswordController(IAccountService accountService, AppConfiguration appConfiguration, IUserManagmentService managmentService)
        {
            this.accountService = accountService;
            _appConfiguration = appConfiguration;
            //_managmentService = managmentService;
        }



        [HttpGet("{token}")]
        public ActionResult Reset(string token)
        {

            var myToken = Token.Decrypt(token, key);
            this.HttpContext.Session.SetString("token", myToken);

            Token modelToken = new Token();

            if (modelToken.GetPrincipalFromExpiredToken(myToken) == null)
            {
                return View("ErrorPassword");
            }
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(UserChange tokenModel)
        {
            if (ModelState.IsValid)
            {

                Token token = new Token();
                var mytoken = this.HttpContext.Session.GetString("token");

                var claims = token.GetPrincipalFromExpiredToken(mytoken);//выдать дешифрованный
                var userIds = int.Parse(claims.FindFirstValue(ClaimTypes.NameIdentifier));

                var reseted = await accountService.ResetPasswordAsync(userIds, tokenModel.Password);
                if (reseted == false)
                {
                    tokenModel.Message = "Внутренняя ошибка, пройдите процедуру снова";
                    return View("Reset");
                }

                return Redirect(_appConfiguration.ApplicationUrl);
            }
            return View("Reset");
        }
    }
}

