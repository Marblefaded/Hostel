using HostelDB.AlfaPruefungDb;
using HostelDB.Model;
using Suo.Client.Attributes;
using Suo.Client.Data.Services;
using Suo.Client.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;

namespace Suo.Client.Controllers;

[AuthorizeJwt]

public class ClaimsController : Controller
{
    private readonly HistoryService _service;
    private HostelDbContext db;
    
    public ClaimsController(HistoryService service, HostelDbContext db)
    {
        _service = service;
        this.db = db;
    }

    [HttpGet]
    [Route("/Claims")]
    public async Task<IActionResult> Claims()
    {
        return View();
    }
    // GET
    public async Task<List<Claim>> GetHistoryClaim()
    {
        var claims = await _service.GetClaimsByUser();
        return claims;
    }
    

    [HttpPost]
    public async Task<string> GetClaims()
    {
        var templateList = await db.DbSetClaimTemplate.ToListAsync();

        List<ClaimTemplate> claimTemplates = new List<ClaimTemplate>();
        claimTemplates = templateList;
        //return View();
        return JsonConvert.SerializeObject(claimTemplates);
    }
    
    
}