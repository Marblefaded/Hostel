using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Suo.Client.Data.Models;
using Suo.Client.Data.Models.Services;
using Newtonsoft.Json;
using System.Security.Claims;
using Suo.Client.Auth;
using System.Net.Http.Headers;
using System.Text;
using Suo.Client.Extentions;
using Suo.Client.Models.Request;
using Suo.Client.Models.Responce;
using System.Security.Cryptography;

namespace Suo.Client.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeJwtAttribute : Attribute, IAuthorizationFilter
{
    public bool login { get; set; }

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


    public void OnAuthorization(AuthorizationFilterContext context)
    {
        LocalStorage storage = new LocalStorage(context.HttpContext);

        var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
        if (allowAnonymous)
            return;

        string savedToken = null;
        try
        {
            savedToken = storage.GetItem(StorageConstants.Local.AuthToken);
            savedToken = Decrypt(savedToken, key);
        }
        catch (Exception e)
        {

            //TODO
            context.Result = new RedirectResult(context.HttpContext.Request.PathBase + "/Login");
            return;
        }
        if (string.IsNullOrWhiteSpace(savedToken))
        {
            //new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            //TODO
            /*var url = context.HttpContext.Request.PathBase + "/Login/Index?redirectUri=" +
                      context.HttpContext.Request.Path;
            */context.Result =
                new RedirectResult(context.HttpContext.Request.PathBase + "/Login/Index?redirectUri=" + context.HttpContext.Request.Path);
            return;
        }
        
        if (CheckExpiryTime(savedToken))
        {
            try
            {

                //if (login == false)
                //{
                //    context.Result = new RedirectResult(context.HttpContext.Request.PathBase + "/Login/Index?redirectUri=" + context.HttpContext.Request.Path);
                //}
                //else
                //{
                //    RefreshAccessToken(context.HttpContext);
                //}
                if (!RefreshAccessToken(context.HttpContext))
                {
                    context.Result = new RedirectResult(context.HttpContext.Request.PathBase + "/Login/Index?redirectUri=" + context.HttpContext.Request.Path);
                    
                }
                else
                {

                }


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return;
                //throw;
            }

        }
        else
        {

            try
            {
                context.HttpContext.User.AddIdentity(new ClaimsIdentity(GetClaimsFromJwt(savedToken)));

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }


        //var user = context.HttpContext.Session.Keys;

        //if (user == null)
        //{
        //    context.Result = new RedirectResult(context.HttpContext.Request.PathBase + "/Login");
        //}


        //context.Result = new JsonResult(new { message = "Unauthorized" })
        //{ StatusCode = StatusCodes.Status401Unauthorized };
    }


    private object _lock;

    private bool RefreshAccessToken(HttpContext context)
    {
        LocalStorage _localStorage = new LocalStorage(context);
        

        var token =  _localStorage.GetItem(StorageConstants.Local.AuthToken);
        /*token = Encrypt(token, key);*/
        var refreshToken =  _localStorage.GetItem(StorageConstants.Local.RefreshToken);
        var deviceId =  _localStorage.GetItem(StorageConstants.Local.DeviceId);
        string content = "";
        HttpResponseMessage response;
        try
        {
            var _httpClient = new HttpClient();
            var requestModel = new RefreshTokenRequest { Token = token, RefreshToken = refreshToken, DeviceId = deviceId };

            var uri = AppConfigGlobals.IdentityServer + "/api/identity/token/refresh";
            //var uri = "http://192.168.1.208/TestWASM" + "/api/identity/token/refresh";
            //var uri = "https://localhost:7088" + "/api/identity/token/refresh";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);
            //_httpClient.Timeout = TimeSpan.FromSeconds(2);
            var jsonContent = JsonConvert.SerializeObject(requestModel);
            
            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            //TODO
            response = _httpClient.Send(request);
            var  stream = new StreamReader(response.Content.ReadAsStream());
            content = stream.ReadToEnd();
            bool login = true;

            //Дешифровать
            /*var decryptContent = Decrypt(content, key);*/
            var result = JsonConvert.DeserializeObject<TokenResponse>(content);

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException("Something went wrong during the refresh token action");
            }

            if (string.IsNullOrEmpty(result.Token) || string.IsNullOrEmpty(result.RefreshToken) || !result.IsSuccsesdTokenLogin)
            {
                if (result.FailMessage == "Invalid Refresh Client Token.")
                {

                    //login = false;

                    return false;
                }
                else
                {
                    //TODO

                    //login = true;

                    return false;
                }
            }
            token = result.Token;
            refreshToken = result.RefreshToken;
             _localStorage.SetItem(StorageConstants.Local.AuthToken, Encrypt(token, key));
             _localStorage.SetItem(StorageConstants.Local.RefreshToken, refreshToken);
             _localStorage.SetItem(StorageConstants.Local.DeviceId, result.DeviceId);
             _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
             context.User.AddIdentity(new ClaimsIdentity(GetClaimsFromJwt(token)));
            _httpClient.Dispose();

            return true;

        }
        catch (Exception exception)
        {

            //TODO
            Console.WriteLine(exception);
            //MarkUserAsLoggedOut();
            //throw;
            return false;
        }
    }




    public bool CheckExpiryTime(string savedToken)
    {
        var expClaim = GetClaimsFromJwt(savedToken).FirstOrDefault(x => x.Type == "exp").Value;
        if (string.IsNullOrEmpty(expClaim))
        {
            return true;
        }
        var expTime = UnixTimeStampToDateTime(long.Parse(expClaim)).ToLocalTime();
        if (DateTime.Now > expTime)
        {

            return true;
        }

        return false;
    }

    public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime;
    }

    private IEnumerable<Claim> GetClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        if (keyValuePairs != null)
        {
            keyValuePairs.TryGetValue(ClaimTypes.Role, out var roles);

            if (roles != null)
            {
                if (roles.ToString().Trim().StartsWith("["))
                {
                    var parsedRoles = JsonConvert.DeserializeObject<string[]>(roles.ToString());

                    claims.AddRange(parsedRoles.Select(role => new Claim(ClaimTypes.Role, role)));
                }
                else
                {
                    claims.Add(new Claim(ClaimTypes.Role, roles.ToString()));
                }

                keyValuePairs.Remove(ClaimTypes.Role);
            }

            keyValuePairs.TryGetValue(ApplicationClaimTypes.Permission, out var permissions);
            if (permissions != null)
            {
                if (permissions.ToString().Trim().StartsWith("["))
                {
                    var parsedPermissions = JsonConvert.DeserializeObject<string[]>(permissions.ToString());
                    claims.AddRange(parsedPermissions.Select(permission => new Claim(ApplicationClaimTypes.Permission, permission)));
                }
                else
                {
                    claims.Add(new Claim(ApplicationClaimTypes.Permission, permissions.ToString()));
                }
                keyValuePairs.Remove(ApplicationClaimTypes.Permission);
            }

            claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));
        }
        return claims;
    }
    private byte[] ParseBase64WithoutPadding(string payload)
    {
        payload = payload.Trim().Replace('-', '+').Replace('_', '/');
        var base64 = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
        return Convert.FromBase64String(base64);
    }

}