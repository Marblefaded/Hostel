using Suo.Autorization.Extentions;
using JWTCoding;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Suo.Autorization.SingleService.Infrastructure.Interfaces.IdentityServices;
using Suo.Autorization.SingleService.Models;

namespace Suo.Autorization.SingleService.Controllers
{
    [Route("register/[controller]/[action]")]
    [AllowAnonymous]
    public class ParameterTokenController : Controller
    {
        private readonly DatabaseSeederUser seederUser;
        private readonly AppConfiguration _appConfiguration;
        public UserData userData = new UserData();

        public static string key = "8asdq728das412zxcq14asd";

        public ParameterTokenController(DatabaseSeederUser seederUser, AppConfiguration appConfiguration, IUserManagmentService managmentService)
        {
            this.seederUser = seederUser;
            _appConfiguration = appConfiguration;

        }

        [HttpGet("{token}")]
        public ActionResult Create(string token)
        {
            var myToken = Token.Decrypt(token, key);
            this.HttpContext.Session.SetString("token", myToken);
            Token modelToken = new Token();

            if (modelToken.GetPrincipalFromExpiredToken(myToken) == null)
            { 
                return View("ErrorJwt");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserData tokenModel)
        {
            if (ModelState.IsValid)
            {
                Token token = new Token();

                tokenModel.UserDeviceUnfo = "MVC Application";
                var mytoken = this.HttpContext.Session.GetString("token");

                var claims = token.GetPrincipalFromExpiredToken(mytoken);

                var userIds = claims.FindFirstValue(ClaimTypes.NameIdentifier);
                var userId = int.Parse(userIds);

                var credentrials = await seederUser.AddUser(tokenModel.Email, tokenModel.Password, userId);
                return Redirect(_appConfiguration.ApplicationUrl);
            }

            return View("Create");


        }
    }
}
