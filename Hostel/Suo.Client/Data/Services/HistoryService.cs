using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using HostelDB.DbRepository;
using HostelDB.Model;
using Microsoft.EntityFrameworkCore;
using HostelDB.AlfaPruefungDb;
using Suo.Client.Data.Models.Services;
using Suo.Client.Models.Request;
using Claim = HostelDB.Model.Claim;

namespace Suo.Client.Data.Services;

public class HistoryService
{

    private int _userId { get; set; } = 0;

    EFRepository<User> _userRepository;
    EFRepository<Claim> _claimRepository;

    public HistoryService(IHttpContextAccessor accessor, HostelDbContext context)
    {

            var userid = accessor.HttpContext?.User.FindFirst("UserId")?.Value;
            int tempUserId = 0;
            if (int.TryParse(userid,out tempUserId))
            {
                this._userId = tempUserId;
            }
            _userRepository = new EFRepository<User>(context);
            _claimRepository = new EFRepository<Claim>(context);
       
    }

    public async Task<List<Claim>> GetClaimsByUser()
    {
        if (_userId ==0)
        {
            return new List<Claim>();
        }


        var result = await _claimRepository.GetQuery().Where(x => x.UserId == this._userId).OrderBy(x => x.CreateDate).ToListAsync();

        return result;

    }
}