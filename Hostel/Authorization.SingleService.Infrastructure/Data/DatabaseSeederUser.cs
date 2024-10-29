using Ams.Pm.Wasm.Core.Auth.Identity;
using Suo.Autorization.SingleService.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using Suo.Autorization.Data.Models;
using Suo.Autorization.Data.Models.Responce;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Suo.Autorization.Extentions;

public class DatabaseSeederUser
{
    private readonly UserManager<AmsUser> _userManager;
    private readonly RoleManager<AmsRole> _roleManager;
    
    private readonly ApplicationAuthDbContext _db;
    

    public DatabaseSeederUser(UserManager<AmsUser> userManager, RoleManager<AmsRole> roleManager, ApplicationAuthDbContext db)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _db = db;
    }

    public async Task<string> AddUser(string email, string password, int UserId)
    {
        /*using (SqlConnection connection = new SqlConnection("Server=92.205.60.239,7133;Database=Hostel;User Id=sa;Password=Wladgood1051!;TrustServerCertificate=Yes"))
        {
            var GetName = $"SELECT Name, Surname FROM [dbo].[User] WHERE UserId = '{UserId}'";
            SqlCommand command = new SqlCommand(GetName, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows) // если есть данные
            {
                while (reader.Read()) // построчно считываем данные
                {
                    userName = reader.GetValue(0).ToString();
                    userSurname = reader.GetValue(1).ToString();

                }
            }
            reader.Close();

           
        }*/
        var resualt = Helper<Tuple<string, string>>.ExecuteSQLQuery(_db, $"SELECT Name, Surname FROM hostel.User WHERE UserId = '{UserId}'", x => new Tuple<string, string>((string)x[0], (string)x[1]));
        var userName = resualt.FirstOrDefault().Item1; 
        var userSurname = resualt.FirstOrDefault().Item2;
        var userRole = new AmsRole("User");

        AmsRole userRoleInDb = null;
        try
        {
            userRoleInDb = await _roleManager.FindByNameAsync("User");
        }
        catch (Exception e)
        {


        }

        if (userRoleInDb == null)
        {
            await _roleManager.CreateAsync(userRole);
            userRoleInDb = await _roleManager.FindByNameAsync("User");
        }

        var User = new AmsUser
        {
            FirstName = userName,
            LastName = userSurname,
            Email = email,
            UserName = email.Substring(0, email.IndexOf("@")),
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            CreatedOn = DateTime.Now,
            IsActive = true,
            UserId = UserId
        };
        var UserInDb = await _userManager.FindByEmailAsync(User.Email);
        if (UserInDb == null)
        {

            await _userManager.CreateAsync(User, password);
            var result = await _userManager.AddToRoleAsync(User, "User");
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
