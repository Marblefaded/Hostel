using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using Suo.Admin.Data.Models;
using Suo.Admin.Data.Models.Request;
using Suo.Admin.Data.Models.Responce;
using Suo.Admin.Extentions;
using System.Security.Cryptography;
using System.Text;

namespace Suo.Admin.Data.Services;

public class HttpAuthenticationService : IHttpAuthenticationService
{
    private readonly ILocalStorageService _storage;
    private readonly AppConfiguration _config;
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public HttpAuthenticationService(ILocalStorageService storage, AppConfiguration config, AuthenticationStateProvider authenticationStateProvider)
    {
        _storage = storage;
        _config = config;
        _authenticationStateProvider = authenticationStateProvider;
        
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

    public async Task<TokenResponse> Login(TokenRequest tokenModel)
    {
        try
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(_config.IdentityServer);
            var response = await client.PostAsJsonAsync(_config.IdentityServer +"/api/identity/token", tokenModel);//Дать шифровку
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<TokenResponse>(content);
            if (result.IsSuccsesdTokenLogin)
            {
                var token = result.Token;
                token = Encrypt(result.Token, key);
                var refreshToken = result.RefreshToken;
                await _storage.SetItemAsync(StorageConstants.Local.AuthToken, token);
                await _storage.SetItemAsync(StorageConstants.Local.RefreshToken, refreshToken);
                await _storage.SetItemAsync(StorageConstants.Local.DeviceId, result.DeviceId);
                await ((HostelAuthenticationStateProvider)_authenticationStateProvider).StateChangedAsync();
                return await Task.FromResult(result);
            }
            else
            {
                return await Task.FromResult(result);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        await (_authenticationStateProvider as HostelAuthenticationStateProvider).StateChangedAsync();
    }



    public async Task Loguot()
    {
        try
        {

        }
        catch(Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

    }

    public async Task RefreshToken()
    {

    }
}