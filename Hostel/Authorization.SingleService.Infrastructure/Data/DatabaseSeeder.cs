using Ams.Pm.Wasm.Core.Auth.Identity;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Suo.Autorization.Data.Models;
using Suo.Autorization.Data.Models.Responce;

namespace Suo.Autorization.Extentions;

public class DatabaseSeeder
{
    private readonly UserManager<AmsUser> _userManager;
    private readonly RoleManager<AmsRole> _roleManager;
    private readonly ApplicationAuthDbContext _db;

    public DatabaseSeeder(UserManager<AmsUser> userManager, RoleManager<AmsRole> roleManager, ApplicationAuthDbContext db)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _db = db;
    }


    public async Task<string> AddAdministrator(string email, string password)
    {
        //Task.Run(async () =>
        {
            //Check if Role Exists
            var adminRole = new AmsRole("Administrator");
            AmsRole adminRoleInDb = null;
            try
            {
                adminRoleInDb = await _roleManager.FindByNameAsync("Administrator");
            }
            catch (Exception e)
            {

            }

            if (adminRoleInDb == null)
            {
                await _roleManager.CreateAsync(adminRole);
                adminRoleInDb = await _roleManager.FindByNameAsync("Administrator");
            }
            //Check if User Exists
            var superUser = new AmsUser
            {
                FirstName = "admin",
                LastName = "admin",
                Email = email,
                UserName = email.Substring(0, email.IndexOf("@")),
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                CreatedOn = DateTime.Now,
                IsActive = true
            };
            var superUserInDb = await _userManager.FindByEmailAsync(superUser.Email);
            if (superUserInDb == null)
            {

                await _userManager.CreateAsync(superUser, password);
                var result = await _userManager.AddToRoleAsync(superUser, "Administrator");
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
                await _roleManager.AddPermissionClaim(adminRoleInDb, permission);
            }
        }
        return "";
    }
}