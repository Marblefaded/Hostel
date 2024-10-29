using System.Net;
using HostelDB.AlfaPruefungDb;
using Newtonsoft.Json;
using Suo.Admin.Data.Models.Request;
using Suo.Admin.Data.Models.Responce;
using Suo.Admin.Extentions;
using System.Net.Http.Headers;
using Suo.Admin.Data.Models;
using Suo.Admin.Data.Services;
using Microsoft.Identity.Client;
using NPOI.XWPF.UserModel;
using Azure;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Cryptography;

namespace Suo.Admin.Data.Service;

public class AspNetUserManagment
{
    private readonly HostelDbContext _context;
    private readonly AppConfiguration _config;
    private readonly ILocalStorageService _localStorage;
    private readonly AuthenticationStateProvider _provider;

    public AspNetUserManagment(HostelDbContext context, AppConfiguration config, ILocalStorageService _localStorage, AuthenticationStateProvider provider)
    {
        _context = context;
        _config = config;
        this._localStorage = _localStorage;
        _provider = provider;
    }

    public async Task<bool> RemoveUserInAuth(int UserId)
    {
        try
        {
            var baseAdressAuth = _config.IdentityServer;
            //HttpClient client = new HttpClient();

            //var request = new HttpRequestMessage();
            //request.Method = HttpMethod.Post;
            ////var tokenrequest = new TokenRequest()
            ////{
            ////    Email = "vlad1999105@gmail.com",
            ////    Password = "Wladgood1051!",
            ////    UserDeviceUnfo = "Suo.Admin"
            ////};
            //var tokenrequest = new TokenRequest()
            //{
            //    Email = "admin@admin.com",
            //    Password = "wlad1051",
            //    UserDeviceUnfo = "Suo.Admin"
            //};
            //request.Content = new StringContent(JsonConvert.SerializeObject(tokenrequest));
            //request.RequestUri = new Uri(baseAdressAuth+"/api/identity/token/get");
            //var respose = await client.SendAsync(request);

            //var tokenResponseS = await respose.Content.ReadAsStringAsync();
            //var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(tokenResponseS);
            //client.Dispose();
            var userIdAsp = Helper<string>.ExecuteSQLQuery(_context, $"select Id from AspNetUsers where UserId = {UserId};",
                reader => (string)reader[0]);
            var resultRows = Helper<string>.ExecuteSQLQuery(_context, $"DELETE FROM AspRefreshTokens where UserId = \'{userIdAsp.FirstOrDefault()}'",
                reader => (string)reader[0]);
            HttpClient client2 = new HttpClient();

            var requestForRemoveAspNetUser = new HttpRequestMessage(HttpMethod.Post, new Uri(baseAdressAuth + $"/api/UserManagment/RemoveUser?userId={userIdAsp.FirstOrDefault()}"));
            //requestForRemoveAspNetUser.Method = HttpMethod.Post;
            var token = await _localStorage.GetItemAsync<string>(StorageConstants.Local.AuthToken);
            token = Decrypt(token, key);
            requestForRemoveAspNetUser.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var result = await client2.SendAsync(requestForRemoveAspNetUser);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw e;
        }
    }

    public async Task<UserResponse> AddHeadmanInAuth(UserAddRequest user)
    {
        try
        {
            var baseAdressAuth = _config.IdentityServer;

            HttpClient client2 = new HttpClient();
            var requestForRemoveAspNetUser = new HttpRequestMessage(HttpMethod.Post, new Uri(baseAdressAuth + $"/api/UserManagment/AddHeadman"));
            var item = JsonConvert.SerializeObject(user);
            requestForRemoveAspNetUser.Content = new StringContent(item, Encoding.UTF8, "application/json");
            var token = await _localStorage.GetItemAsync<string>(StorageConstants.Local.AuthToken);
            token = Decrypt(token, key);    
            requestForRemoveAspNetUser.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var result = await client2.SendAsync(requestForRemoveAspNetUser);
            var content = await result.Content.ReadAsStringAsync();
            var end = JsonConvert.DeserializeObject<UserAddResponse>(content);

            return await Task.FromResult(end.User);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw e;
        }

    }

