using Suo.Client.Extentions;
using Suo.Client.Models.Request;
using Suo.Client.Models.Responce;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Suo.Client.Data.Models.Services;

public class HttpAuthenticationService
{
    private readonly AppConfiguration _config;

    public HttpAuthenticationService(AppConfiguration config)
    {
        _config = config;
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
    public async Task<TokenResponse> Login(TokenRequest tokenModel, HttpContext context)
    {
        try
        {
            LocalStorage _storage = new LocalStorage(context);
            HttpClient client = new HttpClient();
            //client.BaseAddress = new Uri(_config.IdentityServer);

            var response = await client.PostAsJsonAsync(_config.IdentityServer + "/api/identity/token", tokenModel);
            var content = await response.Content.ReadAsStringAsync();
            //var result = JsonSerializer.Deserialize<TokenResponse>(content);
            var result = JsonConvert.DeserializeObject<TokenResponse>(content);

            //var result = await response.ToResult<TokenResponse>();
            if (result.IsSuccsesdTokenLogin)
            {
                result.Token = Encrypt(result.Token,key);
                var token = result.Token;
                var refreshToken = result.RefreshToken;
                _storage.SetItem(StorageConstants.Local.AuthToken, token);
                _storage.SetItem(StorageConstants.Local.RefreshToken, refreshToken);
                _storage.SetItem(StorageConstants.Local.DeviceId, result.DeviceId);

                //await ((HostelAuthenticationStateProvider)_authenticationStateProvider).StateChangedAsync();

                //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

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

    }

    public async Task LogOut(TokenRequest tokenModel, HttpContext context)
    {
        try
        {
            LocalStorage _storage = new LocalStorage(context);

            _storage.SetItem(StorageConstants.Local.AuthToken, "");
            _storage.SetItem(StorageConstants.Local.RefreshToken, "");
            _storage.SetItem(StorageConstants.Local.DeviceId, "");

            await Task.CompletedTask;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

    }

    public void LogOut(HttpContext context)
    {
        try
        {
            LocalStorage _storage = new LocalStorage(context);

            _storage.SetItem(StorageConstants.Local.AuthToken, "");
            _storage.SetItem(StorageConstants.Local.RefreshToken, "");
            _storage.SetItem(StorageConstants.Local.DeviceId, "");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

    }

}