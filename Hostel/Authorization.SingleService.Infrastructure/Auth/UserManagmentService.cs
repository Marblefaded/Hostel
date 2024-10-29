using System.Data;
using Suo.Autorization.Data.Models;
using Suo.Autorization.Data.Models.Responce;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Suo.Autorization.SingleService.Infrastructure.Interfaces;
using Suo.Autorization.SingleService.Infrastructure.Interfaces.IdentityServices;
using Suo.Autorization.SingleService.Infrastructure.RequestModels;
using Suo.Autorization.SingleService.Infrastructure.ResponseModels;
using static Suo.Autorization.Data.Models.Responce.Permissions;

namespace Suo.Autorization.SingleService.Infrastructure.Auth;

public class UserManagmentService : IServerSideService, IUserManagmentService
{
    private readonly UserManager<AmsUser> _userManager;
    private readonly ICurrentUserService _currentUserService;
    private readonly RoleManager<AmsRole> _roleManager;
    //private readonly ApplicationAuthDbContext _db;

    public UserManagmentService(UserManager<AmsUser> userManager, ICurrentUserService currentUserService, RoleManager<AmsRole> roleManager)
    {
        _userManager = userManager;
        _currentUserService = currentUserService;
        _roleManager = roleManager;
    }

    public async Task<int> GetCount()
    {
        var count = await _userManager.Users.CountAsync();
        return count;
    }

    public async Task<UserResponse> GetUser(string userId)
    {
        var user = await _userManager.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
        if (user != null)
        {
            return await Task.FromResult(Convert(user));
        }
        else
        {
            throw new NullReferenceException("User Is Null!");
        }
    }

    public async Task<UserResponse> GetHeadman(string userId)
    {
        var userData = await _userManager.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
        var resualt = await _userManager.GetRolesAsync(userData);

        if (resualt.Any(x => x == "Headman"))
        {
            return await Task.FromResult(Convert(userData));
        }
        else
        {
            throw new NullReferenceException("User Is Null!");
        }
    }

    public UserResponse Convert(AmsUser user) => new UserResponse(user);
    public async Task<List<UserResponse>> GetAll()
    {
        var users = await _userManager.Users.ToListAsync();
        var result = users.Select(x => Convert(x)).ToList();
        return await Task.FromResult(result);
    }

    public async Task<List<UserResponse>> GetAllHeadman()
    {
        var result = await _userManager.GetUsersInRoleAsync("Headman");
        var end = result.Select(x => Convert(x)).ToList();
        return await Task.FromResult(end);
    }

