using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GargamelinBurnu.Models;

namespace GargamelinBurnu.Controllers;

public class HomeController : Controller
{
    [HttpGet("/")]
    public IActionResult Index()
    {
        
        return View();
    }
}