using Entities.Models;
using GargamelinBurnu.Areas.Admin.Models.Panel;
using GargamelinBurnu.Models.Userpage;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GargamelinBurnu.Areas.Admin.Components;

public class OfficialUserCardViewComponent : ViewComponent
{
    private readonly UserManager<User> _userManager;

    public OfficialUserCardViewComponent(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public IViewComponentResult Invoke()
    {
        var model = new OfficialUserCardViewModel()
        {
            Username = User.Identity.Name,
            ImageUrl = _userManager
                .Users
                .Where(u => u.UserName.Equals(User.Identity.Name))
                .Select(u => u.Image)
                .FirstOrDefault()
        };
        
        
        return View(model);
    }
}