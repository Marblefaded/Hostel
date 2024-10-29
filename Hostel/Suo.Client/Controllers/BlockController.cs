using Suo.Client.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Suo.Client.Controllers;
[AuthorizeJwt]

public class BlockController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}