using Microsoft.AspNetCore.Mvc;

namespace GargamelinBurnu.Areas.Main.Controllers;

[Area("Main")]
public class HomeController : Controller
{
    [HttpGet("/")]
    public IActionResult Index()
    {
        return View();
    }
}