using GargamelinBurnu.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Contracts;

namespace GargamelinBurnu.Components;

public class LastSubjectsViewComponent : ViewComponent
{
    private readonly IServiceManager _manager;

    public LastSubjectsViewComponent(IServiceManager manager)
    {
        _manager = manager;
    }

    public IViewComponentResult Invoke()
    {
        List<SubjectCardViewModel> model = _manager
            .SubjectService
            .GetAllSubjects(false)
            .Take(15)
            .Include(s => s.User)
            .Include(s => s.Comments)
            .Include(s => s.Category)
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new SubjectCardViewModel
            {
                Username = s.User.UserName,
                Title = s.Title,
                Url = s.Url,
                CreatedAt = s.CreatedAt,
                MessageCount = s.Comments.Count,
                CategoryName = s.Category.CategoryName
            }).ToList();
        
        return View(model);
    }
}