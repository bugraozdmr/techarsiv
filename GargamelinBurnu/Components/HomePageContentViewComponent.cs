using GargamelinBurnu.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Services.Contracts;

namespace GargamelinBurnu.Components;

public class HomePageContentViewComponent : ViewComponent
{
    private readonly IServiceManager _manager;
    private readonly ILikeDService _likeDService;

    public HomePageContentViewComponent(IServiceManager manager,
        ILikeDService likeDService)
    {
        _manager = manager;
        _likeDService = likeDService;
    }

    public IViewComponentResult Invoke()
    {
        List<TitleViewModel> model;

        model = _manager
            .SubjectService
            .GetAllSubjects(false)
            .OrderByDescending(s => s.CreatedAt)
            .Include(s => s.Comments)
            .Include(s => s.User)
            .Include(s => s.Category)
            .Select(s => new TitleViewModel()
            {
                Username = s.User.UserName,
                CategoryName = s.Category.CategoryName,
                Title = s.Title,
                createdAt = s.CreatedAt,
                SubjectId = s.SubjectId,
                Url = s.Url,
                CommentCount = s.Comments.Count
            }).ToList();

        foreach (var title in model)
        {
            title.HeartCount = _likeDService
                .Hearts
                .Count(s => s.SubjectId.Equals(title.SubjectId));
        }
        
        return View(model);
    }
}