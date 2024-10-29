using System.Collections.Immutable;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Ams.Pm.Wasm.Core.Auth.Identity;
using Suo.Autorization.Data.Models;
using Suo.Autorization.Extentions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Suo.Autorization.SingleService.Infrastructure.Auth.Identity;
using Suo.Autorization.SingleService.Infrastructure.RequestModels;
using Suo.Autorization.SingleService.Infrastructure.ResponseModels;

namespace Suo.Autorization.SingleService.Infrastructure.Auth;

public class TokenService : ITokenService
{

    private const string InvalidErrorMessage = "Invalid email or password.";

    private readonly UserManager<AmsUser> _userManager;
    private readonly RoleManager<AmsRole> _roleManager;

    private readonly AppConfiguration _appConfig;
    private readonly SignInManager<AmsUser> _signInManager;
    private readonly ApplicationAuthDbContext _authDbContext;
    public TokenService(UserManager<AmsUser> userManager, RoleManager<AmsRole> roleManager, IOptions<AppConfiguration> appConfig, SignInManager<AmsUser> signInManager, ApplicationAuthDbContext authDbContext)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _appConfig = appConfig.Value;
        _signInManager = signInManager;
        _authDbContext = authDbContext;
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

    public async Task<TokenResponse> LoginAsync(TokenRequest model)
    {

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return TokenResponse.FailTokenResponse("User Not Found.");
        }
        if (!user.IsActive)
        {
            return TokenResponse.FailTokenResponse("User Not Active. Please contact the administrator.");
        }
        if (!user.EmailConfirmed)
        {
            return TokenResponse.FailTokenResponse("E-Mail not confirmed.");
        }
        var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
        if (!passwordValid)
        {
            return TokenResponse.FailTokenResponse("Invalid Credentials.");
        }

        user.RefreshToken = GenerateRefreshToken();
        AmsRefreshTokens tokenDb = new AmsRefreshTokens();
        tokenDb.RefreshToken = GenerateRefreshToken();
        tokenDb.RefreshTokenExpiration = DateTime.Now.AddMonths(1);
        tokenDb.DeviceId = Guid.NewGuid().ToString("N");
        tokenDb.UserInfo = model.UserDeviceUnfo;

        if (user.Tokens == null)
        {
            user.Tokens = new List<AmsRefreshTokens>();
        }
        //TODO
        //tokenDb.UserInfo = 
        user.Tokens.Add(tokenDb);
        //user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
        await _userManager.UpdateAsync(user);

        var token = await GenerateJwtAsync(user);
        var response = TokenResponse.SuccessTokenResponse(token, tokenDb.RefreshToken, tokenDb.RefreshTokenExpiration, tokenDb.DeviceId);



