using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Services.Contracts;

namespace GargamelinBurnu.Components;

public class HomePageContentViewComponent : ViewComponent
{
    private readonly IServiceManager _manager;

    public HomePageContentViewComponent(IServiceManager manager)
    {
        _manager = manager;
    }

    public IViewComponentResult Invoke()
    {
        var subjects = _manager
            .SubjectService
            .GetAllSubjects(false)
            .Include(s => s.User)
            .Include(s => s.Category);
        
        return View(subjects);
    }
}