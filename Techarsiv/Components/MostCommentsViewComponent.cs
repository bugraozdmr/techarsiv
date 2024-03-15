using GargamelinBurnu.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Contracts;

namespace GargamelinBurnu.Components;

public class MostCommentsViewComponent : ViewComponent
{
    private readonly IServiceManager _manager;

    public MostCommentsViewComponent(IServiceManager manager)
    {
        _manager = manager;
    }

    public IViewComponentResult Invoke()
    {
        var most = _manager
            .SubjectService
            .GetAllSubjects(false)
            .Include(s => s.Comments)
            .OrderByDescending(s => s.Comments.Count())
            .Take(5)
            .Select(s => new MostHeartViewModel
            {
                owner = s.User.UserName,
                OwnerImage = s.User.Image,
                Title = s.Title,
                Url = s.Url,
                CreatedAt = s.CreatedAt,
                CategoryUrl = s.Category.CategoryUrl,
                CategoryName = s.Category.CategoryName,
                MessageCount = s.Comments.Count
            }).ToList();

        return View(most);
    }
}