        return await Task.FromResult(response);
    }


    private async Task<string> GenerateJwtAsync(AmsUser user)
    {
        var token = GenerateEncryptedToken(GetSigningCredentials(), await GetClaimsAsync(user));
        return token;
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfig.Secret)),
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

    private SigningCredentials GetSigningCredentials()
    {
        var secret = Encoding.UTF8.GetBytes(_appConfig.Secret);
        return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
    }
    private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
    {
        var token = new JwtSecurityToken(
            claims: claims,
            //expires: DateTime.UtcNow.AddSeconds(20),
            expires: DateTime.UtcNow.AddSeconds(20),
            signingCredentials: signingCredentials);
        var tokenHandler = new JwtSecurityTokenHandler();
        var encryptedToken = tokenHandler.WriteToken(token);
        return encryptedToken;
    }



    public async Task<TokenResponse> GetRefreshTokenAsync(RefreshTokenRequest model)
    {
        if (model is null)
        {
            return TokenResponse.FailTokenResponse("Invalid Client Token.");
        }
        model.Token = Decrypt(model.Token, key);
        var userPrincipal = GetPrincipalFromExpiredToken(model.Token);
        var userEmail = userPrincipal.FindFirstValue(ClaimTypes.Email);
        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user == null)
            return TokenResponse.FailTokenResponse("User Not Found.");
        var tokenDbItem = _authDbContext.RefreshTokens.Where(x => x.DeviceId == model.DeviceId && x.UserId == user.Id).FirstOrDefault();
        //var tokenDbItem = user.Tokens.Where(x => x.DeviceId == model.DeviceId && x.UserId == user.Id).FirstOrDefault();
        if (tokenDbItem == null)
        {
            return TokenResponse.FailTokenResponse("Invalid Refresh Client Token.");
        }



        //TODO
        if (tokenDbItem.RefreshToken != model.RefreshToken || tokenDbItem.RefreshTokenExpiration <= DateTime.Now)
            return TokenResponse.FailTokenResponse("Invalid Refresh Client Token.");
        var token = GenerateEncryptedToken(GetSigningCredentials(), await GetClaimsAsync(user));

        //TODO

        var acsessToken = token;
        tokenDbItem.RefreshToken = GenerateRefreshToken();
        tokenDbItem.RefreshTokenExpiration = DateTime.Now.AddDays(7);

        _authDbContext.Entry(tokenDbItem).State = EntityState.Modified;
        _authDbContext.SaveChanges();

        _authDbContext.Entry(tokenDbItem).State = EntityState.Detached;
        _authDbContext.SaveChanges();

        //AmsRefreshTokens tokenDb = new AmsRefreshTokens();
        //tokenDb.RefreshToken = GenerateRefreshToken();
        //tokenDb.RefreshTokenExpiration = DateTime.Now.AddDays(7);
        //user.Tokens.Add();
        //user.RefreshToken = GenerateRefreshToken();

        //await _userManager.UpdateAsync(user);

        var response = new TokenResponse { DeviceId = tokenDbItem.DeviceId, Token = acsessToken, RefreshToken = tokenDbItem.RefreshToken, RefreshTokenExpiryTime = tokenDbItem.RefreshTokenExpiration, IsSuccsesdTokenLogin = true };
        return await Task.FromResult(response);
    }


    public async Task Logout(string userId, string deviceId)
    {

        //var user = await this._userManager.FindByIdAsync(userId);
        var user = await this._userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);

        var tokens = _authDbContext.RefreshTokens.Where(x => x.UserId == user.Id);

        
        if (tokens.Count() == 0)
        {
            return;
        }
        var token = tokens.Where(x => x.DeviceId == deviceId && x.UserId == userId).FirstOrDefault();

        if (token != null)
        {
            //tokens.ExecuteDelete(token);
            _authDbContext.RefreshTokens.Attach(token);

            _authDbContext.RefreshTokens.Remove(token);
            await _authDbContext.SaveChangesAsync();


            //await _userManager.UpdateAsync(user);

        }

    }

    public async Task<List<UserDeviceInfoResponse>> GetDevices(string id)
    {
        var tokens = await _authDbContext.RefreshTokens.Where(x => x.UserId == id).ToListAsync();
        var result = tokens.Select(x => new UserDeviceInfoResponse()
            { DeviceId = x.DeviceId, DeviceInfo = x.UserInfo }).ToList();
        return result;

    }

    public async Task<bool> RemoveDevice(string id, string deviceId)
    {
        try
        {
            var token = await _authDbContext.RefreshTokens.FirstOrDefaultAsync(x => x.UserId == id && x.DeviceId == deviceId);

            if (token != null)
            {
                _authDbContext.RefreshTokens.Attach(token);
                _authDbContext.RefreshTokens.Remove(token);
                await _authDbContext.SaveChangesAsync();
            }
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    public async Task<bool> RemoveAllOtherDevice(string id, string currentDeviceId)
    {
        var tokens = await _authDbContext.RefreshTokens.Where(x => x.UserId == id && x.DeviceId != currentDeviceId).ToListAsync();

        if (tokens != null)
        {
            if (tokens.Count != 0)
            {
                foreach (var token in tokens)
                {
                    _authDbContext.RefreshTokens.Attach(token);
                    _authDbContext.RefreshTokens.Remove(token);
                    await _authDbContext.SaveChangesAsync();
                }
            }
        }
        return true;
    }

    private async Task<IEnumerable<Claim>> GetClaimsAsync(AmsUser user)
    {
        var userClaims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);
        var roleClaims = new List<Claim>();
        var permissionClaims = new List<Claim>();
        foreach (var role in roles)
        {
            roleClaims.Add(new Claim(ClaimTypes.Role, role));
            var thisRole = await _roleManager.FindByNameAsync(role);
            var allPermissionsForThisRoles = await _roleManager.GetClaimsAsync(thisRole);
            permissionClaims.AddRange(allPermissionsForThisRoles);
        }

        var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Name, user.FirstName),
                new(ClaimTypes.Surname, user.LastName),
                new(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty),
                new("UserId", user.UserId.ToString() ?? string.Empty)
            }
            .Union(userClaims)
            .Union(roleClaims)
            .Union(permissionClaims);
        return claims;
    }

}