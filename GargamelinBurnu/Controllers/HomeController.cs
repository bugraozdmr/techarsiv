using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GargamelinBurnu.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace GargamelinBurnu.Controllers;

public class HomeController : Controller
{
    [HttpGet("/")]
    public IActionResult Index(string? section)
    {
        section = section ?? "son eklenenler";
        return View("Index",section);
    }
    
    public IActionResult LastComments()
    {
        return RedirectToAction("Index", new { section = "Son yorumlar" });
    }
}