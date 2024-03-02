
using Entities.Models;
using Entities.RequestParameters;
using Microsoft.AspNetCore.Mvc;
using GargamelinBurnu.Models;
using GargamelinBurnu.Models.Userpage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Services.Contracts;

namespace GargamelinBurnu.Controllers;

public class HomeController : Controller
{
    private readonly IServiceManager _manager;
    private readonly UserManager<User> _userManager;

    public HomeController(IServiceManager manager,
        UserManager<User> userManager)
    {
        _manager = manager;
        _userManager = userManager;
    }

    [HttpGet("/index")]
    public IActionResult indexFor()
    {
        return RedirectToAction("Index");
    }
    
    [HttpGet("/")]
    public async Task<IActionResult> Index(SubjectRequestParameters? p)
    {
        p.Pagesize = p.Pagesize <= 0 || p.Pagesize == null ? 15 : p.Pagesize;
        p.PageNumber = p.PageNumber <= 0 ? 1 : p.PageNumber;

        p.Pagesize = p.Pagesize > 15 ? 15 : p.Pagesize;

        IndexPageViewModel model;
        
        if (User.Identity.IsAuthenticated)
        {
            await _manager.BanService.checkBan(User.Identity.Name);
            
            var banuntill = _userManager
                .Users
                .Where(s => s.UserName.Equals(User.Identity.Name))
                .Select(s => s.BanUntill)
                .FirstOrDefault();
            
            if (banuntill != null)
            {
                model = new IndexPageViewModel()
                {
                    // gelmezde now dedik
                    p = p,
                    banuntill = banuntill ?? DateTime.Now
                };
            }
            else
            {
                model = new IndexPageViewModel()
                {
                    p = p,
                };
            }
        }
        else
        {
            model = new IndexPageViewModel()
            {
                p = p,
            };
        }
        
        return View("Index",model);
    }
    
    [HttpGet("/son_mesajlar")]
    public async Task<IActionResult> LastComments(SubjectRequestParameters p)
    {
        p.Pagesize = p.Pagesize <= 0 || p.Pagesize == null ? 15 : p.Pagesize;
        p.PageNumber = p.PageNumber <= 0 ? 1 : p.PageNumber;
        
        p.Pagesize = p.Pagesize > 15 ? 15 : p.Pagesize;
        
        if (User.Identity.IsAuthenticated)
        {
            await _manager.BanService.checkBan(User.Identity.Name);
        }
        
        var model = new IndexPageViewModel()
        {
            p = p,
            section = "Son mesajlar"
        };
        return View("Index",model);
    }
    
    [HttpGet("/konularim")]
    public async Task<IActionResult> userSubjects(SubjectRequestParameters p)
    {
        p.Pagesize = p.Pagesize <= 0 || p.Pagesize == null ? 15 : p.Pagesize;
        p.PageNumber = p.PageNumber <= 0 ? 1 : p.PageNumber;
        
        p.Pagesize = p.Pagesize > 15 ? 15 : p.Pagesize;
        
        if (User.Identity.IsAuthenticated)
        {
            await _manager.BanService.checkBan(User.Identity.Name);
        }
        
        var model = new IndexPageViewModel()
        {
            p = p,
            section = "Kullanıcı konuları"
        };
        return View("Index",model);
    }

    [HttpGet("/takip")]
    public async Task<IActionResult> followedSubjects(SubjectRequestParameters p)
    {
        p.Pagesize = p.Pagesize <= 0 || p.Pagesize == null ? 15 : p.Pagesize;
        p.PageNumber = p.PageNumber <= 0 ? 1 : p.PageNumber;
        
        p.Pagesize = p.Pagesize > 15 ? 15 : p.Pagesize;
        
        if (User.Identity.IsAuthenticated)
        {
            await _manager.BanService.checkBan(User.Identity.Name);
        }
        
        var model = new IndexPageViewModel()
        {
            p = p,
            section = "Takip edilen konular"
        };
        return View("Index",model);
    }
    

    [HttpGet("/bolum/{category}")]
    public async Task<IActionResult> Tags(string? category,SubjectRequestParameters p)
    {
        p.Pagesize = p.Pagesize <= 0 || p.Pagesize == null ? 15 : p.Pagesize;
        p.PageNumber = p.PageNumber <= 0 ? 1 : p.PageNumber;
        
        p.Pagesize = p.Pagesize > 15 ? 15 : p.Pagesize;
        
        if (User.Identity.IsAuthenticated)
        {
            await _manager.BanService.checkBan(User.Identity.Name);
        }
        
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