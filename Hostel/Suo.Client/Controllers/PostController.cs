using System.Text;
using HostelDB.AlfaPruefungDb;
using HostelDB.Model;
using Suo.Client.Attributes;
using Suo.Client.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Suo.Client.Controllers;

[AuthorizeJwt]
public class PostController : Controller
{
    private HostelDbContext _context;

    public PostController(HostelDbContext dbContext)
    {
        _context = dbContext;
    }
    
    [Route("/News")]
    public async Task<IActionResult> News()
    {
        return View();
    }

    public async Task<List<Post>> GetPostsModelByStep(int step)
    {
        var model = _context.GetPostPageByStep(step);
        return model;
    }
    
    public async Task<List<string>> GetListPostView(int step)
    {
        var model = await GetPostsModelByStep(step);
    
        var result = new List<string>();
    
        foreach (var item in model)
        {    
            result.Add(await this.RenderViewToStringAsync("Post", item));
        }
    
        return result;
    }
}
