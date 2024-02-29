using Entities.Models;
using GargamelinBurnu.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Contracts;

namespace GargamelinBurnu.Components;

public class BanWidgetViewComponent : ViewComponent
{
    private readonly IServiceManager _manager;
    private readonly UserManager<User> _userManager;

    public BanWidgetViewComponent(IServiceManager manager,
        UserManager<User> userManager)
    {
        _manager = manager;
        _userManager = userManager;
    }

    public IViewComponentResult Invoke()
    {
        BanWidgetViewModel model = new BanWidgetViewModel();
        
        if (User.Identity.IsAuthenticated)
        {
            var banuntill = _userManager
                .Users
                .Where(s => s.UserName.Equals(User.Identity.Name))
                .Select(s => s.BanUntill)
                .FirstOrDefault();

            if (banuntill is not null)
            {
                model = _userManager
                    .Users
                    .Where(s => s.UserName.Equals(User.Identity.Name))
                    .Select(s => new BanWidgetViewModel()
                    {
                        // zaten düşmez
                        EndTime = s.BanUntill ?? DateTime.Now,
                        Image = s.Image,
                        Username = s.UserName
                    })
                    .FirstOrDefault();
            }
        }
        
        
        return View(model);
    }
}