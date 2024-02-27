using Entities.Models;
using GargamelinBurnu.Areas.Admin.Models.Panel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace GargamelinBurnu.Areas.Admin.Components;

public class infocardsViewComponent : ViewComponent
{
    private readonly IServiceManager _manager;
    private readonly UserManager<User> _userManager;

    public infocardsViewComponent(IServiceManager manager,
        UserManager<User> userManager)
    {
        _manager = manager;
        _userManager = userManager;
    }

    public IViewComponentResult Invoke()
    {
        var model = new PanelCardViewModel()
        {
            CommentCount = _manager
                .CommentService
                .getAllComments(false)
                .Count(),
            SubjectCount = _manager
                .SubjectService
                .GetAllSubjects(false)
                .Count(s => s.IsActive.Equals(true)),
            UserCount = _userManager
                .Users
                .Count(),
            waitingSubjectCount = _manager
                .SubjectService
                .GetAllSubjects(false)
                .Count(s => s.IsActive.Equals(false))
        };
        
        return View(model);
    }
}