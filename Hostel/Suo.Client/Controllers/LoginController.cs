using Microsoft.AspNetCore.Mvc;
using Suo.Client.Data.Models.Services;
using Suo.Client.Models.Request;
using Suo.Client.Models;
using Microsoft.AspNetCore.SignalR.Protocol;
using Suo.Client.Models.Responce;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using Suo.Client.Extentions;

namespace Suo.Client.Controllers
{
    public class LoginController : Controller
    {
        private readonly HttpAuthenticationService _authenticationService;
        private readonly AppConfiguration _appConfiguration;

        public LoginController(HttpAuthenticationService authenticationService, AppConfiguration appConfiguration)
        {
            _authenticationService = authenticationService;
            _appConfiguration = appConfiguration;
        }

        public IActionResult Index(string redirectUri)
        {
            var tokenModel = new TokenRequest();
            var tokenResponse = new TokenResponse();
            tokenResponse.TelegramLink = _appConfiguration.TelegramLink;
            ViewBag.RedirectUri = "/";
            //ViewBag["redirectUri"] = redirectUri;
            return View(tokenResponse);
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, string redirectUri)
        {
            var tokenModel = new TokenRequest();
            var tokenResponse = new TokenResponse();
            tokenModel.Email = email;
            tokenModel.Password = password;
            tokenModel.UserDeviceUnfo = "MVC Application";
            var resualt = await _authenticationService.Login(tokenModel, this.HttpContext);
            if (resualt.IsSuccsesdTokenLogin)
            {
                resualt.Token = Decrypt(resualt.Token, key);
                var token = GetPrincipalFromExpiredToken(resualt.Token);
                var answer = token.Claims.FirstOrDefault(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Value;
                if (answer != "User")
                {
                    await _authenticationService.LogOut(tokenModel, this.HttpContext);
                    return Redirect(_appConfiguration.AdminProjectUrl);
                }
                return Redirect("/");
            }
            else
            {
                resualt.FailMessage = "Вы ввели не правильный Email или пароль. Повторите попытку.";
                ViewBag.RedirectUri = "/";
                return View("Index", resualt);
            }
        }

        private const int Keysize = 128;

        private const int DerivationIterations = 1000;

        public static string key = "8uqz391oz12acm";

        public static string symbol = "&#x2F;";

        public static string Encrypt(string plainText, string passPhrase)
        {

            var saltStringBytes = Generate128BitsOfRandomEntropy();
            var ivStringBytes = Generate128BitsOfRandomEntropy();
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = Aes.Create())
                {
                    symmetricKey.BlockSize = 128;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;

                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                var cipherTextBytes = saltStringBytes;
                                cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }

        public static string Decrypt(string cipherText, string passPhrase)
        {

            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);

            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
            var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = Aes.Create())
                {
                    symmetricKey.BlockSize = 128;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            using (var streamReader = new StreamReader(cryptoStream, Encoding.UTF8))
                            {
                                return streamReader.ReadToEnd();
                            }
                        }
                    }
                }
            }
        }

        private static byte[] Generate128BitsOfRandomEntropy()
        {
            var randomBytes = new byte[16];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            var tokenModel = new TokenRequest();
            await _authenticationService.LogOut(tokenModel, this.HttpContext);
            return Redirect("/");
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("BLAG00!JHweaS4L1!Jlu3w2rhWOI!EUR@iu!4723YRU")),
                ValidateIssuer = false,
                ValidateAudience = false,
                RoleClaimType = ClaimTypes.Role,
                ClockSkew = TimeSpan.Zero,
                ValidateLifetime = false
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
    }
}