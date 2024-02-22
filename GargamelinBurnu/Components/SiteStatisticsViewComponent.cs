using Entities.Models;
using GargamelinBurnu.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace GargamelinBurnu.Components;

public class SiteStatisticsViewComponent : ViewComponent
{
    private readonly IServiceManager _manager;
    private readonly UserManager<User> _userManager;

    public SiteStatisticsViewComponent(IServiceManager manager,
        UserManager<User> userManager)
    {
        _manager = manager;
        _userManager = userManager;
    }

    public IViewComponentResult Invoke()
    {
        var model = new SiteStatisticsViewModel()
        {
            CommentCount =  _manager.CommentService.getAllComments(false).Count(),
            Usercount = _userManager.Users.Count(),
            SubjectCount = _manager.SubjectService.GetAllSubjects(false).Count(),
            LastUser = _userManager.Users.OrderBy(s => s.CreatedAt)
                .Select(s => s.UserName).LastOrDefault()
        };
        return View(model);
    }
}