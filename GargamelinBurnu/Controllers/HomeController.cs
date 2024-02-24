using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GargamelinBurnu.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Services.Contracts;

namespace GargamelinBurnu.Controllers;

public class HomeController : Controller
{
    private readonly IServiceManager _manager;

    public HomeController(IServiceManager manager)
    {
        _manager = manager;
    }

    [HttpGet("/")]
    public IActionResult Index(string? section)
    {
        section = section ?? "son_eklenenler";
        return View("Index",section);
    }
    
    public IActionResult LastComments()
    {
        return RedirectToAction("Index", new { section = "son_yorumlar" });
    }

    [HttpGet("/bolum/{category}")]
    public IActionResult Tags(string? category)
    {
        var model = new CategoryViewModel();

        model = _manager
            .CategoryService
            .GetAllCategories(false)
            .Where(c => c.CategoryUrl.Equals(category))
            .Select(c => new CategoryViewModel()
            {
                CategoryDesc = c.Description,
                CategoryName = c.CategoryName,
                CategoryUrl = c.CategoryUrl
            })
            .FirstOrDefault();
        
        return View(model);
    }
}