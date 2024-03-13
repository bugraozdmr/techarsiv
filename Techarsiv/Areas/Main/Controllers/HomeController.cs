using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace GargamelinBurnu.Areas.Main.Controllers;

[Area("Main")]
public class HomeController : Controller
{
    private readonly IServiceManager _manager;

    public HomeController(IServiceManager manager)
    {
        _manager = manager;
    }

    [HttpGet("/")]
    public IActionResult Home()
    {
        
        
        return View();
    }
}