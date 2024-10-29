using Suo.Autorization.SingleService.Core.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Suo.Admin.Data.Models;
using Suo.Admin.Data.Models.Request;
using Suo.Admin.Data.Models.Responce;
using Suo.Admin.Extentions;
using System.Security.Cryptography;

namespace Suo.Admin.Data.Services;

public class HostelAuthenticationStateProvider : AuthenticationStateProvider, IHostEnvironmentAuthenticationStateProvider, IDisposable
{
    private Task<AuthenticationState> _authenticationStateTask;


    /// <inheritdoc />

    /// <inheritdoc />
    public void SetAuthenticationState(Task<AuthenticationState> authenticationStateTask)
    {
        _authenticationStateTask = authenticationStateTask ?? throw new ArgumentNullException(nameof(authenticationStateTask));
        NotifyAuthenticationStateChanged(_authenticationStateTask);

    }

    //private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private readonly IHttpContextAccessor _accessor;
    private readonly AppConfiguration _config;

    public HostelAuthenticationStateProvider(ILocalStorageService localStorage, IHttpContextAccessor accessor, AppConfiguration config)
    {
        //_httpClient = httpClient;
        _localStorage = localStorage;
        _accessor = accessor;
        _config = config;
    }
    public ClaimsPrincipal AuthenticationStateUser { get; set; }

    public async Task StateChangedAsync()
    {
        var authState = Task.FromResult(await GetAuthenticationStateAsync());

        NotifyAuthenticationStateChanged(authState);

    }

    private Timer _timerAuthenticationExpiration;
    public void MarkUserAsLoggedOut()
    {
        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
        var authState = Task.FromResult(new AuthenticationState(anonymousUser));
        if (_timerAuthenticationExpiration != null)
        {
            _timerAuthenticationExpiration.Dispose();
        }
        _localStorage.SetItemAsync(StorageConstants.Local.AuthToken, "");
        _localStorage.SetItemAsync(StorageConstants.Local.RefreshToken, "");
        NotifyAuthenticationStateChanged(authState);
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            string savedToken = null;
            try
            {
                var preSavedToken = await _localStorage.GetItemAsync<string>(StorageConstants.Local.AuthToken);//Делать дешифровку
                savedToken = Decrypt(preSavedToken, key);//дешифровать нормально
            }
            catch (Exception e)
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            if (string.IsNullOrWhiteSpace(savedToken))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", savedToken);
            var state = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(GetClaimsFromJwt(savedToken), "jwt")));
            AuthenticationStateUser = state.User;

            if (_timerAuthenticationExpiration != null)
            {
                await _timerAuthenticationExpiration.DisposeAsync();
            }

            _timerAuthenticationExpiration = new Timer(TriggerAuth, null, TimeSpan.Zero, TimeSpan.FromSeconds(20));
            return state;
        }
        catch (Exception e)
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        }
    }

    private int CountJSDisconnectedException { get; set; } = 0;
    private async void TriggerAuth(object? state)
    {
        var test = 1;
        var savedToken = "";
        try
        {
            savedToken = await _localStorage.GetItemAsync<string>(StorageConstants.Local.AuthToken);
        }
        catch (JSDisconnectedException e)
        {
            if (CountJSDisconnectedException == 6)
            {
                var stateAuth = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                this.AuthenticationStateUser = stateAuth.User;
                await StateChangedAsync();
            }
            return;

        }

        if (await CheckExpiryTime(savedToken))
        {
            try
            {
                await RefreshAccessToken();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
                MarkUserAsLoggedOut();
            }

        }

        //var expClaim = GetClaimsFromJwt(savedToken).FirstOrDefault(x => x.Type == "exp").Value;
        //var expTime = UnixTimeStampToDateTime(long.Parse(expClaim));
        //if (DateTime.Now > expTime)
        //{
        //    var ex = 4;
        //}

        //Console.WriteLine(DateTime.Now.ToString("T"));
    }
    private static bool IsExecuting = false;
    private object _lock = new object();

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

    public async Task RefreshAccessToken()
    {
        /*lock (_lock)
        {
            if (IsExecuting)
            {
                //return null;
                return;
            }
            else
            {
                IsExecuting = true;
            }
        }*/

        var token = await _localStorage.GetItemAsync<string>(StorageConstants.Local.AuthToken);
        /*token = Encrypt(token, key);*/
        var refreshToken = await _localStorage.GetItemAsync<string>(StorageConstants.Local.RefreshToken);
        var deviceId = await _localStorage.GetItemAsync<string>(StorageConstants.Local.DeviceId);
        string content = "";
        HttpResponseMessage response;
        try
        {
            var _httpClient = new HttpClient();
            var requestModel = new RefreshTokenRequest { Token = token, RefreshToken = refreshToken, DeviceId = deviceId };
            //var request = new RequestModel<RefreshTokenRequest>();
            var uri = _config.IdentityServer + "/api/identity/token/refresh";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);
            //_httpClient.Timeout = TimeSpan.FromSeconds(2);
            var jsonContent = JsonConvert.SerializeObject(requestModel);
            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            //request.Method = HttpMethod.Post;
            //request.Uri = _httpClient.BaseAddress + "api/identity/token/refresh";
            response = await _httpClient.SendAsync(request);
            content = await response.Content.ReadAsStringAsync();

            //var response = await _httpClient.PostAsJsonAsync("api/identity/token/refresh", new RefreshTokenRequest { Token = token, RefreshToken = refreshToken });
            //var content = await response.Content.ReadAsStringAsync();
            //var result = JsonSerializer.Deserialize<TokenResponse>(content);
            var result = JsonConvert.DeserializeObject<TokenResponse>(content);
            /*result.Token = Decrypt(result.Token, key);*/
            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException("Something went wrong during the refresh token action");
            }

            if (string.IsNullOrEmpty(result.Token) || string.IsNullOrEmpty(result.RefreshToken) || !result.IsSuccsesdTokenLogin)
            {
                if (result.FailMessage == "Invalid Refresh Client Token.")
                {
                    IsExecuting = false;
                    MarkUserAsLoggedOut();
                    //_navigationManager.NavigateTo("");
                    return;
                    //return null;
                }
                else
                {

                    IsExecuting = false;
                    //return null;
                    return;
                }
            }
            token = result.Token;
            
            refreshToken = result.RefreshToken;
            
            await _localStorage.SetItemAsync(StorageConstants.Local.AuthToken, Encrypt(token,key));
            await _localStorage.SetItemAsync(StorageConstants.Local.RefreshToken, refreshToken);
            await _localStorage.SetItemAsync(StorageConstants.Local.DeviceId, result.DeviceId);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _httpClient.Dispose();
            IsExecuting = false;
            //return token;
        }
        catch (Exception exception)
        {
            IsExecuting = false;
            Console.WriteLine(exception);
            MarkUserAsLoggedOut();
            //throw;
        }
    }

    public async Task<bool> CheckExpiryTime(string savedToken)
    {
        savedToken = Decrypt(savedToken, key);
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
        /*jwt = Decrypt(jwt,key);*/
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

    public void Dispose()
    {
        _authenticationStateTask.Dispose();
        //_httpClient.Dispose();
        _timerAuthenticationExpiration?.Dispose();
    }
}