    public async Task<bool> CheckExistUserEmail(string email)
    {
        var result = _userManager.Users.FirstOrDefault(x => x.NormalizedEmail == email.ToUpper());
        if (result == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    public async Task<List<UserResponse>> Take(int count, int skip)
    {
        var users = await _userManager.Users.OrderBy(x => x.Id).Skip(skip).Take(count).ToListAsync();
        var result = users.Select(x => Convert(x)).ToList();
        return await Task.FromResult(result);
    }

    public async Task<UserRemoveResponse> RemoveUser(string userId)
    {

        var user = await this._userManager.FindByIdAsync(userId);
        try
        {
            IdentityResult result = null;

            result = await _userManager.DeleteAsync(user);


            if (result.Succeeded)
            {
                return UserRemoveResponse.Success();
            }
            else
            {
                return UserRemoveResponse.Error(string.Join("\n", result.Errors));
            }
        }
        catch (Exception ex)
        {
            return UserRemoveResponse.Error(ex.Message);
        }
    }


    public async Task<UserAddResponse> AddUser(UserAddRequest user)
    {

        AmsRole roleInDb = null;
        try
        {
            if (user.IsAdmin)
            {
                roleInDb = await _roleManager.FindByNameAsync("Administrator");
            }
            else
            {
                roleInDb = await _roleManager.FindByNameAsync("User");
            }
        }
        catch (Exception e)
        {

        }

        if (roleInDb == null)
        {
            if (user.IsAdmin)
            {
                var role = new AmsRole("Administrator");
                await _roleManager.CreateAsync(role);
                roleInDb = await _roleManager.FindByNameAsync("Administrator");
            }
            else
            {
                var role = new AmsRole("User");
                await _roleManager.CreateAsync(role);
                roleInDb = await _roleManager.FindByNameAsync("User");
            }

        }
        //Check if User Exists
        var superUser = new AmsUser
        {
            FirstName = user.FirstName.Trim(),
            LastName = user.LastName.Trim(),
            Email = user.Email.Trim(),
            UserName = user.Email.Substring(0, user.Email.IndexOf("@")),
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            CreatedOn = DateTime.Now,
            IsActive = true,
            PhoneNumber = user.PhoneNumber,
            //LegacyId = user.LegacyId
        };
        var superUserInDb = await _userManager.FindByEmailAsync(superUser.Email);
        if (superUserInDb == null)
        {

            await _userManager.CreateAsync(superUser, user.Password);
            IdentityResult result;
            if (user.IsAdmin)
            {
                result = await _userManager.AddToRoleAsync(superUser, "Administrator");

            }
            else
            {
                result = await _userManager.AddToRoleAsync(superUser, "User");

            }

            foreach (var permission in Permissions.GetRegisteredPermissions())
            {
                await _roleManager.AddPermissionClaim(roleInDb, permission);
            }

            if (result.Succeeded)
            {

                return UserAddResponse.Success();
            }
            else
            {

                return UserAddResponse.Error(string.Join("\n", result.Errors.Select(x => x.Description + " " + x.Code).ToList()));
            }

        }

        return UserAddResponse.Error("Unknown Error");
    }

    public async Task<UserAddResponse> AddHeadman(UserAddRequest user)
    {

        AmsRole roleInDb = null;

        roleInDb = await _roleManager.FindByNameAsync("Headman");
        if (roleInDb == null)
        {
            var role = new AmsRole("Headman");
            await _roleManager.CreateAsync(role);
            roleInDb = await _roleManager.FindByNameAsync("Headman");
        }

        var superUser = new AmsUser
        {
            FirstName = user.FirstName.Trim(),
            LastName = user.LastName.Trim(),
            Email = user.Email.Trim(),
            UserName = user.Email.Substring(0, user.Email.IndexOf("@")),
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            CreatedOn = DateTime.Now,
            IsActive = true,
            PhoneNumber = user.PhoneNumber,

        };
        var superUserInDb = await _userManager.FindByEmailAsync(superUser.Email);
        if (superUserInDb == null)
        {

            await _userManager.CreateAsync(superUser, user.Password);
            IdentityResult result;

            result = await _userManager.AddToRoleAsync(superUser, "Headman");

            foreach (var permission in Permissions.GetRegisteredPermissions())
            {
                await _roleManager.AddPermissionClaim(roleInDb, permission);
            }

            if (result.Succeeded)
            {

                return UserAddResponse.Success(superUser);
            }
            else
            {

                return UserAddResponse.Error(string.Join("\n", result.Errors.Select(x => x.Description + " " + x.Code).ToList()));
            }

        }

        return UserAddResponse.Error("Unknown Error");
    }

    public async Task<UserUpdateResponse> UpdateUser(UserUpdateRequest user)
    {
        var userFromDb = await _userManager.FindByEmailAsync(user.Email);

        if (userFromDb == null)
        {
            return UserUpdateResponse.Error($"Can not find user with email [{user.Email}]");
        }
        userFromDb.FirstName = user.FirstName.Trim();
        userFromDb.LastName = user.LastName.Trim();
        userFromDb.Email = user.Email.Trim();


        var result = await _userManager.UpdateAsync(userFromDb);

        if (result.Succeeded)
        {
            return UserUpdateResponse.Success();
        }
        else
        {
            return UserUpdateResponse.Error(result.Errors.Select(x => x.Description).First());
        }
    }



    //.GetAwaiter().GetResult();
    public async Task<UserResponse> FindByLegacyId(int legacyId)
    {
        //AmsUser user = _userManager.Users.Where(u => u.LegacyId == legacyId).Single();
        return null;
        //return Task.FromResult(Convert(user));
    }


}