using Ams.Pm.Wasm.Core.Auth.Identity;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Suo.Autorization.Data.Models;
using Suo.Autorization.Data.Models.Responce;

namespace Suo.Autorization.Extentions;

public class DatabaseSeederCreater
{
    private readonly UserManager<AmsUser> _userManager;
    private readonly RoleManager<AmsRole> _roleManager;
    private readonly ApplicationAuthDbContext _db;

    public DatabaseSeederCreater(UserManager<AmsUser> userManager, RoleManager<AmsRole> roleManager, ApplicationAuthDbContext db)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _db = db;
    }

    public async Task<string> AddCreater(string email, string password)
    {

        var userRole = new AmsRole("Creator");
        AmsRole userRoleInDb = null;
        try
        {
            userRoleInDb = await _roleManager.FindByNameAsync("Creator");
        }
        catch (Exception e)
        {


        }

        if (userRoleInDb == null)
        {
            await _roleManager.CreateAsync(userRole);
            userRoleInDb = await _roleManager.FindByNameAsync("Creator");
        }

        var User = new AmsUser
        {
            FirstName = "creator",
            LastName = "creator",
            Email = email,
            UserName = email.Substring(0, email.IndexOf("@")),
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            CreatedOn = DateTime.Now,
            IsActive = true,
            
        };
        var UserInDb = await _userManager.FindByEmailAsync(User.Email);
        if (UserInDb == null)
        {

            await _userManager.CreateAsync(User, password);
            var result = await _userManager.AddToRoleAsync(User, "Creator");
            if (result.Succeeded)
            {
                return $"{email}/{password}";
            }
            else
            {
                return JsonConvert.SerializeObject(result.Errors);
            }

        }
        foreach (var permission in Permissions.GetRegisteredPermissions())
        {
            await _roleManager.AddPermissionClaim(userRoleInDb, permission);
        }


        return "";
    }
}
