using Entities.Dtos.Ban;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace GargamelinBurnu.Areas.Admin.Controllers;

[Area("Admin")]
public class BanController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly IServiceManager _manager;

    public BanController(UserManager<User> userManager,
        IServiceManager manager)
    {
        _userManager = userManager;
        _manager = manager;
    }

    [HttpGet("/ban/{username}")]
    [Authorize(Roles = "Admin,Moderator")]
    public IActionResult Index(string username)
    {
        var model = new BanCauseDto()
        {
            username = username
        };
        
        return View("Index",model);
    }
    
    [HttpPost("/ban/{username}")]
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> Index(BanCauseDto model)
    {
        if (ModelState.IsValid)
        {
            var result = await _manager.BanService
                .CreateBan(model);

            
            if (result == 1)
            {
                return RedirectToAction("Index", "Panel");    
            }
            
            ModelState.AddModelError("","Bir hata oluştu tekrar deneyin.");
        }
        
        
        return View(model);
    }
}