using HostelDB.AlfaPruefungDb;
using HostelDB.DbRepository;
using HostelDB.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using NPOI.SS.Formula.Functions;
using NPOI.XSSF.Streaming.Values;
using Suo.Client.ViewModel;
using System.ComponentModel.DataAnnotations;
using Suo.Client.Data.Models.Services;
using Suo.Client.Models;

namespace Suo.Client.Data.Services;

public class UserService
{
    private readonly IHttpContextAccessor _accessor;
    private static HostelDbContext _context;
    EFRepository<User> _userRepository;
    private readonly int _userId;
    EFRepository<TelegramUser> _repo;
    public bool passForExist { get; set; }
    public bool passForDuble { get; set; }


    public UserService(IHttpContextAccessor accessor, HostelDbContext context)
    {
        _accessor = accessor;
        _context = context;
        var userid = accessor.HttpContext?.User.FindFirst("UserId")?.Value;  
        int tempUserId = 0;
        if (int.TryParse(userid, out tempUserId))
        {
            this._userId = tempUserId;
        }
        _userRepository = new EFRepository<User>(context);
        _repo = new EFRepository<TelegramUser>(context);
    }
    public async Task<User> GetUser(int UserId)
    {
        var user = _userRepository.GetQuery().FirstOrDefault(x => x.UserId == UserId);
        return await Task.FromResult(user);
    }
    public async Task<User> GetCurrentUser()
    {
         if (_userId == 0)
        {
            return null;
            //throw new InvalidOperationException();
        }
        var user = _userRepository.GetQuery().FirstOrDefault(x => x.UserId == _userId);
        return await Task.FromResult(user);
    }

    public async Task<User> Check(string number)
    {
        if (number.Length == 10 && (!number.StartsWith("+7") || !number.StartsWith("7"))) 
        {
            number = "8" + number;
        }
        if (number.Length > 11)
        {
            number = number.Replace(" ", "");
            number = number.Replace("-", "");
            number = number.Replace("(", "");
            number = number.Replace(")", "");
            number = number.Replace("+7", "8");
            
        }
        
        var user = _userRepository.GetQuery().FirstOrDefault(x => x.PhoneNumber == number);
        if(user != null)
        {
            var isUser = _repo.GetQuery().Any(x => x.UserId == user.UserId && !string.IsNullOrEmpty(x.TelegramUserId));
            if (isUser == false)
            {

                return null;
            }
            return user;
        }
        else
        {
            return null;
        }
        
    }
    
    public TelegramUser UserInfo(int userId)
    {
        var telegramUser = _repo.GetQuery().FirstOrDefault(x => x.UserId == userId && !string.IsNullOrEmpty(x.TelegramUserId));
        return telegramUser;
    }
}
