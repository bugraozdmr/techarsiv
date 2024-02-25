
using Entities.RequestParameters;
using Microsoft.AspNetCore.Mvc;
using GargamelinBurnu.Models;

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
    [HttpGet("/index")]
    public IActionResult Index(SubjectRequestParameters? p)
    {
        p.Pagesize = p.Pagesize <= 0 || p.Pagesize == null ? 15 : p.Pagesize;
        p.PageNumber = p.PageNumber <= 0 ? 1 : p.PageNumber;

        p.Pagesize = p.Pagesize > 15 ? 15 : p.Pagesize;
        
        var model = new IndexPageViewModel()
        {
            p = p
        };
        return View("Index",model);
    }
    
    [HttpGet("/son_yorumlar")]
    public IActionResult LastComments(SubjectRequestParameters p)
    {
        p.Pagesize = p.Pagesize <= 0 || p.Pagesize == null ? 15 : p.Pagesize;
        p.PageNumber = p.PageNumber <= 0 ? 1 : p.PageNumber;
        
        p.Pagesize = p.Pagesize > 15 ? 15 : p.Pagesize;
        
        var model = new IndexPageViewModel()
        {
            p = p,
            section = "Son yorumlar"
        };
        return View("Index",model);
    }

    [HttpGet("/bolum/{category}")]
    public IActionResult Tags(string? category,SubjectRequestParameters p)
    {
        p.Pagesize = p.Pagesize <= 0 || p.Pagesize == null ? 15 : p.Pagesize;
        p.PageNumber = p.PageNumber <= 0 ? 1 : p.PageNumber;
        
        p.Pagesize = p.Pagesize > 15 ? 15 : p.Pagesize;
        
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

        model.p = p;
        
        return View(model);
    }
}