    public async Task<List<UserResponse>> GetAllHeadmanInAuth()
    {
        try
        {

            var baseAdressAuth = _config.IdentityServer;


            HttpClient client2 = new HttpClient();

            var requestForGetAllAspNetUser = new HttpRequestMessage(HttpMethod.Post, new Uri(baseAdressAuth + $"/api/UserManagment/GetAllHeadman"));
            var token = await _localStorage.GetItemAsync<string>(StorageConstants.Local.AuthToken);
            token = Decrypt(token, key);
            requestForGetAllAspNetUser.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var result = await client2.SendAsync(requestForGetAllAspNetUser);
            if (HttpStatusCode.Unauthorized == result.StatusCode)
            {
                (_provider as HostelAuthenticationStateProvider)?.RefreshAccessToken();
            }
            var content = await result.Content.ReadAsStringAsync();

            

            var end = JsonConvert.DeserializeObject<List<UserResponse>>(content);

            return await Task.FromResult(end);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw e;
        }

    }

    public async Task<UserResponse> GetHeadmanInAuth(int UserId)
    {
        try
        {
            var baseAdressAuth = _config.IdentityServer;


            HttpClient client2 = new HttpClient();
            var requestForGetAspNetUser = new HttpRequestMessage(HttpMethod.Post, new Uri(baseAdressAuth + $"/api/UserManagment/GetHeadman?userId={UserId.ToString()}"));
            //requestForRemoveAspNetUser.Method = HttpMethod.Post;
            var token = await _localStorage.GetItemAsync<string>(StorageConstants.Local.AuthToken);
            token = Decrypt(token, key);
            requestForGetAspNetUser.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var result = await client2.SendAsync(requestForGetAspNetUser);

            var content = await result.Content.ReadAsStringAsync();
            var end = JsonConvert.DeserializeObject<UserResponse>(content);

            return await Task.FromResult(end);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw e;
        }

    }





    public async Task<UserUpdateResponse> UpdateHeadmanInAuth(int UserId)
    {
        try
        {
            UserUpdateResponse userUpdateResponse = new();
            var baseAdressAuth = _config.IdentityServer;

            HttpClient client2 = new HttpClient();
            var requestForUpdateAspNetUser = new HttpRequestMessage(HttpMethod.Post, new Uri(baseAdressAuth + $"/api/UserManagment/UpdateUser?userId={UserId.ToString()}"));
            //requestForRemoveAspNetUser.Method = HttpMethod.Post;
            var token = await _localStorage.GetItemAsync<string>(StorageConstants.Local.AuthToken);
            token = Decrypt(token, key);
            requestForUpdateAspNetUser.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var result = await client2.SendAsync(requestForUpdateAspNetUser);

            var content = await result.Content.ReadAsStringAsync();
            var end = JsonConvert.DeserializeObject<UserUpdateResponse>(content);

            return await Task.FromResult(end);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new NullReferenceException("User Is Null!");
        }

    }
    public async Task<bool> RemoveUser(string UserId)
    {
        try
        {
            var baseAdressAuth = _config.IdentityServer;
            var resultRows = Helper<string>.ExecuteSQLQuery(_context, $"DELETE FROM AspRefreshTokens where UserId = \'{UserId}'",
                reader => (string)reader[0]);
            HttpClient client2 = new HttpClient();

            var requestForRemoveAspNetUser = new HttpRequestMessage(HttpMethod.Post, new Uri(baseAdressAuth + $"/api/UserManagment/RemoveUser?userId={UserId}"));

            //requestForRemoveAspNetUser.Method = HttpMethod.Post;
            var token = await _localStorage.GetItemAsync<string>(StorageConstants.Local.AuthToken);
            token = Decrypt(token, key);
            requestForRemoveAspNetUser.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var result = await client2.SendAsync(requestForRemoveAspNetUser);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw e;
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
}
